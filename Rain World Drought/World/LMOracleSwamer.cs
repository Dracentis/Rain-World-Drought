using System;
using System.Collections.Generic;
using CoralBrain;
using RWCustom;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class LMOracleSwarmer : OracleSwarmer
{
    // Token: 0x060022D6 RID: 8918 RVA: 0x00212894 File Offset: 0x00210A94
    public LMOracleSwarmer(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        travelDirection = Custom.RNV();
        currentBehavior = new LMOracleSwarmer.Behavior(this);
        color = currentBehavior.color;
        stuckList = new System.Collections.Generic.List<Vector2>();
    }

    // Token: 0x060022D7 RID: 8919 RVA: 0x002128D4 File Offset: 0x00210AD4
    public override void NewRoom(Room newRoom)
    {
        base.NewRoom(newRoom);
        system = null;
        int num = 0;
        while (num < newRoom.updateList.Count && system == null)
        {
            if (newRoom.updateList[num] is CoralNeuronSystem)
            {
                system = (newRoom.updateList[num] as CoralNeuronSystem);
            }
            num++;
        }
        stuckList.Clear();
        stuckListCounter = 10;
    }

    // Token: 0x060022D8 RID: 8920 RVA: 0x00212994 File Offset: 0x00210B94
    public override void Update(bool eu)
    {
        if (system != null && system.Frozen)
        {
            return;
        }
        base.Update(eu);
        if (!room.readyForAI || room.gravity * affectedByGravity > 0.5f)
        {
            return;
        }
        direction = travelDirection;
        switch (mode)
        {
            case MovementMode.Swarm:
                SwarmBehavior();
                if (onlySwarm > 0)
                {
                    onlySwarm--;
                }
                else if (currentBehavior.suckle && UnityEngine.Random.value < 0.1f && system != null && system.mycelia.Count > 0)
                {
                    Mycelium mycelium = system.mycelia[UnityEngine.Random.Range(0, system.mycelia.Count)];
                    if (Custom.DistLess(firstChunk.pos, mycelium.Tip, 400f) && room.VisualContact(firstChunk.pos, mycelium.Tip))
                    {
                        bool flag = false;
                        int num = 0;
                        while (num < otherSwarmers.Count && !flag)
                        {
                            if ((otherSwarmers[num] as LMOracleSwarmer).suckleMyc == mycelium)
                            {
                                flag = true;
                            }
                            num++;
                        }
                        if (!flag)
                        {
                            mode = MovementMode.SuckleMycelia;
                            suckleMyc = mycelium;
                            attachedToMyc = false;
                        }
                    }
                }
                else if (room.aimap.getAItile(firstChunk.pos).terrainProximity < 7)
                {
                    if (stuckListCounter > 0)
                    {
                        stuckListCounter--;
                    }
                    else
                    {
                        stuckList.Insert(0, firstChunk.pos);
                        if (stuckList.Count > 10)
                        {
                            stuckList.RemoveAt(stuckList.Count - 1);
                        }
                        stuckListCounter = 80;
                    }
                    if (UnityEngine.Random.value < 0.025f && stuckList.Count > 1 && Custom.DistLess(firstChunk.pos, stuckList[stuckList.Count - 1], 200f))
                    {
                        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
                        for (int i = 0; i < room.abstractRoom.connections.Length; i++)
                        {
                            if (room.aimap.ExitDistanceForCreature(room.GetTilePosition(firstChunk.pos), i, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly)) > 0)
                            {
                                list.Add(i);
                            }
                        }
                        if (list.Count > 0)
                        {
                            mode = MovementMode.FollowDijkstra;
                            dijkstra = list[UnityEngine.Random.Range(0, list.Count)];
                        }
                    }
                }
                break;
            case MovementMode.SuckleMycelia:
                if (suckleMyc == null)
                {
                    mode = MovementMode.Swarm;
                }
                else if (attachedToMyc)
                {
                    direction = Custom.DirVec(firstChunk.pos, suckleMyc.Tip);
                    float num2 = Vector2.Distance(firstChunk.pos, suckleMyc.Tip);
                    firstChunk.vel -= (2f - num2) * direction * 0.15f;
                    firstChunk.pos -= (2f - num2) * direction * 0.15f;
                    suckleMyc.points[suckleMyc.points.GetLength(0) - 1, 0] += (2f - num2) * direction * 0.35f;
                    suckleMyc.points[suckleMyc.points.GetLength(0) - 1, 2] += (2f - num2) * direction * 0.35f;
                    travelDirection = new Vector2(0f, 0f);
                    if (UnityEngine.Random.value < 0.05f)
                    {
                        room.AddObject(new NeuronSpark((firstChunk.pos + suckleMyc.Tip) / 2f));
                    }
                    if (UnityEngine.Random.value < 0.0125f)
                    {
                        suckleMyc = null;
                        onlySwarm = UnityEngine.Random.Range(40, 400);
                    }
                }
                else
                {
                    travelDirection = Custom.DirVec(firstChunk.pos, suckleMyc.Tip);
                    if (Custom.DistLess(firstChunk.pos, suckleMyc.Tip, 5f))
                    {
                        attachedToMyc = true;
                    }
                    else if (UnityEngine.Random.value < 0.05f && !room.VisualContact(firstChunk.pos, suckleMyc.Tip))
                    {
                        suckleMyc = null;
                    }
                }
                color = Vector2.Lerp(color, currentBehavior.color, 0.05f);
                break;
            case MovementMode.FollowDijkstra:
                {
                    IntVector2 tilePosition = room.GetTilePosition(firstChunk.pos);
                    int num3 = -1;
                    int num4 = int.MaxValue;
                    for (int j = 0; j < 4; j++)
                    {
                        if (!room.GetTile(tilePosition + Custom.fourDirections[j]).Solid)
                        {
                            int num5 = room.aimap.ExitDistanceForCreature(tilePosition + Custom.fourDirections[j], dijkstra, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly));
                            if (num5 > 0 && num5 < num4)
                            {
                                num3 = j;
                                num4 = num5;
                            }
                        }
                    }
                    if (num3 > -1)
                    {
                        travelDirection += Custom.fourDirections[num3].ToVector2().normalized * 1.4f + Custom.RNV() * UnityEngine.Random.value * 0.5f;
                    }
                    else
                    {
                        mode = MovementMode.Swarm;
                    }
                    travelDirection.Normalize();
                    int num6 = room.aimap.ExitDistanceForCreature(tilePosition, dijkstra, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Fly));
                    if ((UnityEngine.Random.value < 0.025f && num6 < 34) || num6 < 12 || dijkstra < 0 || UnityEngine.Random.value < 0.0025f || (room.aimap.getAItile(firstChunk.pos).terrainProximity >= 7 && UnityEngine.Random.value < 0.0166666675f))
                    {
                        mode = MovementMode.Swarm;
                    }
                    break;
                }
        }
        firstChunk.vel += travelDirection * 0.3f * (1f - room.gravity * affectedByGravity);
        firstChunk.vel *= Custom.LerpMap(firstChunk.vel.magnitude, 0.2f, 3f, 1f, 0.9f);
        if (currentBehavior.Dead)
        {
            Vector2 vector = currentBehavior.color;
            currentBehavior = new LMOracleSwarmer.Behavior(this);
            if (UnityEngine.Random.value > 0.25f)
            {
                currentBehavior.color = vector;
            }
        }
        else if (currentBehavior.leader == this)
        {
            currentBehavior.life -= currentBehavior.deathSpeed;
        }
        if (abstractPhysicalObject.destroyOnAbstraction && grabbedBy.Count > 0)
        {
            abstractPhysicalObject.destroyOnAbstraction = false;
        }
    }

    // Token: 0x060022D9 RID: 8921 RVA: 0x002131D0 File Offset: 0x002113D0
    private void SwarmBehavior()
    {
        Vector2 a = default(Vector2);
        float num = 0f;
        float num2 = currentBehavior.torque;
        Vector2 a2 = new Vector2(0f, 0f);
        float num3 = 0f;
        float num4 = currentBehavior.revolveSpeed;
        float num5 = 0f;
        int num6 = 0;
        int num7 = -1;
        for (int i = listBreakPoint; i < otherSwarmers.Count; i++)
        {
            if (otherSwarmers[i].slatedForDeletetion)
            {
                otherSwarmers.RemoveAt(i);
                num7 = i;
                break;
            }
            if (Custom.DistLess(firstChunk.pos, otherSwarmers[i].firstChunk.pos, 400f) && (otherSwarmers[i] as LMOracleSwarmer).mode != MovementMode.SuckleMycelia)
            {
                float num8 = Mathf.InverseLerp(400f, 0f, Vector2.Distance(firstChunk.pos, otherSwarmers[i].firstChunk.pos));
                a += otherSwarmers[i].firstChunk.pos * num8;
                num2 += (otherSwarmers[i] as LMOracleSwarmer).torque * num8;
                num4 += (otherSwarmers[i] as LMOracleSwarmer).revolveSpeed * num8;
                num5 += (otherSwarmers[i].rotation - Mathf.Floor(otherSwarmers[i].rotation)) * num8;
                num += num8;
                a2 += (otherSwarmers[i] as LMOracleSwarmer).color * Mathf.InverseLerp(0.9f, 1f, num8);
                num3 += Mathf.InverseLerp(0.9f, 1f, num8);
                travelDirection += (otherSwarmers[i].firstChunk.pos + (otherSwarmers[i] as LMOracleSwarmer).travelDirection * currentBehavior.aimInFront * num8 - firstChunk.pos).normalized * num8 * 0.01f;
                travelDirection += (firstChunk.pos - otherSwarmers[i].firstChunk.pos).normalized * Mathf.InverseLerp(currentBehavior.idealDistance, 0f, Vector2.Distance(firstChunk.pos, otherSwarmers[i].firstChunk.pos)) * 0.1f;
                if (currentBehavior.Dominance < (otherSwarmers[i] as LMOracleSwarmer).currentBehavior.Dominance * Mathf.Pow(num8, 4f))
                {
                    currentBehavior = (otherSwarmers[i] as LMOracleSwarmer).currentBehavior;
                }
                num6++;
                if (num6 > 30)
                {
                    num7 = i;
                    break;
                }
            }
        }
        listBreakPoint = num7 + 1;
        travelDirection += Custom.RNV() * 0.5f * currentBehavior.randomVibrations;
        if (num > 0f)
        {
            travelDirection += Custom.PerpendicularVector(firstChunk.pos, a / num) * torque;
            num5 /= num;
            num5 += Mathf.Floor(rotation);
            if (Mathf.Abs(rotation - num5) < 0.4f)
            {
                rotation = Mathf.Lerp(rotation, num5, 0.05f);
            }
        }
        torque = Mathf.Lerp(torque, num2 / (1f + num), 0.1f);
        revolveSpeed = Mathf.Lerp(revolveSpeed, num4 / (1f + num), 0.2f);
        if (num3 > 0f)
        {
            color = Vector2.Lerp(color, a2 / num3, 0.4f);
        }
        color = Vector2.Lerp(color, currentBehavior.color, 0.05f);
        if (room.aimap.getAItile(firstChunk.pos).terrainProximity < 5)
        {
            IntVector2 tilePosition = room.GetTilePosition(firstChunk.pos);
            Vector2 a3 = new Vector2(0f, 0f);
            for (int j = 0; j < 4; j++)
            {
                if (!room.GetTile(tilePosition + Custom.fourDirections[j]).Solid && !room.aimap.getAItile(tilePosition + Custom.fourDirections[j]).narrowSpace)
                {
                    float num9 = 0f;
                    for (int k = 0; k < 4; k++)
                    {
                        num9 += (float)room.aimap.getAItile(tilePosition + Custom.fourDirections[j] + Custom.fourDirections[k]).terrainProximity;
                    }
                    a3 += Custom.fourDirections[j].ToVector2() * num9;
                }
            }
            travelDirection = Vector2.Lerp(travelDirection, a3.normalized * 2f, 0.5f * Mathf.Pow(Mathf.InverseLerp(5f, 1f, (float)room.aimap.getAItile(firstChunk.pos).terrainProximity), 0.25f));
        }
        travelDirection.Normalize();
    }

    // Token: 0x060022DA RID: 8922 RVA: 0x00213848 File Offset: 0x00211A48
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Color color;
        sLeaser.sprites[0].scale = 0.8f;
        if (!dark)
        {
            color = Custom.HSL2RGB((this.color.x >= 0.5f) ? Custom.LerpMap(this.color.x, 0.5f, 1f, 0.6666667f, 0.997222245f) : Custom.LerpMap(this.color.x, 0f, 0.5f, 0.444444448f, 0.6666667f), 0.4f, 0.5f + 0.5f * this.color.y);
            sLeaser.sprites[4].color = Custom.HSL2RGB((this.color.x >= 0.5f) ? Custom.LerpMap(this.color.x, 0.5f, 1f, 0.6666667f, 0.997222245f) : Custom.LerpMap(this.color.x, 0f, 0.5f, 0.444444448f, 0.6666667f), 1f - this.color.y, Mathf.Lerp(0.8f + 0.2f * Mathf.InverseLerp(0.4f, 0.1f, this.color.x), 0.35f, Mathf.Pow(this.color.y, 2f)));
        }
        else
        {
            color = Custom.HSL2RGB((this.color.x > 0.5f) ? Custom.LerpMap(this.color.x, 0.5f, 1f, 0.6666667f, 0.997222245f) : 0.6666667f, 0.4f, Mathf.Lerp(0.1f, 0.5f, this.color.y));
            sLeaser.sprites[4].color = Custom.HSL2RGB((this.color.x > 0.5f) ? Custom.LerpMap(this.color.x, 0.5f, 1f, 0.6666667f, 0.997222245f) : 0.6666667f, 1f, Mathf.Lerp(0.75f, 0.9f, this.color.y));
            sLeaser.sprites[0].isVisible = false;
        }
        for (int i = 0; i < 4; i++)
        {
            sLeaser.sprites[i].color = color;
        }
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
    
    public Vector2 CircleCenter(int index, float timeStacker)
    {
        return Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
    }
    
    public override bool Edible
    {
        get
        {
            return grabbedBy.Count <= 0 || !(grabbedBy[0].grabber is Player) || (grabbedBy[0].grabber as Player).FoodInStomach != (grabbedBy[0].grabber as Player).MaxFoodInStomach;
        }
    }
    
    public CoralNeuronSystem system;
    
    public Vector2 travelDirection;
    
    public LMOracleSwarmer.Behavior currentBehavior;
    
    private float torque;
    
    private int listBreakPoint;
    
    public Vector2 color;
    
    public System.Collections.Generic.List<Vector2> stuckList;
    
    public int stuckListCounter;
    
    public LMOracleSwarmer.MovementMode mode;
    
    public Mycelium suckleMyc;
    
    public bool attachedToMyc;
    
    public int onlySwarm;
    
    public int dijkstra;
    
    public bool dark;
    
    public enum MovementMode
    {
        Swarm,
        SuckleMycelia,
        FollowDijkstra
    }
    
    public class Behavior
    {
        public Behavior(LMOracleSwarmer leader)
        {
            this.leader = leader;
            dom = UnityEngine.Random.value;
            idealDistance = Mathf.Lerp(10f, 300f, UnityEngine.Random.value * UnityEngine.Random.value);
            life = 1f;
            if (UnityEngine.Random.RandomRange(0f, 1f) > 0.9f)
            {
                life = 0f;
            }
            deathSpeed = 1f / Mathf.Lerp(40f, 220f, UnityEngine.Random.value);
            color = new Vector2((float)UnityEngine.Random.Range(0, 3) / 2f, (UnityEngine.Random.value >= 0.75f) ? 1f : 0f);
            aimInFront = Mathf.Lerp(40f, 300f, UnityEngine.Random.value);
            torque = ((UnityEngine.Random.value >= 0.5f) ? Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) : 0f);
            randomVibrations = UnityEngine.Random.value * UnityEngine.Random.value * UnityEngine.Random.value;
            revolveSpeed = ((UnityEngine.Random.value >= 0.5f) ? 1f : -1f) / Mathf.Lerp(15f, 65f, UnityEngine.Random.value);
            suckle = (UnityEngine.Random.value < 0.166666672f);
        }

        // Token: 0x17000573 RID: 1395
        // (get) Token: 0x060022DE RID: 8926 RVA: 0x00213C7C File Offset: 0x00211E7C
        public float Dominance
        {
            get
            {
                return (!Dead) ? (dom * Mathf.Pow(life, 0.25f)) : -1f;
            }
        }

        // Token: 0x17000574 RID: 1396
        // (get) Token: 0x060022DF RID: 8927 RVA: 0x00213CB0 File Offset: 0x00211EB0
        public bool Dead
        {
            get
            {
                return life <= 0f || leader.slatedForDeletetion || leader.currentBehavior != this;
            }
        }

        // Token: 0x040025D5 RID: 9685
        private float dom;

        // Token: 0x040025D6 RID: 9686
        public float idealDistance;

        // Token: 0x040025D7 RID: 9687
        public float aimInFront;

        // Token: 0x040025D8 RID: 9688
        public float torque;

        // Token: 0x040025D9 RID: 9689
        public float randomVibrations;

        // Token: 0x040025DA RID: 9690
        public float revolveSpeed;

        // Token: 0x040025DB RID: 9691
        public float life;

        // Token: 0x040025DC RID: 9692
        public float deathSpeed;

        // Token: 0x040025DD RID: 9693
        public LMOracleSwarmer leader;

        // Token: 0x040025DE RID: 9694
        public Vector2 color;

        // Token: 0x040025DF RID: 9695
        public bool suckle;
    }
}
