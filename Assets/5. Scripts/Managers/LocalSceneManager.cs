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

            return default;
        }
        

        public void LoadSubScenes(List<int> requiredScenes, List<int> optionalScenes)
        {
            var (toBeLoaded, toBeUnloaded) = ProcessSceneIndecies(requiredScenes, optionalScenes);
            
            foreach (var load in toBeLoaded)
            {
                LoadSceneAdditively(load);
            }

            foreach (var unload in toBeUnloaded)
            {
                UnloadScene(unload);
            }
        }
        
        private void LoadSceneAdditively(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            }
        }
        private void UnloadScene(int sceneIndex)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex);
            if (scene.isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            }
        }
        
        
    }
}