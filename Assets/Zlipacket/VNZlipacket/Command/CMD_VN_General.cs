using System;
using System.Collections;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.Command.Database;

namespace Zlipacket.VNZlipacket.Command
{
    public class CMD_VN_General : CMD_DatabaseExtension
    {
        public new static void Extend(CommandDatabase database)
        {
            database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));
        }

        public static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }
    }
}