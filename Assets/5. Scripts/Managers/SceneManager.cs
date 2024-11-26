
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Managers.Checkpoints;
using Scriptable;
using Thema;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneManager : MonoBehaviour
    {

        public CheckpointManager.CheckpointInfo savedCheckpointInfo;
        public SceneData sceneData;
        
        private static string SavePath => Path.Combine(Application.persistentDataPath, "Checkpoint.data");
        public static SceneManager Instance { get; private set; }
        public bool AllLoaded { get; private set; }

        private void Awake()
        {
            if (SceneManager.Instance == null)
            {
                SceneManager.Instance = this;
            }
            else if (SceneManager.Instance != this)
            {
                Destroy(SceneManager.Instance.gameObject);
            }
            
            LoadCheckpointData();
            DontDestroyOnLoad(gameObject);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void LoadCheckpointData()
        {
            if (File.Exists(SavePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(SavePath, FileMode.Open);
                savedCheckpointInfo = (CheckpointManager.CheckpointInfo)formatter.Deserialize(stream);

            }
        }
        
        public void SaveCheckpointData(CheckpointManager.CheckpointInfo newCheckpointInfo)
        {
            savedCheckpointInfo= newCheckpointInfo;
            BinaryFormatter formatter = new BinaryFormatter();

            using FileStream stream = new FileStream(SavePath, FileMode.Create);
            formatter.Serialize(stream, savedCheckpointInfo);
        }

        public bool CanSaveCheckpoint(CheckpointManager.CheckpointInfo info)
        {
            
            if(info.level > savedCheckpointInfo.level) return true;
            if (info.checkpoint > savedCheckpointInfo.checkpoint) return true;

            return false;
        }



        public void LoadFromSavedData()
        {
            if (sceneData.sceneCheckpointData.TryGetValue(savedCheckpointInfo.level, out var value))
            {
                int[] requiredSubScenes =  value[savedCheckpointInfo.checkpoint].requiredSubScenes;
                LoadMainAndSubScenesInBackground(savedCheckpointInfo.level, requiredSubScenes);
            }
            
        }
        public void LoadNewGame()
        {
            savedCheckpointInfo = new CheckpointManager.CheckpointInfo() { level = 1, checkpoint = 0 }; //THIS IS DEFAULT VALUE
            SaveCheckpointData(savedCheckpointInfo);
            LoadFromSavedData();
            
        }

        public void LoadNewLevel(int levelBuildIndex)
        {
            CheckpointManager.CheckpointInfo newData = new CheckpointManager.CheckpointInfo() { level = levelBuildIndex, checkpoint = 0 }; //THIS IS DEFAULT VALUE
            SaveCheckpointData(newData);
            LoadFromSavedData();
        }

        public void LoadMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            Destroy(this.gameObject);
        }
     
        
        /*--  LOAD SCENE --*/
        
        
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
        
        
        // New function to load main scene and sub-scenes in the background
        private void LoadMainAndSubScenesInBackground(int mainScene, int[] subScenes)
        {
            if (mainScene < 0 || mainScene >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                return;
            }

            foreach (int subSceneIndex in subScenes)
            {
                if (subSceneIndex < 0 || subSceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                {
                    return;
                }
            }

            StartCoroutine(LoadMainAndSubScenesAsync(mainScene, subScenes));
        }

        // Coroutine to load main scene and sub-scenes asynchronously
        private IEnumerator LoadMainAndSubScenesAsync(int mainScene, int[] subScenes)
        {

            AllLoaded = false;
            
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

            AllLoaded = true;

        }

        
        
        [ContextMenu("Load")]
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
        }


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
