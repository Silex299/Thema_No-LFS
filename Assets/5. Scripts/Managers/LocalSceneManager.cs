using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LocalSceneManager : MonoBehaviour
    {
        
        public static LocalSceneManager Instance { get; private set; }
        public bool AllSceneLoaded { get; private set; } = true;

        private void OnEnable()
        {
            if (LocalSceneManager.Instance == null)
            {
                LocalSceneManager.Instance = this;
            }
            else if (LocalSceneManager.Instance != this)
            {
                Destroy(LocalSceneManager.Instance);
                LocalSceneManager.Instance = this;
            }
        }

        [Button]
        private List<int> GetAllLoadedScenes()
        {
            int mainSceneIndex = SceneManager.Instance.savedCheckpointInfo.level;

            List<int> loadedSceneIndices = new List<int>();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if(scene.buildIndex == mainSceneIndex) continue;
                if (scene.isLoaded)
                {
                    loadedSceneIndices.Add(scene.buildIndex);
                }
            }

            return loadedSceneIndices;
        }

        private (List<int>, List<int>) ProcessSceneIndecies(List<int> requiredScenes, List<int> optionalScenes)
        {
            
            List<int> loadedScenes = GetAllLoadedScenes();
            
            List<int> toBeLoaded = requiredScenes.Where(sceneIndex => !loadedScenes.Contains(sceneIndex)).ToList();
            List<int> toBeUnloaded = loadedScenes.Where(sceneIndex => !requiredScenes.Contains(sceneIndex) && !optionalScenes.Contains(sceneIndex)).ToList();


            //Update Loaded List
            foreach (var load in toBeLoaded)
            {
                loadedScenes.Add(load);
            }

            foreach (var unload in toBeUnloaded)
            {
                loadedScenes.Remove(unload);
            }

            return (toBeLoaded, toBeUnloaded);
        }
        
        public void LoadSubScenes(List<int> requiredScenes, List<int> optionalScenes)
        {
            var (toBeLoaded, toBeUnloaded) = ProcessSceneIndecies(requiredScenes, optionalScenes);


            StartCoroutine(LoadScene(toBeLoaded));

            foreach (var unload in toBeUnloaded)
            {
                UnloadScene(unload);
            }
        }

        private IEnumerator LoadScene(List<int> scenes)
        {
            AllSceneLoaded = false;
            // Start loading sub-scenes as additive
            AsyncOperation[] subSceneLoadingOperations = new AsyncOperation[scenes.Count];
            for (int i = 0; i < scenes.Count; i++)
            {
                subSceneLoadingOperations[i] = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
            }


            yield return new WaitUntil(() => subSceneLoadingOperations.All(scene => scene.isDone));
            AllSceneLoaded = true;

        }
        
        private void UnloadScene(int sceneIndex)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);
            if (scene.isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            }
        }
        
        
        
        
        
        public void LoadFromSavedData()
        {
            StartCoroutine(LoadFromSaveDataIEnumerator());
        }
        
        private  IEnumerator LoadFromSaveDataIEnumerator()
        {
            SceneManager.Instance.LoadFromSavedData();

            yield return new WaitForSeconds(1.5f);
            
            SceneManager.Instance.ActivateLoadedMainAndSubScenes();
        }

        
        public void LoadNextScene(int index)
        {
            SceneManager.Instance.LoadNewLevel(index);
        }
        
        public void ActivateNextScene()
        {
            SceneManager.Instance.ActivateLoadedMainAndSubScenes();
        }

        public void LoadMainMenu()
        {
            SceneManager.Instance.LoadMainMenu();
        }
        
        

        
        
    }
}