using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Interactable_Items
{
    public class EmergencyFlare : MonoBehaviour
    {

        public Transform handSocket;

        [SerializeField]private GameObject flareLight;


        private readonly static int flare = Animator.StringToHash("Flare");
        private GameObject _flareInstance;


        private bool _isPickedUp;

        public void PickUpFlare()
        {
            if (_flareInstance) return;
            StartCoroutine(PickUp());
        }
        private IEnumerator PickUp()
        {
            _isPickedUp = true;
            
            Player_Scripts.PlayerController.Instance.CrossFadeAnimation("pick up", 0.5f);
            
            yield return new WaitForSeconds(2f);

            _flareInstance = Instantiate(flareLight);
            _flareInstance.transform.parent = handSocket;
            _flareInstance.transform.localPosition = Vector3.zero;
            _flareInstance.transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(2.5f);
            //LightUp The flair
            Player_Scripts.PlayerController.Instance.Player.AnimationController.SetBool(flare, true);
            _flareInstance.GetComponentInChildren<VisualEffect>().Play();
            _flareInstance.GetComponentInChildren<Light>().intensity = 1f;



            yield return null;
        }

        public void ThrowFlare()
        {
            if (!_isPickedUp) return;
            StartCoroutine(Throw());
        }

        private IEnumerator Throw()
        {
            _isPickedUp = false;
            _flareInstance.GetComponentInChildren<VisualEffect>().Stop();
            _flareInstance.GetComponentInChildren<Light>().intensity = 0f;


            yield return new WaitForSeconds(1f);

            Player_Scripts.PlayerController.Instance.Player.AnimationController.SetBool(flare, false);

            yield return new WaitForSeconds(1f);
            Destroy(_flareInstance);
        }


    }

}

