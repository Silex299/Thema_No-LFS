using System;
using System.Collections;
using Health;
using Managers;
using MyCamera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts
{
    public class PlayerHealth : HealthBase
    {

        [SerializeField, BoxGroup("References")] private PlayerController player;

        [SerializeField, BoxGroup("Misc")] private float effectDelay = 1f;
        [SerializeField, BoxGroup("Misc")] private float dissolveSpeed = 0.5f;


        [SerializeField, BoxGroup("Death"), AssetsOnly]
        private GameObject explodePlayerPrefab;
        
        
        //Events
        public event Action PlayerDeath;


        private bool _dissolve;
        private float _dissolveSlider = -1;
        private static readonly int Dissolve1 = Shader.PropertyToID("_Dissolve");


        protected override void Start()
        {
            base.Start();
            player.Player.mainMaterial.SetFloat(Dissolve1, -1);
        }

        private void Update()
        {
            if (_dissolve)
            {
                if (_dissolveSlider > 1)
                {
                    _dissolve = false;
                    return;
                }
                _dissolveSlider = Mathf.MoveTowards(_dissolveSlider, 1, Time.deltaTime * dissolveSpeed);
                player.Player.mainMaterial.SetFloat(Dissolve1, _dissolveSlider);

            }
        }


        #region Overriden

        public override void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                StartCoroutine(PlayDead());
            }
        }


        public override void DeathByGodRay()
        {
            player.Player.CanMove = false;
            player.Player.canRotate = false;
            player.CrossFadeAnimation("DeathByGodRay", 0.2f);
        } 
        
        public override void DeathByParticles()
        {
            PlayerDeath?.Invoke();
            currentHealth = 0;
            player.Player.AnimationController.CrossFade("DeathByParticle", 0.01f, 1);
            player.Player.canRotate = false;
            player.Player.CanMove = false;
            
        }


        #endregion



        #region Custom methods
        
        private IEnumerator PlayDead()
        {
            PlayerDeath?.Invoke();
            player.Player.CanMove = false;
            player.Player.PlayerController.enabled = false;

            yield return new WaitForSeconds(0.01f);

            player.Player.AnimationController.enabled = false;

            CameraManager.Instance.followCamera.target = player.Player.AnimationController.GetBoneTransform(HumanBodyBones.Hips);

            yield return new WaitForSeconds(effectDelay);
            

            DeadEffects();
            
        }

        public void PlayerDamage(string animationName)
        {

            PlayerDeath?.Invoke();
            currentHealth = 0;
            player.Player.AnimationController.CrossFade(animationName, 0.1f);
            player.Player.CanMove = false;
            DeadEffects();

        }

        public IEnumerator DeathByAnimation(string animationName, float time = 0)
        {
            PlayerDeath?.Invoke();
            currentHealth = 0;
            player.Player.AnimationController.CrossFade(animationName, 0.1f);
            player.Player.CanMove = false;

            yield return new WaitForSeconds(time);
            
            CameraManager.Instance.followCamera.followSmoothness = 3f;
            CameraManager.Instance.followCamera.target = transform;
        }

        private void DeadEffects()
        {
            //layerMask = gameObject.layer << gameObject.layer; then I use layerMask = ~layerMask

            //Get exact blood position
            if (Physics.Raycast(player.Player.AnimationController.GetBoneTransform(HumanBodyBones.Hips).position, Vector3.down, out RaycastHit hit, Mathf.Infinity, 1 << 7))
            {
                player.Player.bloodSpill.Play();
                player.Player.bloodSpill.transform.position = hit.point + new Vector3(0, 0.05f, 0);
            }
        }
        

        #endregion
        
        

        #region Animation Calls

        public void Dissolve()
        {
            _dissolve = true;
        }

        public void PlayerExplode()
        {
            var obj = Instantiate(explodePlayerPrefab, GameManager.instance.transform);

            
            var transform1 = transform;
            obj.transform.position = transform1.position;
            obj.transform.rotation = transform1.rotation;
            
            PlayerDeath?.Invoke();
            
            Destroy(gameObject);
        }

        #endregion
        

    }
}
