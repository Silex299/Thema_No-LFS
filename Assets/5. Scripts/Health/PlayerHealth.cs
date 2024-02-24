using Player_Scripts;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Health
{

    public class PlayerHealth : HealthBaseClass
    {

        [SerializeField, FoldoutGroup("Components")] private Player player;

        [SerializeField, BoxGroup("Player Health")] private Material dissolveMaterial;
        [SerializeField, BoxGroup("Player Health")] private float dissolveTime = 3;


        private bool _isDead;
        private bool _dissolveDeath;


        private static readonly int Dissolve1 = Shader.PropertyToID("_Dissolve");

        public Action OnDeath;
        public Action OnRevive;
        private float _dissolveTimeElapsed = 0;

        private void Start()
        {
            dissolveMaterial.SetFloat(Dissolve1, -1);
            _isDead = false;
            _dissolveDeath = false;
        }

        private void Update()
        {
            if (_dissolveDeath)
            {
                DissolveDeath();
            }
        }


        protected override void Death(string message = "")
        {
            base.Death(message);
            _isDead = true;
            OnDeath.Invoke();
        }


        public override void Kill(string message)
        {
            if (message == "RAY")
            {
                DisableCompoents();
                PlayerMovementController.Instance.PlayAnimation("Float Death", 0.5f, 1); //Play Death  Animation
            }
            else
            {
                DisableCompoents();
                Death(message);
                PlayerMovementController.Instance.PlayAnimation(message, 0.2f, 1); //Play Death  Animation
            }
        }


        public void DissolveDeath()
        {
            if (!_dissolveDeath)
            {
                _dissolveDeath = true;
                _dissolveTimeElapsed = 0;
            }
            else
            {

                _dissolveTimeElapsed += Time.deltaTime;
                float fraction = _dissolveTimeElapsed / dissolveTime;


                float value =  Mathf.Lerp(-1, 1, fraction);


                dissolveMaterial.SetFloat(Dissolve1, value);

                if (!_isDead && fraction >= 0.5f)
                {
                    Death();
                }

                if (fraction >= 1)
                {
                    _dissolveDeath = false;
                    return;
                }
            }
        }


        public void DisableCompoents()
        {
            player.CController.enabled = false;
            player.MovementController.enabled = false;
            player.EffectsManager.enabled = false;
        }


        public void Reset()
        {

            player.CController.enabled = true;
            player.MovementController.enabled = true;
            player.EffectsManager.enabled = true;
            player.AnimationController.enabled = true;

            OnRevive.Invoke();
            Start();
        }

    }

}