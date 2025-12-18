using System;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.VNZlipacket.GraphicPanel
{
    public class GraphicPanelsManager : Singleton<GraphicPanelsManager>
    {
        public const float DEFAULT_TRANSITION_SPEED = 3f;
        
        [SerializeField] private GraphicPanel[] allPanels;

        public GraphicPanel GetPanel(string panelName)
        {
            foreach (var panel in allPanels)
            {
                if (string.Equals(panel.panelName.ToLower(), panelName.ToLower(), StringComparison.CurrentCultureIgnoreCase))
                    return panel;
            }
            
            return null;
        }
    }
}