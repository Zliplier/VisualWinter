using System;
using System.Collections;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.Command.Database;

namespace Zlipacket.VNZlipacket.Command
{
    public class CMD_VN_Graphic : CMD_DatabaseExtension
    {
        public new static void Extend(CommandDatabase database)
        {
            database.AddCommand("SetLayerMedia", new Func<string[], IEnumerator>(SetLayerMedia));
        }

        public static IEnumerator SetLayerMedia(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0f;
            bool immediate = false;
            string blendTexName = "";
            bool useAudio = false;

            string pathToGraphics = "";
            UnityEngine.Object graphic = null;
            Texture blendTex = null;
            
            yield break;
        }
    }
}