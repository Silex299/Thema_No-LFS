using System.Collections;
using UnityEngine;


namespace Misc.Items
{

    public class FloatingOnRay : ReactiveInteractable
    {

        [SerializeField] private Animator floaterAnimator;
        [SerializeField] private float delay;
        [SerializeField] private float dropDelay = 1;

        [SerializeField, Space(10)] private AudioSource source;
        [SerializeField] private AudioClip floatSound;
        [SerializeField] private AudioClip dropSound;


        private bool _isFloating;
        private Coroutine _resetInteractor;
        public override void Interact(bool status)
        {
            if (status && !_isFloating)
            {
                _isFloating = true;
                Invoke("PlayFloat", delay);
            }

            if (_resetInteractor != null)
            {
                StopCoroutine(_resetInteractor);
            }

            _resetInteractor = StartCoroutine(ResetInteractor());

        }

        
        private IEnumerator ResetInteractor()
        {
            yield return new WaitForSeconds(0.1f);

            _isFloating = false;
            Invoke("Drop", dropDelay);
        }

        private void PlayFloat()
        {
            floaterAnimator.Play("Float");
            Invoke(nameof(PlayFloatSound), 0.3f);
        }

        private void PlayFloatSound()
        {
            if (source)
            {
                if (floatSound)
                {
                    source.PlayOneShot(floatSound, 1f);
                }
            }
        }

        private void Drop()
        {
            floaterAnimator.CrossFade("drop", 0.06f);
            Invoke(nameof(PlayDropSound), 0.1f);
        }


        private void PlayDropSound()
        {
            if (source)
            {
                if (dropSound)
                {
                    source.PlayOneShot(dropSound, 0.4f);
                }
            }
        }

    }
}
