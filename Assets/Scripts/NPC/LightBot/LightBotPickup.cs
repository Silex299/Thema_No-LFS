using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPC.LightBot
{
    public class LightBotPickup : MonoBehaviour
    {
        [SerializeField, BoxGroup] private float delay;
        [SerializeField, BoxGroup, SceneObjectsOnly] private Transform instantiateLocation;
        [SerializeField, AssetsOnly, BoxGroup] private GameObject prefab;

        public void OnTrigger()
        {
            StartCoroutine(IEOnTrigger());
        }

        private IEnumerator IEOnTrigger()
        {
            yield return new WaitForSeconds(delay);
            Instantiate(prefab,
                PlayerController.Instance.Player.AnimationController.GetBoneTransform(HumanBodyBones.RightHand)
                    .position, Quaternion.identity);
        }

    }
}
