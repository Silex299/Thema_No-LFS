using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{

    public class FractureExplode : MonoBehaviour
    {

        [SerializeField] private float force;
        [SerializeField] private Rigidbody[] bodyParts;

        [Button("Set RB", ButtonSizes.Large), GUIColor(0.3f, 0.9f, 0)]
        public void SetRb()
        {
            bodyParts = GetComponentsInChildren<Rigidbody>();
        }




        private void Start()
        {
            Explode();
        }

        private void Explode()
        {
            foreach (var part in bodyParts)
            {
                var direction = new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1));
                part.AddForceAtPosition(force * direction, transform.position, ForceMode.Impulse);
            }

        }
        


    }


}
