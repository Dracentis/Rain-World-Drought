using System;
using MonoMod;
using UnityEngine;
using RWCustom;

    public class patch_SaveState : SaveState
    {
        [MonoModIgnore]
        public patch_SaveState(int saveStateNumber, PlayerProgression progression) : base(saveStateNumber, progression)
        {
        }
    
        public extern void orig_LoadGame(string str, RainWorldGame game);
    
        public new void LoadGame(string str, RainWorldGame game)
        {
            orig_LoadGame(str, game);
            if (denPosition.Equals("SU_C04"))
            {
                Debug.Log("New Save File Created");
                denPosition = "FS_A01";
                this.deathPersistentSaveData.theMark = true;
            }
        }

        public void BringStomachUpToDate(RainWorldGame game)
        {
            bool flag = false;
            for (int i = 0; i < game.session.Players.Count; i++)
            {
                if ((game.session.Players[i].realizedCreature as Player).objectInStomach != null)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                this.swallowedItems = new string[game.session.Players.Count];
                for (int j = 0; j < game.session.Players.Count; j++)
                {
                    if ((game.session.Players[j].realizedCreature as Player).objectInStomach != null)
                    {
                        if ((game.session.Players[j].realizedCreature as Player).objectInStomach is AbstractCreature)
                        {
                            AbstractCreature abstractCreature = (game.session.Players[j].realizedCreature as Player).objectInStomach as AbstractCreature;
                            if (game.world.GetAbstractRoom(abstractCreature.pos.room) == null)
                            {
                                abstractCreature.pos = (game.session.Players[j].realizedCreature as Player).coord;
                            }
                            this.swallowedItems[j] = SaveState.AbstractCreatureToString(abstractCreature);
                        }
                        else
                        {
                            this.swallowedItems[j] = (game.session.Players[j].realizedCreature as Player).objectInStomach.ToString();
                        }
                    }
                    else
                    {
                        this.swallowedItems[j] = "0";
                    }
                }
            }
            else
            {
                this.swallowedItems = null;
            }
        }
    }

