using System;
using System.Collections;
using Managers;
using Managers.Checkpoints;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace Scene_Scripts
{
    public class GuardEncounter : MonoBehaviour
    {
        public float initialSceneNameDelay = 9f;
        public string sceneName;
        public PlayableDirector initSceneTimeline;


        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoad;
        }

        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoad;
        }


        private void OnCheckpointLoad(int checkpoint)
        {
            if (checkpoint == 0)
            {
                initSceneTimeline.Play();
                StartCoroutine(StarScene());
            }
            else
            {
                UIManager.Instance.FadeOut(3);
            }
        }

        private IEnumerator StarScene()
        {
            yield return new WaitForSeconds(initialSceneNameDelay);

            //Set Scene name
            var sceneTitle = UIManager.Instance.sceneTitle;
            sceneTitle.text = sceneName;

            //fade ina

            //Set scene title text to scene name, with one letter at a time using for 
            //loop and yield return new WaitForSeconds(0.1f)
            for (int i = 0; i < sceneName.Length; i++)
            {
                sceneTitle.text = sceneName.Substring(0, i + 1);
                yield return new WaitForSeconds(0.03f);
            }


            //wait 
            yield return new WaitForSeconds(5f);

            for (int j = sceneName.Length; j >= 0; j--)
            {
                sceneTitle.text = sceneName.Substring(0, j);
                yield return new WaitForSeconds(0.03f);
            }
        }
        

        public void FadeOutPpVolume(Volume renderingVolume)
        {
            StartCoroutine(FadeOutPpEnumerator(renderingVolume));
        }

        private IEnumerator FadeOutPpEnumerator(Volume renderingVolume)
        {
            if (renderingVolume.weight < 1)
                yield break;

            float timeElapsed = 0;
            while (timeElapsed<6)
            {
                timeElapsed += Time.deltaTime;
                
                renderingVolume.weight = Mathf.Lerp(1, 0, timeElapsed / 6);
                yield return null;
            }

            renderingVolume.gameObject.SetActive(false);
            
            yield return null;
        }

        public void Exit()
        {
            PlayerMovementController.Instance.DisablePlayerMovement(true);
            StartCoroutine(ExitScene());
        }
        private IEnumerator ExitScene()
        {
            UIManager.Instance.FadeIn(0.5f);
            
            //Set Scene name
            var sceneTitle = UIManager.Instance.sceneTitle;
            string title =  "DEMO: SKIPPING LEVELS";
            sceneTitle.text = "";

            //fade ina

            //Set scene title text to scene name, with one letter at a time using for 
            //loop and yield return new WaitForSeconds(0.1f)
            for (int i = 0; i < sceneName.Length; i++)
            {
                sceneTitle.text = title.Substring(0, i + 1);
                yield return new WaitForSeconds(0.01f);
            }

        }


    }
}