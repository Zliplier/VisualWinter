using Unity.Cinemachine;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.Misc
{
    public class ChangeCameraTarget : MonoBehaviour
    {
        private CinemachineCamera cinemachineCam;

        private void Start()
        {
            cinemachineCam = GetComponent<CinemachineCamera>();
        }

        public void ChangeCamera(CinemachineCamera targetCamera)
        {
            cinemachineCam.Priority--;
            targetCamera.Priority++;
        }
    }
}