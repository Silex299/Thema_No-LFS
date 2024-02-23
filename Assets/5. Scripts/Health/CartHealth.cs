using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace Health
{

    public class CartHealth : HealthBaseClass
    {

        [SerializeField, BoxGroup("Cart Health")] private Animator cartAnimator;
        [SerializeField, BoxGroup("Cart Health")] private PlayableDirector director;

        [SerializeField, BoxGroup("Cart Health")] private GameObject fracturedPrefab;
        [SerializeField, BoxGroup("Cart Health")] private Transform fractureSpawnPrefab;

        [SerializeField, BoxGroup("Misc")] private float otherActionDelay;

        public override void Kill(string message)
        {
            if(message == "RAY")
            {
                director.Stop();
                cartAnimator.Play("explode cart");
                print("Calling EXPLODE");
            }
        }

        public void Explode()
        {
            Instantiate(fracturedPrefab, fractureSpawnPrefab.transform.position, fractureSpawnPrefab.transform.rotation);
            Invoke(nameof(PlayOtherAction), otherActionDelay);
        }

        public void PlayOtherAction()
        {
            deathAction?.Invoke();
            Destroy(this.gameObject);
        }


    }


}
