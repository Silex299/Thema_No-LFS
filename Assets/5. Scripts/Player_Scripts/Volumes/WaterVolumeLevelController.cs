using UnityEngine;


namespace Player_Scripts.Volumes
{

    public class WaterVolumeLevelController : MonoBehaviour
    {

        [SerializeField] private WaterVolume waterVolume;
        [SerializeField] private Transform waterSurface;
        [SerializeField] private float surfaceDistanceOffset;

        public bool checkChanges;
        public float distance;


        private void Update()
        {
            if (!checkChanges) return;


            distance = waterSurface.transform.position.y - waterVolume.transform.position.y;
            waterVolume.surfaceThreshold = Mathf.Clamp(distance - surfaceDistanceOffset, 0, 100);





        }

    }


}