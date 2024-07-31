using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Player_Scripts;
using System;
using System.Collections;
using Managers.Checkpoints;
using Thema_Camera;
using TMPro;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("UI")] private Animator animator;
        [SerializeField, BoxGroup("UI")] private Image fader;


        [BoxGroup("Action")] public Image actionFill;
        [BoxGroup("Action")] public TextMeshProUGUI actionText;
        
        [SerializeField, BoxGroup("Params")] private float fadeTransitionTime;


        [SerializeField, BoxGroup("Player Health"), Space(10)]
        private GameObject healthBar;

        [SerializeField, BoxGroup("Player Health")]
        private Image healthFill;

        [SerializeField, BoxGroup("Player Health")]
        private Image damageImage;


        [BoxGroup("Fields")] public TextMeshProUGUI sceneTitle;


        private bool _isLcpViewOpen;


        private static UIManager _instance;
        public static UIManager Instance => _instance;


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
                UIManager._instance = this;
            }
        }


        private void Start()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;

            controller.player.Health.onDeath += RestartOrExitView;
            controller.player.Health.onRevive += CloseRestartOrExitView;
            controller.player.Health.onTakingDamage += TakeDamage;
        }

        private void OnDisable()
        {
            PlayerMovementController controller = PlayerMovementController.Instance;

            if (controller)
            {
                controller.player.Health.onDeath -= RestartOrExitView;
                controller.player.Health.onRevive -= CloseRestartOrExitView;
                controller.player.Health.onTakingDamage -= TakeDamage;
            }
        }


        private Coroutine _fadingCoroutine;



        #region Action Fill
        
        
        public void UpdateActionFill(float fraction, string text = ">")
        {
            if (Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
            {
                actionFill.gameObject.SetActive(false);
            }
            else
            {
                if (!actionFill.gameObject.activeInHierarchy)
                {
                    actionFill.gameObject.SetActive(true);
                    actionText.text = text;
                }
            }
            
            actionFill.fillAmount = fraction;
        }

        public void UpdateActionFillPos(Vector3 position, Vector3 offset)
        {
            var myCamera = CameraFollow.Instance.myCamera;

            var pos = myCamera.WorldToScreenPoint(position + offset);
            actionFill.rectTransform.position = pos;
        }
        

        #endregion

        #region Fades

        
        public void FadeIn(float transitionTime = 0.2f)
        {
            if (_fadingCoroutine != null)
            {
                StopCoroutine(_fadingCoroutine);
            }

            _fadingCoroutine = StartCoroutine(FadeEnumerator(1, transitionTime));
        }

        private void FadeIn(Action action, float transitionTime = 0.2f)
        {
            {
                if (_fadingCoroutine != null)
                {
                    StopCoroutine(_fadingCoroutine);
                }

                _fadingCoroutine = StartCoroutine(FadeEnumerator(1, transitionTime, action));
            }
        }

        private IEnumerator FadeEnumerator( float endAlpha,float transitionTime = 0.2f, Action action = null)
        {
            float timeElapsed = 0;
            Color faderColor = fader.color;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;

                float alpha = Mathf.Lerp(faderColor.a, endAlpha, timeElapsed / transitionTime);
                fader.color = new Color(faderColor.r, faderColor.g, faderColor.b, alpha);

                yield return null;
            }

            action?.Invoke();
            _fadingCoroutine = null;
        }

        public void FadeOut(float transitionTime = 0.2f)
        {
            if (_fadingCoroutine != null)
            {
                StopCoroutine(_fadingCoroutine);
            }

            _fadingCoroutine = StartCoroutine(FadeEnumerator(0, transitionTime));
        }
        

        #endregion

        #region Menus

        private void RestartOrExitView()
        {
            if (_isLcpViewOpen) return;

            void Action()
            {
                animator.Play("LCP_View");
                _isLcpViewOpen = true;
            }

            FadeIn(Action);
        }

        private void CloseRestartOrExitView()
        {
            if (!_isLcpViewOpen) return;

            _isLcpViewOpen = false;
            animator.Play("EXIT_LCP_VIEW");
            FadeOut();
        }
        
        private void TakeDamage(float fraction)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
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

        #endregion

        #region Button Methods

        public void OnLoadLastCheckpointButtonClicked()
        {
            print("Load Checkpoint");
            CheckpointManager.Instance?.LoadCheckpoint();
        }

        public void LoadLastCheckpointWithTransition()
        {
            
        }

        #endregion
    }
}