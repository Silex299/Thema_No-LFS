using Sirenix.OdinInspector;
using UnityEngine;


namespace Player_Scripts.Interactables
{
    public class DraggableCart : DragBox
    {

        [SerializeField, BoxGroup("Cart Porperty")] private float wheelSpeed;
        [SerializeField, BoxGroup("Cart Porperty")] private Transform[] wheelTransforms;



        public bool _overrideEffects;
        private float lastDirection;
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_isInteracting)
            {
                if (_isMoving)
                {
                    foreach(var wheel in wheelTransforms)
                    {
                        var multiplier = _movingForward ? -1 : 1;
                        wheel.Rotate(multiplier * wheelSpeed * Time.fixedDeltaTime, 0, 0, Space.Self);
                    }
                }
            }

            if (_overrideEffects)
            {
                foreach (var effect in effects)
                {
                    effect.Play();
                }

                if (!soundSource.isPlaying)
                {
                    soundSource.Play();
                }
            }
            else if(!_isMoving)
            {
                if (soundSource.isPlaying)
                {
                    soundSource.Stop();
                    foreach (var effect in effects)
                    {
                        effect.Stop();
                    }
                }
            }

        }

        public void OverrideEffects(bool overrideEffects)
        {
            _overrideEffects = overrideEffects;
        }
    }


}