using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class FractureParent : MonoBehaviour
    {

        [SceneObjectsOnly] public GameObject mainObject;
        [AssetsOnly] public GameObject fracturedObject;
        public Vector3 forceDirection = new Vector3(0, 1, 0);
        public float destroyAfter = 5;
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, forceDirection);
        }
        
        [Button]
        public void Fracture()
        {
            mainObject.SetActive(false);
            //instantiates the fractured object and get fractureExplode component
            var fractured = Instantiate(fracturedObject, mainObject.transform.position, mainObject.transform.rotation);
            var fractureExplode = fractured.GetComponent<FractureExplode>();
            //call explode in fracture explode 
            fractureExplode.Explode(forceDirection);

            if (destroyAfter > 0)
            {
                Destroy(fractured, destroyAfter);
            }
            
        }
        public void Reset()
        {
            mainObject.SetActive(true);
        }
        
    }
}
