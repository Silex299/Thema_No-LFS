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


        public bool IsDead { get; private set; }
        private bool _dissolveDeath;


        private static readonly int Dissolve1 = Shader.PropertyToID("_Dissolve");

        //public Action onDeath;
        public Action onRevive;
        public Action<float> onTakingDamage;
        private float _dissolveTimeElapsed = 0;
        private static readonly int VerticalAcceleration = Animator.StringToHash("VerticalAcceleration");

        protected override void Start()
        {
            dissolveMaterial.SetFloat(Dissolve1, -1);
            currentHealth = maximumHealth;
            IsDead = false;
            _dissolveDeath = false;
        }

        private void Update()
        {
            if (_dissolveDeath)
            {
                DissolveDeath();
            }
        }

        public void TakeDamage(float damage, string deathOverrideString)
        {
            if (currentHealth < 0) return;

            currentHealth -= damage;

            onTakingDamage?.Invoke(currentHealth / maximumHealth);

            if (currentHealth <= 0)
            {
                Kill(deathOverrideString);
            }

        }

        public override void TakeDamage(float damage)
        {
            if (currentHealth < 0) return;

            currentHealth -= damage;

            onTakingDamage?.Invoke(currentHealth / maximumHealth);

            if (currentHealth <= 0)
            {
                Kill("RAGDOLL DEATH");
            }

        }

        public override void ResetHealth()
        {
            base.ResetHealth();
            onTakingDamage?.Invoke(1);
        }


        protected override void Death(string message = "")
        { 
            base.Death(message);
            player.EffectsManager.PlayPlayerSound("Moan");
            IsDead = true;
        }


        public override void Kill(string message)
        {
            if (message == "RAY")
            {
                DisableComponents();
                PlayerMovementController.Instance.PlayAnimation("Float Death", 0.5f, 1); //Play Death  Animation
            }
            else if (message == "RAGDOLL DEATH")
            {
                player.AnimationController.enabled = false;
                //player.EffectsManager.PlayOtherSounds("Death", 1);
                DisableComponents();
                Death();
            }
            else if (message == "FALL")
            {
                player.AnimationController.enabled = false;
                //player.EffectsManager.PlayOtherSounds("Fall Death", 1);
                DisableComponents();
                Death();
            }
            else
            {
                DisableComponents();
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


                float value = Mathf.Lerp(-1, 1, fraction);


                dissolveMaterial.SetFloat(Dissolve1, value);

                if (!IsDead && fraction >= 0.5f)
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


        public void DisableComponents()
        {
            player.CController.enabled = false;
            player.MovementController.enabled = false;
            player.EffectsManager.enabled = false;
        }


        //TODO: SOME STUPID TWO RESET PLAYER FIX IT
        public void ResetPlayer()
        {

            player.CController.enabled = true;
            player.MovementController.enabled = true;
            player.EffectsManager.enabled = true;
            player.AnimationController.enabled = true;
            player.AnimationController.SetFloat(VerticalAcceleration, 0);
            onRevive?.Invoke();
            Start();
        }

        public void PlayRagdoll()
        {
            player.CController.enabled = false;
            player.AnimationController.enabled = false;
            player.MovementController.enabled = false;
            player.EffectsManager.enabled = false;
        }

    }

}