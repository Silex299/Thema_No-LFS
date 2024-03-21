using UnityEngine;
using Sirenix.OdinInspector;

namespace Misc
{

    public class Rotor : MonoBehaviour
    {
        [SerializeField, BoxGroup] private bool active = true;

        [SerializeField, BoxGroup] private float speed;
        [SerializeField, BoxGroup] private Vector3 rotationAxis;

        [SerializeField, BoxGroup] private float accelerationTime;

        private bool _running;
        private bool _transition;
        private float _currentSpeed;
        private float _transitionTimeElapsed;


        private void Start()
        {
            _running = active;
            _currentSpeed = speed;
            _transition = false;
        }

        private void Update()
        {
            if (!active) return;


            if (_transition)
            {
                if (_running)
                {
                    StopRotor();
                }
                else
                {
                    StartRotor();
                }
            }
            else if(_running)
            {
                Rotate();
            }

        }

        private void Rotate()
        {
            float degreesPerSecond = _currentSpeed * 360f / 60f;
            float rotationAngle = degreesPerSecond * Time.deltaTime;
            transform.Rotate(rotationAxis, rotationAngle, Space.Self);
        }

        [SerializeField, Button("Stop Rotor", ButtonSizes.Medium), GUIColor(1f,0f,0f)]
        public void StopRotor()
        {
            if (!_running) return;

            if (!_transition)
            {
                _transition = true;
                _transitionTimeElapsed = 0;
            }
            else
            {

                Rotate();
                _transitionTimeElapsed += Time.deltaTime;

                float fraction = _transitionTimeElapsed / accelerationTime;

                _currentSpeed = Mathf.Lerp(speed, 0, fraction);

                if (fraction >= 1)
                {
                    _transition = false;
                    _running = false;
                }
            }
        }
        
        [SerializeField, Button("Start Rotor", ButtonSizes.Medium), GUIColor(0.2f, 1f, 0.2f)]
        public void StartRotor()
        {
            if (_running) return;

            if (!_transition)
            {
                _transition = true;
                _transitionTimeElapsed = 0;
            }
            else
            {
                Rotate();

                _transitionTimeElapsed += Time.deltaTime;

                float fraction = _transitionTimeElapsed / accelerationTime;

                _currentSpeed = Mathf.Lerp(0, speed, fraction);

                if (fraction >= 1)
                {
                    _transition = false;
                    _running = true;
                }

            }
        }


        public void ToggleRotor(bool start)
        {
            if (!start)
            {
                StopRotor();
            }
            else
            {
                StartRotor();
            }
        }
    }

}