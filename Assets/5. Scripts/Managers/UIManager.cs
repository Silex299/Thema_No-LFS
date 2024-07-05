using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Player_Scripts;
using System;
using Managers.Checkpoints;
using TMPro;

namespace Managers
{

    public class UIManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("UI")] private Animator animator;
        [SerializeField, BoxGroup("UI")] private Image fader;

        [SerializeField, BoxGroup("Params")] private float fadeTransitionTime;


        [SerializeField, BoxGroup("Player Health"), Space(10)] private GameObject healthBar;
        [SerializeField, BoxGroup("Player Health")] private Image healthFill;
        [SerializeField, BoxGroup("Player Health")] private Image damageImage;


        [BoxGroup("Fields")] public TextMeshProUGUI sceneTitle;
        
        
        private bool _isFadingIn;
        private bool _isFadingOut;
        private float _fadeTimeElapsed;
        private bool isLCPViewOpen;


        private static UIManager instance;
        public static UIManager Instance => instance;


        private void Awake()
        {
            if (UIManager.Instance)
            {
                if (UIManager.Instance != this)
                {
                    Destroy(this);
                }
            }
            else
            {
                UIManager.instance = this;
            }
        }


        private void Start()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;

            controller.player.Health.OnDeath += RestartLastCheckPointView;
            controller.player.Health.OnRevive += CloseLastCheckpointView;
            controller.player.Health.OnTakingDamage += TakeDamage;
        }

        private void OnDisable()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;

            if (controller)
            {
                controller.player.Health.OnDeath -= RestartLastCheckPointView;
                controller.player.Health.OnRevive -= CloseLastCheckpointView;
                controller.player.Health.OnTakingDamage -= TakeDamage;
            }
        }


        private void Update()
        {
            if (_isFadingIn) FadeIn();
            if (_isFadingOut) FadeOut();

        }

        private Action FadeInCallback;
        public void FadeIn()
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

        public void FadeOut()
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
            if (isLCPViewOpen) return;

            FadeInCallback = () =>
            {
                animator.Play("LCP_View");
                isLCPViewOpen = true;
            };

            FadeIn();
        }

        private void CloseLastCheckpointView()
        {
            if (!isLCPViewOpen) return;

            isLCPViewOpen = false;
            animator.Play("EXIT_LCP_VIEW");
            FadeOut();
        }


        private void TakeDamage(float fraction)
        {
            if (fraction == 1)
            {
                healthBar.SetActive(false);
            }
            else
            {
                if (!healthBar.activeInHierarchy)
                {
                    healthBar.SetActive(true);
                }
                healthFill.fillAmount = fraction;
            }

            Color color = damageImage.color;
            color.a = 1 - fraction;

            damageImage.color = color;

        }

        #region Button Methods


        public void OnLoadLastCheckpointButtonClicked()
        {
            print("Load Checkpoint");
            CheckpointManager.Instance?.LoadCheckpoint();
        }

        #endregion

    }

}
