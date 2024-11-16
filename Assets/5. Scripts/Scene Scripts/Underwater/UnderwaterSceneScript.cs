
using System.Collections;
using Managers;
using UnityEngine;

namespace Scene_Scripts.Underwater
{
    public class UnderwaterSceneScript : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            UIManager.Instance.FadeOut(2f);
            print("Hello");
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
