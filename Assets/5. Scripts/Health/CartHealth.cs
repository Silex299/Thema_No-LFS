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


        [SerializeField, BoxGroup("Draggable")] private Player_Scripts.Interactables.DraggableCart draggable;

        [SerializeField, BoxGroup("Misc")] private float otherActionDelay;
        [SerializeField, BoxGroup("Misc")] private GameObject explodedItem;

        public override void Kill(string message)
        {
            if (message == "RAY")
            {
                director.Stop();
                cartAnimator.Play("explode cart");
                print("Calling EXPLODE");
            }
        }

        public void Explode()
        {
            explodedItem = Instantiate(fracturedPrefab, fractureSpawnPrefab.transform.position, fractureSpawnPrefab.transform.rotation, transform.parent);
            Invoke(nameof(PlayOtherAction), otherActionDelay);
        }

        public void PlayOtherAction()
        {
            deathAction?.Invoke();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            if (explodedItem)
            {
                Destroy(explodedItem);
            }

            cartAnimator.Play("reset cart");
            transform.localPosition = new Vector3(0.7978142f, 2.018701f, 0.02612144f);
            transform.localRotation = Quaternion.Euler(new Vector3(90f, 269.9998f, 0f));
        }


    }


}
