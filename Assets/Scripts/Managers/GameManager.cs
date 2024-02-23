using System;
using Player_Scripts;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {

        #region Variables

        public static GameManager instance;

        public event Action OnGameOver;

        #endregion

        #region Builtin methods
        private void Awake()
        {
            if (!GameManager.instance)
            {
                GameManager.instance = this;
            }
            else if (GameManager.instance != this)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            PlayerController.Instance.Player.Health.PlayerDeath += GameOver;
        }
        private void OnDisable()
        {
            PlayerController.Instance.Player.Health.PlayerDeath -= GameOver;
        }

        #endregion

        #region Custom Methods

        private void GameOver()
        {
            OnGameOver?.Invoke();
        }

        public void ChangeScene()
        {

            Debug.Log("Changing Scene");

        }



        #endregion


    }
}
