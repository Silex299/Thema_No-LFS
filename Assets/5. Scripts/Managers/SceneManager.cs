using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneManager : MonoBehaviour
    {
        private int _sceneToLoadIndex = -1;
        private AsyncOperation _loadingOperation;
        private AsyncOperation[] _subSceneLoadingOperations;

        // Function to load a scene in the background using the scene index
        public void LoadSceneInBackground(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                StartCoroutine(LoadSceneAsync(sceneIndex));
            }
            else
            {
                Debug.LogError("Invalid scene index");
            }
        }

        // Coroutine to handle the loading process asynchronously
        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            _loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            _loadingOperation.allowSceneActivation = false;
            _sceneToLoadIndex = sceneIndex;

            while (!_loadingOperation.isDone)
            {
                yield return null;
            }
        }

        // Function to change to the loaded scene when called
        public void ActivateLoadedScene()
        {
            if (_loadingOperation != null && _sceneToLoadIndex >= 0)
            {
                _loadingOperation.allowSceneActivation = true;
            }
            else
            {
                Debug.LogError("No scene loaded to activate");
            }
        }


        public void TestSceneLoad()
        {
            int mainScene = 2;
            int[] subScenes = new int[] { 3, 4, 5 };

            LoadMainAndSubScenesInBackground(mainScene, subScenes);
        }
        
        
        
        // New function to load main scene and sub-scenes in the background
        public void LoadMainAndSubScenesInBackground(int mainScene, int[] subScenes)
        {
            if (mainScene < 0 || mainScene >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError("Invalid main scene index");
                return;
            }

            foreach (int subSceneIndex in subScenes)
            {
                if (subSceneIndex < 0 || subSceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                {
                    Debug.LogError("Invalid sub-scene index: " + subSceneIndex);
                    return;
                }
            }

            StartCoroutine(LoadMainAndSubScenesAsync(mainScene, subScenes));
        }

        // Coroutine to load main scene and sub-scenes asynchronously
        private IEnumerator LoadMainAndSubScenesAsync(int mainScene, int[] subScenes)
        {
            _loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(mainScene);
            _loadingOperation.allowSceneActivation = false;
            _sceneToLoadIndex = mainScene;

            // Start loading sub-scenes as additive
            _subSceneLoadingOperations = new AsyncOperation[subScenes.Length];
            for (int i = 0; i < subScenes.Length; i++)
            {
                _subSceneLoadingOperations[i] = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(subScenes[i], LoadSceneMode.Additive);
                _subSceneLoadingOperations[i].allowSceneActivation = false;
            }

            // Wait until all scenes are loaded
            while (!_loadingOperation.isDone || !AllSubScenesLoaded())
            {
                yield return null;
            }
        }

        // Function to activate all scenes when ready
        public void ActivateLoadedMainAndSubScenes()
        {
            if (_loadingOperation != null && _sceneToLoadIndex >= 0)
            {
                _loadingOperation.allowSceneActivation = true;
                foreach (var subSceneOperation in _subSceneLoadingOperations)
                {
                    if (subSceneOperation != null)
                    {
                        subSceneOperation.allowSceneActivation = true;
                    }
                }
            }
            else
            {
                Debug.LogError("No scenes loaded to activate");
            }
        }

        // Helper function to check if all sub-scenes are loaded
        private bool AllSubScenesLoaded()
        {
            foreach (var subSceneOperation in _subSceneLoadingOperations)
            {
                if (subSceneOperation != null && !subSceneOperation.isDone)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
