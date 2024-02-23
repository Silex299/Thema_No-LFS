using Player_Scripts;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Health
{

    public class PlayerHealth : HealthBaseClass
    {
       
        [SerializeField, BoxGroup("Player Health")] private Material dissolveMaterial;
        [SerializeField, BoxGroup("Player Health")] private float dissolveSpeed;

        [SerializeField, FoldoutGroup("Misc")] private Component[] componentsToDestroy;
        private bool _dissolveDeath;

        private float _dissolveSlider = -1;
        private static readonly int Dissolve1 = Shader.PropertyToID("_Dissolve");


        private void Start()
        {
            dissolveMaterial.SetFloat(Dissolve1, -1);
        }

        private void Update()
        {
            if (_dissolveDeath)
            {
                if (_dissolveSlider >= 1)
                {
                    _dissolveDeath = false;
                    Death();
                    return;
                }
                _dissolveSlider = Mathf.MoveTowards(_dissolveSlider, 1, Time.deltaTime * dissolveSpeed);
                dissolveMaterial.SetFloat(Dissolve1, _dissolveSlider);
            }
        }


        public override void Kill(string message)
        {
            if(message == "RAY")
            {
                DisableCompoents();
                PlayerMovementController.Instance.PlayAnimation("Float Death", 0.5f, 1); //Play Death  Animation
            }
            else
            {
                DisableCompoents();
                PlayerMovementController.Instance.PlayAnimation(message, 0.2f, 1); //Play Death  Animation
            }
        }


        public void DissolveDeath()
        {
            _dissolveDeath = true;
        }


        public void DisableCompoents()
        {
            foreach(var component in componentsToDestroy)
            {
                Destroy(component);
            }
        }

    }

}