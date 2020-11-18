using System;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace Rain_World_Drought
{
    internal static class Debugging
    {
        private static FLabel logs;
        private const int logLength = 25;
        private static Queue<string> logLines;
        private static int ctr;

        public static void Patch()
        {
            //On.MainLoopProcess.RawUpdate += MainLoopProcess_RawUpdate;
            On.RainWorld.Start += RainWorld_Start;
            On.RainWorld.Update += RainWorld_Update;
            On.FFont.GetQuadInfoForText += FFont_GetQuadInfoForText;
        }

        // Create log display
        private static void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);
            logLines = new Queue<string>();
            logs = new FLabel("font", "");
            logs.anchorX = 0f;
            logs.anchorY = 1f;
            Futile.stage.AddChild(logs);
        }

        // Move log display
        private static void RainWorld_Update(On.RainWorld.orig_Update orig, RainWorld self)
        {
            orig(self);
            logs.MoveToFront();
            logs.x = 10.5f;
            logs.y = Futile.screen.height - 10.5f;
        }

        public static void Log(object msg) => Log(msg.ToString());
        private static int dump = 0;
        public static void Log(string msg)
        {
            try
            {
                string[] lines = msg.Split('\n');
                lines[0] = string.Concat("[", ctr.ToString("D3"), "] ", lines[0]);
                ctr++;
                if (ctr > 999) ctr = 0;
                for (int i = 0; i < lines.Length; i++)
                    logLines.Enqueue(lines[i]);
                while (logLines.Count > logLength)
                    logLines.Dequeue();
                logs.text = string.Join(Environment.NewLine, logLines.ToArray());
            }
            catch (Exception e) {
                Debug.LogException(new Exception($"Failed to log message: {msg}", e));
                Debug.Log($"Generated text dump: dmp{dump}.txt");
                System.IO.File.WriteAllText($"dmp{dump}.txt", string.Join(Environment.NewLine, logLines.ToArray()));
                dump++;
            }
        }
        
        // Remove limit of one update per frame
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

        // Remove the limit of 15 lines per text box
        // I would like to personally slap MattRix
        private static List<FLetterQuadLine> lines = new List<FLetterQuadLine>();
        private static FLetterQuadLine[] FFont_GetQuadInfoForText(On.FFont.orig_GetQuadInfoForText orig, FFont self, string text, FTextParams labelTextParams)
        {
            int lineCount = 0;
            int charCount = 0;
            char[] array = text.ToCharArray();
            lines.Clear();
            int num3 = array.Length;
            for (int i = 0; i < num3; i++)
            {
                char c = array[i];
                if (c == '\n')
                {
                    lines.Add(new FLetterQuadLine()
                    {
                        letterCount = charCount,
                        quads = new FLetterQuad[charCount]
                    });
                    lineCount++;
                    charCount = 0;
                }
                else
                {
                    charCount++;
                }
            }
            lines.Add(new FLetterQuadLine()
            {
                letterCount = charCount,
                quads = new FLetterQuad[charCount]
            });
            lineCount = 0;
            charCount = 0;
            float num4 = 0f;
            float num5 = 0f;
            char c2 = '\0';
            float num6 = float.MaxValue;
            float num7 = float.MinValue;
            float num8 = float.MaxValue;
            float num9 = float.MinValue;
            float num10 = self._lineHeight + labelTextParams.scaledLineHeightOffset + self._textParams.scaledLineHeightOffset;
            for (int k = 0; k < num3; k++)
            {
                char c3 = array[k];
                if (c3 == '\n')
                {
                    FLetterQuadLine line = lines[lineCount];
                    if (charCount == 0)
                        line.bounds = new Rect(0f, 0f, num5, num5 - num10);
                    else
                        line.bounds = new Rect(num6, num8, num7 - num6, num9 - num8);
                    lines[lineCount] = line;
                    num6 = float.MaxValue;
                    num7 = float.MinValue;
                    num8 = float.MaxValue;
                    num9 = float.MinValue;
                    num4 = 0f;
                    num5 -= num10;
                    lineCount++;
                    charCount = 0;
                }
                else
                {
                    FKerningInfo fkerningInfo = self._nullKerning;
                    for (int l = 0; l < self._kerningCount; l++)
                    {
                        FKerningInfo fkerningInfo2 = self._kerningInfos[l];
                        if (fkerningInfo2.first == c2 && fkerningInfo2.second == c3)
                        {
                            fkerningInfo = fkerningInfo2;
                        }
                    }
                    FLetterQuad fletterQuad = default(FLetterQuad);
                    FCharInfo fcharInfo;
                    if (self._charInfosByID.ContainsKey(c3))
                    {
                        fcharInfo = self._charInfosByID[c3];
                    }
                    else
                    {
                        fcharInfo = self._charInfosByID[0u];
                    }
                    float num11 = fkerningInfo.amount + labelTextParams.scaledKerningOffset + self._textParams.scaledKerningOffset;
                    if (charCount == 0)
                    {
                        num4 = -fcharInfo.offsetX;
                    }
                    else
                    {
                        num4 += num11;
                    }
                    fletterQuad.charInfo = fcharInfo;
                    Rect rect = new Rect(num4 + fcharInfo.offsetX, num5 - fcharInfo.offsetY - fcharInfo.height, fcharInfo.width, fcharInfo.height);
                    fletterQuad.rect = rect;
                    lines[lineCount].quads[charCount] = fletterQuad;
                    num6 = Math.Min(num6, rect.xMin);
                    num7 = Math.Max(num7, rect.xMax);
                    num8 = Math.Min(num8, num5 - num10);
                    num9 = Math.Max(num9, num5);
                    num4 += fcharInfo.xadvance;
                    charCount++;
                }
                c2 = c3;
            }
            {
                FLetterQuadLine line = lines[lineCount];
                if (charCount == 0)
                    line.bounds = new Rect(0f, 0f, num5, num5 - num10);
                else
                    line.bounds = new Rect(num6, num8, num7 - num6, num9 - num8);
                lines[lineCount] = line;
            }
            return lines.ToArray();
        }
    }
}
