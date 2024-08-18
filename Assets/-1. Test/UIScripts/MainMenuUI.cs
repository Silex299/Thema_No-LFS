using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIScripts
{
    public class MainMenuUI : MonoBehaviour
    {
        [BoxGroup("References")] public Animator animator;
        
        [FoldoutGroup("Main Menu")]public MenuButton[] menuButtons;

        [FoldoutGroup("Misc")] public int selectedButtonIndex;

        private Coroutine _lastCoroutine;

        #region Flags

        public bool CanInteract { get; set; } = true;
        public bool CanInteractMainMenu { get; set; } = true;

        #endregion

        private void PrevMenuButton()
        {
            if (_lastCoroutine != null) return;

            int desiredIndex = (selectedButtonIndex - 1) % menuButtons.Length;
            desiredIndex = desiredIndex < 0 ? menuButtons.Length - 1 : desiredIndex;

            while (desiredIndex != selectedButtonIndex)
            {
                if (menuButtons[desiredIndex].Enabled)
                {
                    _lastCoroutine = StartCoroutine(ChangeMenuButton(selectedButtonIndex, desiredIndex));
                    selectedButtonIndex = desiredIndex;
                    break;
                }

                desiredIndex = (desiredIndex - 1) % menuButtons.Length;
                desiredIndex = desiredIndex < 0 ? menuButtons.Length - 1 : desiredIndex;
            }
        }

        private void NextMenuButton()
        {
            if (_lastCoroutine != null) return;

            int desiredIndex = (selectedButtonIndex + 1) % menuButtons.Length;
            while (desiredIndex != selectedButtonIndex)
            {
                if (menuButtons[desiredIndex].Enabled)
                {
                    _lastCoroutine = StartCoroutine(ChangeMenuButton(selectedButtonIndex, desiredIndex));
                    selectedButtonIndex = desiredIndex;
                    break;
                }

                desiredIndex = (desiredIndex + 1) % menuButtons.Length;
            }
        }

        private void Update()
        {
            if (!CanInteract) return;
            
            if (CanInteractMainMenu)
            {
                #region Scroll through the menu buttons

                if (Input.GetAxis("Vertical") > 0)
                {
                    PrevMenuButton();
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    NextMenuButton();
                }

                #endregion
                #region Triggering menu buttons

                if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
                {
                    if (!menuButtons[selectedButtonIndex].Triggered)
                    {
                        menuButtons[selectedButtonIndex].onClick.Invoke();
                        menuButtons[selectedButtonIndex].Triggered = true;
                    }
                }

                #endregion
            }
        }


        private IEnumerator ChangeMenuButton(int lastIndex, int nextIndex)
        {
            yield return menuButtons[lastIndex].SelectButton(false, 0.15f);

            yield return menuButtons[nextIndex].SelectButton(true, 0.15f);

            yield return new WaitForSecondsRealtime(0.3f);

            _lastCoroutine = null;
        }


        [System.Serializable]
        public class MenuButton
        {
            [SerializeField, OnValueChanged(nameof(UpdateButton))]
            private bool enabled;

            [SerializeField, OnValueChanged(nameof(UpdateButton))]
            private bool selected;

            public Image fillImage;
            public TextMeshProUGUI buttonText;

            public Color defaultColor = Color.white;
            public Color selectedColor = Color.black;
            public Color disabledColor = Color.grey;


            public UnityEvent onClick;

            public bool Enabled
            {
                get => enabled;
                set
                {
                    enabled = value;
                    buttonText.color = enabled ? defaultColor : disabledColor;
                }
            }

            public bool Selected
            {
                get => selected;
                set
                {
                    selected = value && Enabled;
                    buttonText.color = selected ? selectedColor : (Enabled ? defaultColor : disabledColor);
                }
            }

            public bool Triggered { get; set; } = false;

            #region Editor

#if UNITY_EDITOR

            private void UpdateButton()
            {
                Enabled = enabled;
                Selected = selected;
            }
#endif

            #endregion

            public IEnumerator SelectButton(bool select, float transitionTime = 1)
            {
                var initFillAmount = fillImage.fillAmount;
                var initColor = buttonText.color;

                var targetFillAmount = select ? 1 : 0;
                var targetColor = select ? selectedColor : defaultColor;


                float requiredTime = Mathf.Abs(targetFillAmount - initFillAmount) * transitionTime;

                float timeElapsed = 0;
                while (timeElapsed < requiredTime)
                {
                    timeElapsed += Time.unscaledDeltaTime;

                    var progress = timeElapsed / requiredTime;
                    fillImage.fillAmount = Mathf.Lerp(initFillAmount, targetFillAmount, progress);
                    buttonText.color = Color.Lerp(initColor, targetColor, progress);
                    yield return null;
                }

                Selected = select;
            }
        }
    }
}