using Sirenix.OdinInspector;
using UnityEngine;


namespace Weapons
{
    public class Sword : MonoBehaviour
    {


        [SerializeField, BoxGroup("refrences")] private Animator animator;

        [SerializeField, BoxGroup("Weapon Properties")] private bool isActive = true;

        public void ActivateSword(bool active)
        {
            if (active == isActive) return;

            animator.Play(active ? "Activate" : "Deactivate");
            isActive = active;

        }


        private void Start()
        {
            ActivateSword(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError(other.name);
        }


    }
}