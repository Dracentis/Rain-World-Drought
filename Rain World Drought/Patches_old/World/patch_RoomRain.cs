using System;
using MonoMod;
using System.Collections.Generic;
using CoralBrain;
using Noise;
using RWCustom;
using ScavTradeInstruction;
using UnityEngine;
using VoidSea;

class patch_RoomRain : RoomRain
{

    [MonoModIgnore]
    private int visibilitySetter;

    [MonoModIgnore]
    patch_RoomRain(GlobalRain globalRain, Room rm) : base(globalRain, rm) { }

    public extern void orig_Update(bool eu);

    public void Update(bool eu)
    {
        float initialWaterLevel = -100f;
        if (this.room.waterObject != null)
        {
            initialWaterLevel = this.room.waterObject.fWaterLevel;
        }
        this.evenUpdate = eu;
        //----------------

        if (this.dangerType == RoomRain.DangerType.Rain || this.dangerType == RoomRain.DangerType.FloodAndRain)
		{
			this.intensity = Mathf.Lerp(this.intensity, this.globalRain.Intensity, 0.2f);
		}
		this.intensity = Mathf.Min(this.intensity, this.room.roomSettings.RainIntensity);
		this.visibilitySetter = 0;
		if (this.intensity == 0f && this.lastIntensity > 0f)
		{
			this.visibilitySetter = -1;
		}
		else if (this.intensity > 0f && this.lastIntensity == 0f)
		{
			this.visibilitySetter = 1;
		}
		this.lastIntensity = this.intensity;
		if (this.globalRain.AnyPushAround)
		{
			this.ThrowAroundObjects();
		}
		if (this.bulletDrips.Count < (int)((float)this.room.TileWidth * this.globalRain.bulletRainDensity * this.room.roomSettings.RainIntensity))
		{
			this.bulletDrips.Add(new BulletDrip(this));
			this.room.AddObject(this.bulletDrips[this.bulletDrips.Count - 1]);
		}
		else if (this.bulletDrips.Count > (int)((float)this.room.TileWidth * this.globalRain.bulletRainDensity * this.room.roomSettings.RainIntensity))
		{
			this.bulletDrips[0].Destroy();
			this.bulletDrips.RemoveAt(0);
		}
		if (this.globalRain.flood > 0f)
		{
			if (this.room.waterObject != null)
			{
				this.room.waterObject.fWaterLevel = Mathf.Lerp(this.room.waterObject.fWaterLevel, this.FloodLevel, 0.2f);
				this.room.waterObject.GeneralUpsetSurface(Mathf.InverseLerp(0f, 0.5f, this.globalRain.Intensity) * 4f);
			}
			else if (this.room.roomSettings.DangerType == RoomRain.DangerType.Flood || this.room.roomSettings.DangerType == RoomRain.DangerType.FloodAndRain)
			{
				this.room.AddWater();
			}
		}
        if (this.room.roomSettings.RainIntensity == 0.01086957f){
		    if (this.dangerType != RoomRain.DangerType.Flood)
		    {
		    	this.normalRainSound.Volume = ((this.intensity <= 0f) ? 0f : (0.1f + 0.9f * Mathf.Pow(Mathf.Clamp01(Mathf.Sin(Mathf.InverseLerp(0.001f, .7f, this.intensity*40) * 3.14159274f)), 1.5f)));
		    	this.normalRainSound.Update();
		    	this.heavyRainSound.Volume = Mathf.Pow(Mathf.InverseLerp(0.12f, 0.5f, this.intensity*10), 0.85f) * Mathf.Pow(1f - this.deathRainSound.Volume, 0.3f);
		    	this.heavyRainSound.Update();
		    }
		    this.deathRainSound.Volume = Mathf.Pow(Mathf.InverseLerp(0.35f, 0.75f, this.intensity*20), 0.8f);
		    this.deathRainSound.Update();
		    this.rumbleSound.Volume = this.globalRain.RumbleSound * this.room.roomSettings.RumbleIntensity;
		    this.rumbleSound.Update();
		    this.distantDeathRainSound.Volume = Mathf.InverseLerp(1400f, 0f, (float)this.room.world.rainCycle.TimeUntilRain) * this.room.roomSettings.RainIntensity;
		    this.distantDeathRainSound.Update();
		    if (this.dangerType != RoomRain.DangerType.Rain)
		    {
		    	this.floodingSound.Volume = Mathf.InverseLerp(0.01f, 0.5f, this.globalRain.floodSpeed);
		    	this.floodingSound.Update();
		    }
		    if (this.room.game.cameras[0].room == this.room)
		    {
		    	this.SCREENSHAKESOUND.Volume = this.room.game.cameras[0].ScreenShake * (1f - this.rumbleSound.Volume);
		    }
		    else
		    {
		    	this.SCREENSHAKESOUND.Volume = 0f;
		    }
		    this.SCREENSHAKESOUND.Update();
        }else{
		    if (this.dangerType != RoomRain.DangerType.Flood)
		    {
		    	this.normalRainSound.Volume = ((this.intensity <= 0f) ? 0f : (0.1f + 0.9f * Mathf.Pow(Mathf.Clamp01(Mathf.Sin(Mathf.InverseLerp(0.001f, 0.7f, this.intensity) * 3.14159274f)), 1.5f)));
		    	this.normalRainSound.Update();
		    	this.heavyRainSound.Volume = Mathf.Pow(Mathf.InverseLerp(0.12f, 0.5f, this.intensity), 0.85f) * Mathf.Pow(1f - this.deathRainSound.Volume, 0.3f);
		    	this.heavyRainSound.Update();
		    }
		    this.deathRainSound.Volume = Mathf.Pow(Mathf.InverseLerp(0.35f, 0.75f, this.intensity), 0.8f);
		    this.deathRainSound.Update();
		    this.rumbleSound.Volume = this.globalRain.RumbleSound * this.room.roomSettings.RumbleIntensity;
		    this.rumbleSound.Update();
		    this.distantDeathRainSound.Volume = Mathf.InverseLerp(1400f, 0f, (float)this.room.world.rainCycle.TimeUntilRain) * this.room.roomSettings.RainIntensity;
		    this.distantDeathRainSound.Update();
		    if (this.dangerType != RoomRain.DangerType.Rain)
		    {
		    	this.floodingSound.Volume = Mathf.InverseLerp(0.01f, 0.5f, this.globalRain.floodSpeed);
		    	this.floodingSound.Update();
		    }
		    if (this.room.game.cameras[0].room == this.room)
		    {
		    	this.SCREENSHAKESOUND.Volume = this.room.game.cameras[0].ScreenShake * (1f - this.rumbleSound.Volume);
		    }
		    else
		    {
		    	this.SCREENSHAKESOUND.Volume = 0f;
		    }
		    this.SCREENSHAKESOUND.Update();
        }

        //---------------
        if (this.globalRain.flood != 0f)
        {
            if (this.room.waterObject != null)
            {
                if (this.room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Pulse) > 0f || this.room.roomSettings.GetEffectAmount((RoomSettings.RoomEffect.Type)patch_RoomSettings.patch_RoomEffect.Type.Drain) > 0f)
                {
                    this.room.waterObject.fWaterLevel = initialWaterLevel;
                }
                else
                {
                    this.room.waterObject.fWaterLevel = Mathf.Lerp(this.room.waterObject.fWaterLevel, this.FloodLevel, 0.2f);
                }
                this.room.waterObject.GeneralUpsetSurface(Mathf.InverseLerp(0f, 0.5f, this.globalRain.Intensity) * 4f);
            }
            else if (this.room.roomSettings.DangerType == RoomRain.DangerType.Flood || this.room.roomSettings.DangerType == RoomRain.DangerType.FloodAndRain)
            {
                this.room.AddWater();
            }
        }
        if (room.roomSettings.RainIntensity == 0.01086957f)
        {
            if (dangerType == DangerType.Rain || dangerType == DangerType.FloodAndRain)
            {
                intensity = Mathf.Lerp(intensity, globalRain.Intensity, 0.2f);
            }
            intensity = Mathf.Min(intensity, 0.17f);
            visibilitySetter = 0;
            if (intensity == 0f && lastIntensity > 0f)
            {
                visibilitySetter = -1;
            }
            else if (intensity > 0f && lastIntensity == 0f)
            {
                visibilitySetter = 1;
            }
            lastIntensity = intensity;
            distantDeathRainSound.Volume = Mathf.InverseLerp(1400f, 0f, (float)room.world.rainCycle.TimeUntilRain) * 0.17f;
        }
    }

    private void ThrowAroundObjects()
    {
        if (room.roomSettings.RainIntensity == 0f || room.roomSettings.RainIntensity <= 0.01086957f)
        {
            return;
        }
        for (int i = 0; i < room.physicalObjects.Length; i++)
        {
            for (int j = 0; j < room.physicalObjects[i].Count; j++)
            {
                for (int k = 0; k < room.physicalObjects[i][j].bodyChunks.Length; k++)
                {
                    BodyChunk bodyChunk = room.physicalObjects[i][j].bodyChunks[k];
                    IntVector2 tilePosition = room.GetTilePosition(bodyChunk.pos + new Vector2(Mathf.Lerp(-bodyChunk.rad, bodyChunk.rad, UnityEngine.Random.value), Mathf.Lerp(-bodyChunk.rad, bodyChunk.rad, UnityEngine.Random.value)));
                    float num = InsidePushAround;
                    bool flag = false;
                    if (rainReach[Custom.IntClamp(tilePosition.x, 0, room.TileWidth - 1)] < tilePosition.y)
                    {
                        flag = true;
                        num = Mathf.Max(OutsidePushAround, InsidePushAround);
                    }
                    if (room.water)
                    {
                        num *= Mathf.InverseLerp(room.FloatWaterLevel(bodyChunk.pos.x) - 100f, room.FloatWaterLevel(bodyChunk.pos.x), bodyChunk.pos.y);
                    }
                    if (num > 0f)
                    {
                        if (bodyChunk.ContactPoint.y < 0)
                        {
                            int num2 = 0;
                            if (rainReach[Custom.IntClamp(tilePosition.x - 1, 0, room.TileWidth - 1)] >= tilePosition.y && !room.GetTile(tilePosition + new IntVector2(-1, 0)).Solid)
                            {
                                num2--;
                            }
                            if (rainReach[Custom.IntClamp(tilePosition.x + 1, 0, room.TileWidth - 1)] >= tilePosition.y && !room.GetTile(tilePosition + new IntVector2(1, 0)).Solid)
                            {
                                num2++;
                            }
                            bodyChunk.vel += Custom.DegToVec(Mathf.Lerp(-30f, 30f, UnityEngine.Random.value) + (float)(num2 * 16)) * UnityEngine.Random.value * ((!flag) ? 4f : 9f) * num / bodyChunk.mass;
                        }
                        else
                        {
                            BodyChunk bodyChunk2 = bodyChunk;
                            bodyChunk2.vel.y = bodyChunk2.vel.y - Mathf.Pow(UnityEngine.Random.value, 5f) * 16.5f * num / bodyChunk.mass;
                        }
                        if (bodyChunk.owner is Creature)
                        {
                            if (Mathf.Pow(UnityEngine.Random.value, 1.2f) * 2f * (float)bodyChunk.owner.bodyChunks.Length < num)
                            {
                                (bodyChunk.owner as Creature).Stun(UnityEngine.Random.Range(1, 1 + (int)(9f * num)));
                            }
                            if (bodyChunk == (bodyChunk.owner as Creature).mainBodyChunk)
                            {
                                (bodyChunk.owner as Creature).rainDeath += num / 20f;
                            }
                            if (num > 0.5f && (bodyChunk.owner as Creature).rainDeath > 1f && UnityEngine.Random.value < 0.025f)
                            {
                                (bodyChunk.owner as Creature).Die();
                            }
                        }
                        bodyChunk.vel += Custom.DegToVec(Mathf.Lerp(90f, 270f, UnityEngine.Random.value)) * UnityEngine.Random.value * 5f * InsidePushAround;
                    }
                }
            }
        }
    }


}

