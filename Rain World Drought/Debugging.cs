using System;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace Rain_World_Drought
{
    internal static class Debugging
    {
        public static void Patch()
        {
            On.MainLoopProcess.RawUpdate += MainLoopProcess_RawUpdate;
            On.RainWorld.Start += RainWorld_Start;
            On.RainWorld.Update += RainWorld_Update;
        }

        private static FLabel logs;
        private const int logLength = 10;
        private static Queue<string> logLines;

        private static void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);
            logLines = new Queue<string>();
            logs = new FLabel("font", "");
            logs.anchorX = 0f;
            logs.anchorY = 1f;
            Futile.stage.AddChild(logs);
        }

        private static void RainWorld_Update(On.RainWorld.orig_Update orig, RainWorld self)
        {
            orig(self);
            logs.MoveToFront();
            logs.x = 10.5f;
            logs.y = Futile.screen.height - 10.5f;
        }

        public static void Log(object msg) => Log(msg.ToString());
        public static void Log(string msg)
        {
            string[] lines = msg.Split('\n');
            for(int i = 0; i < lines.Length; i++)
                logLines.Enqueue(lines[i]);
            while (logLines.Count > logLength)
                logLines.Dequeue();
            logs.text = string.Join("\n", logLines.ToArray());
        }

        private static void MainLoopProcess_RawUpdate(On.MainLoopProcess.orig_RawUpdate orig, MainLoopProcess self, float dt)
        {
            self.myTimeStacker += dt * self.framesPerSecond;
            int repsLeft = 10;
            while (self.myTimeStacker > 1f)
            {
                self.Update();
                self.myTimeStacker -= 1f;
                if(repsLeft-- <= 0)
                {
                    self.myTimeStacker = self.myTimeStacker % 1f;
                    break;
                }
            }
            self.GrafUpdate(self.myTimeStacker);
        }
    }
}
