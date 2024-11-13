using System;
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
        [FoldoutGroup("Main Menu")] public MenuButton[] menuButtons;
        [FoldoutGroup("Episodes")] public Episode[] episodes;
        [FoldoutGroup("Chapter Placeholders")] public Chapter[] chapters;

        [FoldoutGroup("Misc")] public int selectedButtonIndex;
        [FoldoutGroup("Misc")] public int selectedEpisodeIndex;
        [FoldoutGroup("Misc")] public int selectChapterIndex;
        [FoldoutGroup("Misc")] public MenuState menuState;
        
        
        private Coroutine _lastCoroutine;
        private Coroutine _mainMenuUpdateCoroutine;
        private Coroutine _episodeMenuUpdateCoroutine;
        private Coroutine _chapterMenuUpdateCoroutine;

        #region Flags

        public bool CanInteract { get; set; } = true;

        #endregion
        
        private void Start()
        {
            menuState = MenuState.Main;
            _mainMenuUpdateCoroutine ??= StartCoroutine(MainMenuUpdate());
        }
        
        private IEnumerator MainMenuUpdate(bool start = false)
        {
            yield return new WaitForSeconds(1f);
            while (menuState == MenuState.Main)
            {
                if (!CanInteract)
                { 
                    break;
                }
                
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
                
                yield return null;
            }
            #region Exit Main Menu
            
            if (menuState == MenuState.Episodes)
            {
                animator.Play("Open Episode Menu");
            }
            foreach (var button in menuButtons)
            {
                button.Triggered = false;
            }
            _mainMenuUpdateCoroutine = null;

            #endregion
            
            #region Main Menu Methods

            void PrevMenuButton()
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
            void NextMenuButton()
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
            IEnumerator ChangeMenuButton(int lastIndex, int nextIndex)
            {
                yield return menuButtons[lastIndex].SelectButton(false, 0.15f);

                yield return menuButtons[nextIndex].SelectButton(true, 0.15f);

                yield return new WaitForSecondsRealtime(0.3f);

                _lastCoroutine = null;
            }

            #endregion
            
        }
        private IEnumerator EpisodeMenuUpdate()
        {
            yield return new WaitForSeconds(1f);
            while (menuState == MenuState.Episodes)
            {
                #region Scroll through the episodes

                if (Input.GetAxis("Horizontal") > 0)
                {
                    NextEpisode();
                }
                else if (Input.GetAxis("Horizontal") < 0)
                {
                    PrevEpisode();
                }

                #endregion
                #region Triggering episodes

                if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
                {
                    ChangeMenu(MenuState.Chapters);
                }

                #endregion

                #region Exit Episode Menu
                
                if (Input.GetButtonDown("esc"))
                {
                    ChangeMenu(MenuState.Main);
                }

                #endregion
                
                yield return null;
            }
            #region Exit
            
            if (menuState == MenuState.Main)
            {
                animator.Play("Close Episode Menu");
            }
            else if (menuState == MenuState.Chapters)
            {
                animator.Play("Opening Chapters");
            }
            _episodeMenuUpdateCoroutine = null;

            #endregion
            
            #region Methods

            void NextEpisode()
            {
                if (_lastCoroutine != null) return;

                int desiredIndex = (selectedEpisodeIndex + 1) % episodes.Length;
                while (desiredIndex != selectedEpisodeIndex)
                {
                    if (!episodes[desiredIndex].Locked)
                    {
                        _lastCoroutine = StartCoroutine(ChangeEpisode(selectedEpisodeIndex, desiredIndex));
                        selectedEpisodeIndex = desiredIndex;
                        break;
                    }

                    desiredIndex = (desiredIndex + 1) % episodes.Length;
                }
            }
            void PrevEpisode()
            {
                if (_lastCoroutine != null) return;

                int desiredIndex = (selectedEpisodeIndex - 1) % episodes.Length;
                desiredIndex = desiredIndex < 0 ? episodes.Length - 1 : desiredIndex;

                while (desiredIndex != selectedEpisodeIndex)
                {
                    if (!episodes[desiredIndex].Locked)
                    {
                        _lastCoroutine = StartCoroutine(ChangeEpisode(selectedEpisodeIndex, desiredIndex));
                        selectedEpisodeIndex = desiredIndex;
                        break;
                    }

                    desiredIndex = (desiredIndex - 1) % episodes.Length;
                    desiredIndex = desiredIndex < 0 ? episodes.Length - 1 : desiredIndex;
                }
            }
            IEnumerator ChangeEpisode(int lastIndex, int nextIndex)
            {
                yield return episodes[lastIndex].CycleEpisode(false, 0.15f);
                yield return episodes[nextIndex].CycleEpisode(true, 0.15f);
                yield return new WaitForSecondsRealtime(0.3f);
                _lastCoroutine = null;
            }
            
            #endregion
        }
        private IEnumerator ChapterMenuUpdate()
        { 
            InitSetup();
            yield return new WaitForSeconds(1f);
            while (menuState == MenuState.Chapters)
            {
                #region Scroll through the chapters

                if (Input.GetAxis("Horizontal") > 0)
                {
                    NextChapter();
                }
                else if (Input.GetAxis("Horizontal") < 0)
                {
                    PrevChapter();
                }

                #endregion
                #region Triggering chapters

                if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit"))
                {
                    if (!chapters[selectChapterIndex].locked)
                    {
                        chapters[selectChapterIndex].onClick.Invoke(selectedEpisodeIndex, 
                            episodes[selectedEpisodeIndex].chapterData[selectChapterIndex].checkpoint);
                    }
                    
                    //LOAD ANIMATION HERE
                }

                #endregion
                
                #region Exit Chapter Menu
                
                if (Input.GetButtonDown("esc"))
                {
                    ChangeMenu(MenuState.Episodes);
                }
                #endregion
                yield return null;
            }
            
            #region exit
            if (menuState == MenuState.Episodes)
            {
                animator.Play("Close Chapters");
            }

            _chapterMenuUpdateCoroutine = null;
            #endregion
            
            #region Methods

            void InitSetup()
            {
                //Set up the chapters;
                int chapterDataCount = episodes[selectedEpisodeIndex].chapterData.Length;
                for(int i = 0; i< chapters.Length; i++)
                {
                    if (i == 0)
                    {
                        chapters[i].Selected = true;
                        selectChapterIndex = 0;
                    }
                
                    if(i<chapterDataCount)
                    {
                        chapters[i].chapterName.text = episodes[selectedEpisodeIndex].chapterData[i].name;
                        chapters[i].Locked = false;
                    }
                    else
                    {   
                        chapters[i].Locked = true;
                    }
                }
            }
            void NextChapter()
            {
                if (_lastCoroutine != null) return;

                int desiredIndex = (selectChapterIndex + 1) % chapters.Length;
                while (desiredIndex != selectChapterIndex)
                {
                    if (!chapters[desiredIndex].Locked)
                    {
                        _lastCoroutine = StartCoroutine(ChangeChapter(selectChapterIndex, desiredIndex));
                        selectChapterIndex = desiredIndex;
                        break;
                    }

                    desiredIndex = (desiredIndex + 1) % chapters.Length;
                }
            }
            void PrevChapter()
            {
                if (_lastCoroutine != null) return;

                int desiredIndex = (selectChapterIndex - 1) % chapters.Length;
                desiredIndex = desiredIndex < 0 ? chapters.Length - 1 : desiredIndex;

                while (desiredIndex != selectChapterIndex)
                {
                    if (!chapters[desiredIndex].Locked)
                    {
                        _lastCoroutine = StartCoroutine(ChangeChapter(selectChapterIndex, desiredIndex));
                        selectChapterIndex = desiredIndex;
                        break;
                    }

                    desiredIndex = (desiredIndex - 1) % chapters.Length;
                    desiredIndex = desiredIndex < 0 ? chapters.Length - 1 : desiredIndex;
                }
            }
            IEnumerator ChangeChapter(int lastIndex, int nextIndex)
            {
                yield return chapters[lastIndex].CycleEpisode(false, 0.15f);
                yield return chapters[nextIndex].CycleEpisode(true, 0.15f);
                yield return new WaitForSecondsRealtime(0.3f);
                _lastCoroutine = null;
            }

            #endregion
            
        }
        

        public void ChangeMenu(int index)
        {
            ChangeMenu((MenuState) index);
        }
        private void ChangeMenu(MenuState newMenu)
        {
            print(newMenu);
            if(newMenu == menuState) return;
            menuState = newMenu;

            switch (menuState)
            {
                case MenuState.Main:
                    _mainMenuUpdateCoroutine ??= StartCoroutine(MainMenuUpdate());
                    break;
                
                case MenuState.Episodes:
                    _episodeMenuUpdateCoroutine ??= StartCoroutine(EpisodeMenuUpdate());
                    break;
                
                case MenuState.Chapters:
                    _chapterMenuUpdateCoroutine ??= StartCoroutine(ChapterMenuUpdate());
                    break;
            }

        }
        
        
        
        [Serializable]
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

        [Serializable]
        public class Episode
        {
            #region editor

#if UNITY_EDITOR
            public GameObject episode;
            [Button]
            public void GetEpisode()
            {
                var images = episode.GetComponentsInChildren<Image>(true);
                foreach (var image in images)
                {
                    if (image.name.Contains("Gradient"))
                    {
                        selectionImage = image;
                        selected = (selectionImage.color.a > 0);
                    }

                    if (image.name.Contains("Locked"))
                    {
                        lockedImage = image.gameObject;
                        locked = lockedImage.activeInHierarchy;
                    }
                }
            }
#endif

            #endregion

            public bool locked;
            public bool selected;
            public ChapterData[] chapterData;
            
            public Image selectionImage;
            public GameObject lockedImage;
            public bool Locked
            {
                get => locked;
                set
                {
                    locked = value;
                    lockedImage.SetActive(locked);
                }
            }
            public bool Selected
            {
                get => selected;
                set
                {
                    selected = value;
                    var color = selectionImage.color;
                    color.a = selected ? 0.25f : 0;
                    selectionImage.color = color;
                }
            }
            
            public virtual IEnumerator CycleEpisode(bool select, float transitionTime = 1)
            {
                var initColor = selectionImage.color;
                Color targetColor = initColor;
                targetColor.a = select ? 0.25f : 0;

                float timeElapsed = 0;
                while (timeElapsed < transitionTime)
                {
                    timeElapsed += Time.unscaledDeltaTime;
                    var progress = timeElapsed / transitionTime;
                    selectionImage.color = Color.Lerp(initColor, targetColor, progress);
                    yield return null;
                }

                Selected = select;
            }
            
        }
        
        [Serializable]
        public class Chapter
        {
            #region editor

#if UNITY_EDITOR
            public GameObject episode;
            [Button]
            public void GetEpisode()
            {
                var transforms = episode.GetComponentsInChildren<Transform>(true);
                foreach (var trans in transforms)
                {
                    if (trans.name.Contains("Gradient"))
                    {
                        selectionImage = trans.GetComponent<Image>();
                        selected = (selectionImage.color.a > 0);
                    }

                    if (trans.name.Contains("Locked"))
                    {
                        lockedImage = trans.gameObject;
                        locked = lockedImage.activeInHierarchy;
                    }

                    if (trans.name.Contains("Title"))
                    {
                        chapterName = trans.GetComponent<TextMeshProUGUI>();
                    }
                }
            }
#endif

            #endregion

            public bool locked;
            public bool selected;

            public Image selectionImage;
            public GameObject lockedImage;
            
            public TextMeshProUGUI chapterName;

            public UnityEvent<int, int> onClick;
            
            public bool Locked
            {
                get => locked;
                set
                {
                    locked = value;
                    lockedImage.SetActive(locked);
                }
            }
            public bool Selected
            {
                get => selected;
                set
                {
                    selected = value;
                    var color = selectionImage.color;
                    color.a = selected ? 0.25f : 0;
                    selectionImage.color = color;
                }
            }
            
            public virtual IEnumerator CycleEpisode(bool select, float transitionTime = 1)
            {
                var initColor = selectionImage.color;
                Color targetColor = initColor;
                targetColor.a = select ? 0.25f : 0;

                float timeElapsed = 0;
                while (timeElapsed < transitionTime)
                {
                    timeElapsed += Time.unscaledDeltaTime;
                    var progress = timeElapsed / transitionTime;
                    selectionImage.color = Color.Lerp(initColor, targetColor, progress);
                    yield return null;
                }

                Selected = select;
            }
            
        }
        
        
        
        [Serializable]
        public struct ChapterData
        {
            //ADD IMAGES TOO
            public string name;
            public int checkpoint;
        }
        public enum MenuState
        {
            Main,
            Episodes,
            Chapters,
            Settings,
        }
        
    }
}