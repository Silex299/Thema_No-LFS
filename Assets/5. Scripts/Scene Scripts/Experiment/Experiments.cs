using System.Collections;
using Managers;
using Managers.Checkpoints;
using UnityEngine;

namespace Scene_Scripts
{
    public class Experiments : MonoBehaviour
    {
        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoad;
            StartCoroutine(UiUpdateOnCheckpointLoad());
        }

        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoad;
        }
        
        private void OnCheckpointLoad(int checkpoint)
        {
            if (checkpoint >= 0)
            {
                StartCoroutine(UiUpdateOnCheckpointLoad());
            }
        }

        private IEnumerator UiUpdateOnCheckpointLoad()
        {
            yield return new WaitForSeconds(2.5f);
            UIManager.Instance.FadeOut(1f);
        }
        
        
        //TOOD: REMOVE THESE
        public void Exit()
        {
            StartCoroutine(ExitScene());
        }
        
        private IEnumerator ExitScene()
        {
            UIManager.Instance.Interactable = false;
            //Set Scene name
            var sceneTitle = UIManager.Instance.sceneTitle;
            string title =  "DEMO: SKIPPING LEVELS";
            sceneTitle.text = "";

            //fade ina

            //Set scene title text to scene name, with one letter at a time using for 
            //loop and yield return new WaitForSeconds(0.1f)
            for (int i = 0; i < title.Length; i++)
            {
                sceneTitle.text = title.Substring(0, i + 1);
                yield return new WaitForSeconds(0.1f);
            }

        }
        
        
    }
}
