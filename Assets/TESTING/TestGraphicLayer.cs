using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zlipacket.VNZlipacket.GraphicPanel;

namespace TESTING
{
    public class TestGraphicLayer : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Running());
        }

        private IEnumerator Running()
        {
            GraphicPanel panel = GraphicPanelsManager.Instance.GetPanel("Background");
            GraphicLayer layer = panel.GetLayer(0, true);
            
            yield return new WaitForSeconds(1f);
            
            Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/hurricane");
            
            layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", blendingTexture: blendTex);
            
            yield return new WaitForSeconds(1f);

            layer.currentGraphic.FadeOut();
        }
    }
}