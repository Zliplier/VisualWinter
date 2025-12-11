using UnityEngine;
using Zlipacket.CoreZlipacket.Player.Input;

namespace Zlipacket.CoreZlipacket.Tools
{
    public static class ZlipUtilities
    {
        public static bool CastMouseCickRaycast( PlayerInputController playerInputController, out RaycastHit raycastHit)
        {
            raycastHit = new RaycastHit();
            
            Vector3 sceneMousePositionNear = new Vector3(
                playerInputController.MousePosition.x,
                playerInputController.MousePosition.y,
                Camera.main.nearClipPlane);
            Vector3 sceneMousePositionFar = new Vector3(
                playerInputController.MousePosition.x,
                playerInputController.MousePosition.y,
                Camera.main.farClipPlane);
            
            Vector3 worldMousePositionNear = Camera.main.ScreenToWorldPoint(sceneMousePositionNear);
            Vector3 worldMousePositionFar = Camera.main.ScreenToWorldPoint(sceneMousePositionFar);

            //Debug.DrawRay(worldMousePositionNear, worldMousePositionFar - worldMousePositionNear, Color.green, 1f);
            if (Physics.Raycast(worldMousePositionNear, worldMousePositionFar - worldMousePositionNear, out RaycastHit hit, float.PositiveInfinity))
            {
                raycastHit = hit;
                return true;
            }
            
            return false;
        }
        
        public static bool ApproximatelyWithMargin(float a, float b, float margin)
        {
            return Mathf.Abs(a - b) < margin;
        }
    }
}