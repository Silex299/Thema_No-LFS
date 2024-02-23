using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Player_Scripts;
using System;

namespace Managers
{

    public class UIManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("UI")] private Animator animator;
        [SerializeField, BoxGroup("UI")] private Image fader;

        [SerializeField, BoxGroup("Params")] private float fadeTransitionTime;



        private bool _isFadingIn;
        private bool _isFadingOut;
        private float _fadeTimeElapsed;


        private void Start()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;
            controller.player.health.OnDeath += RestartLastCheckPointView;
        }

        private void OnDisable()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;
            controller.player.health.OnDeath -= RestartLastCheckPointView;
        }


        private void Update()
        {
            if (_isFadingIn) FadeIn();
            if (_isFadingOut) FadeOut();
        }

        private Action FadeInCallback;
        private void FadeIn()
        {
            if (!_isFadingIn)
            {
                _isFadingIn = true;
                _fadeTimeElapsed = 0;
            }
            else
            {
                _fadeTimeElapsed += Time.deltaTime;
                float fraction = _fadeTimeElapsed / fadeTransitionTime;

                float a = Mathf.Lerp(0, 1, fraction);
                Color color = fader.color;
                color.a = a;

                fader.color = color;

                if (fraction >= 1)
                {
                    _isFadingIn = false;
                    FadeInCallback?.Invoke();
                    FadeInCallback = null;
                }

            }
        }

        private void FadeOut()
        {
            if (!_isFadingOut)
            {
                _isFadingOut = true;
                _fadeTimeElapsed = 0;
            }
            else
            {
                _fadeTimeElapsed += Time.deltaTime;
                float fraction = _fadeTimeElapsed / fadeTransitionTime;

                float a = Mathf.Lerp(255, 0, fraction);
                Color color = fader.color;
                color.a = a;

                fader.color = color;

                if (fraction >= 1)
                {
                    _isFadingOut = false;
                }

            }
        }


        private void RestartLastCheckPointView()
        {
            FadeInCallback = () =>
            {

                animator.Play("LCP_View");
            };
            FadeIn();
        }
    }

}