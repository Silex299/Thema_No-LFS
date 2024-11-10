using Thema_Camera;
using UnityEngine;

namespace Player_Scripts.Volumes
{
    [RequireComponent(typeof(WaterVolume))]
    public class WaterVolumeDifferentCameraOffsetExtension : MonoBehaviour
    {
        
        public bool aboveWaterCameraOffset = true;

        public WaterVolume waterVolume;
        public RegionalCameraOffsetInfo[] regionalCameraOffsets;



        public int currentRegionIndex = -1;
        private Vector3 PlayerPosition => PlayerMovementController.Instance.transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Vector3 defaultPos = transform.position;

            defaultPos.y = aboveWaterCameraOffset ? waterVolume.surfaceLevel : 0;

            foreach (var region in regionalCameraOffsets)
            {
                Gizmos.DrawLine(defaultPos + transform.forward * region.zRange.x, defaultPos + transform.forward * region.zRange.y);
            }
            
        }


        private void Update()
        {
            if (!waterVolume.Triggered) return;

            float defaultZ = transform.position.z;
            int i = 0;
            for (i = 0; i <= regionalCameraOffsets.Length; i++)
            {
                if (PlayerPosition.z > defaultZ + regionalCameraOffsets[i].zRange.x && PlayerPosition.z <  defaultZ + regionalCameraOffsets[i].zRange.y)
                {
                    if (i == currentRegionIndex) return;
                    
                    currentRegionIndex = i;
                    ChangeRegionOffset(currentRegionIndex);
                }
            }
            
        }

        private void ChangeRegionOffset(int index)
        {
            if (aboveWaterCameraOffset)
            {
                waterVolume.aboveWaterCameraOffset = regionalCameraOffsets[index].cameraOffset;
                if(waterVolume.PlayerOnSurface) waterVolume.ChangeCameraOffset(true);
            }
            else
            {
                waterVolume.underwaterCameraOffset = regionalCameraOffsets[index].cameraOffset;
                if(!waterVolume.PlayerOnSurface) waterVolume.ChangeCameraOffset(false);
            }
        }


        
        //TODO for checkpoint reset make sure that index is reset to -1;
        


        [System.Serializable]
        public struct RegionalCameraOffsetInfo
        {

            public ChangeOffset cameraOffset;
            public Vector2 zRange;

        }
        
    }
}
