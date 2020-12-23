using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUD;

namespace Rain_World_Drought.OverWorld
{
    internal static class DialogBoxHK
    {
        private static Dictionary<DialogBox, TextData> textData = new Dictionary<DialogBox, TextData>();

        public static void Patch()
        {
            On.HUD.DialogBox.Draw += new On.HUD.DialogBox.hook_Draw(DrawHK);
            On.HUD.DialogBox.ClearSprites += new On.HUD.DialogBox.hook_ClearSprites(ClearSpritesHK);
        }

        private static void ClearSpritesHK(On.HUD.DialogBox.orig_ClearSprites orig, DialogBox self)
        {
            textData.Remove(self);
            orig(self);
        }

        private static void DrawHK(On.HUD.DialogBox.orig_Draw orig, DialogBox self, float timeStacker)
        {
            if(self.CurrentMessage == null)
            {
                orig(self, timeStacker);
                return;
            }

            // Calculate average character width of the longest line
            if (!textData.TryGetValue(self, out TextData data) || data.message != self.CurrentMessage)
            {
                data = new TextData();
                data.message = self.CurrentMessage;
                string oldText = self.label.text;
                self.label.text = data.message.text;
                data.meanCharWidth = self.label.textRect.width / data.message.longestLine;
                self.label.text = oldText;
                textData[self] = data;
            }
            float oldMeanCharWidth = DialogBox.meanCharWidth;
            DialogBox.meanCharWidth = data.meanCharWidth;
            orig(self, timeStacker);
            DialogBox.meanCharWidth = oldMeanCharWidth;
        }

        private class TextData
        {
            public DialogBox.Message message;
            public float meanCharWidth;
        }
    }
}
