using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        private int _sceneToLoadIndex = -1;
        private AsyncOperation _loadingOperation;

        // Function to load a scene in the background using the scene index
        public void LoadSceneInBackground(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
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
            _loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
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
    }
}