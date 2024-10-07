using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Misc
{
    public class FractureExplode : MonoBehaviour
    {

        [SerializeField] private Rigidbody[] bodyParts;
        [SerializeField] private bool selfDestruct;

        [SerializeField] private bool fixedDirection;
        [SerializeField, ShowIf("fixedDirection")] private Vector3 explodeDirection;
        
        #region Editor

#if  UNITY_EDITOR
        [Button("Set RB", ButtonSizes.Large), GUIColor(0.3f, 0.9f, 0)]
        public void SetRb()
        {
            bodyParts = GetComponentsInChildren<Rigidbody>();
        }
#endif

        #endregion
        
        public void Explode(Vector3 force)
        {
            foreach (var part in bodyParts)
            {
                part.AddForceAtPosition(force, transform.position, ForceMode.Impulse);
            }
            if (selfDestruct)
            {
                Destroy(gameObject, 5);
            }
        }

    }


}
