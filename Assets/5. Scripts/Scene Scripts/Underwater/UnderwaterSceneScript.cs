using System;
using Managers;
using UnityEngine;

namespace Scene_Scripts.Underwater
{
    public class UnderwaterSceneScript : MonoBehaviour
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static UnderwaterSceneScript Instance { get; private set; }

        public void Awake()
        {
            if (UnderwaterSceneScript.Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            UIManager.Instance.FadeOut(1f);
        }
    }
}
