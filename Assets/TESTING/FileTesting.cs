using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.IO;

namespace TESTING
{
    public class FileTesting : MonoBehaviour
    {
        string fileName = "Testing.txt";
        string assetName = "Testing";

        private void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            //List<string> lines = FileManager.ReadTextFile(fileName, false);
            List<string> lines = FileManager.ReadTextAsset(assetName, false);
            
            foreach (string line in lines)
                Debug.Log(line);
            
            yield return null;
        }
    }
}
