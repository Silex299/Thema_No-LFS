
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
    }
}
