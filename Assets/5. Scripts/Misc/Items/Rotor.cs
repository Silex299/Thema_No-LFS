using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.Items
{

    public class Rotor : MonoBehaviour
    {
        //[SerializeField, BoxGroup("Params")] private bool active = true;

        [SerializeField, BoxGroup("Params")] private float speed;
        [SerializeField, BoxGroup("Params")] private float accelerationTime;
        [SerializeField, BoxGroup("Params")] private Vector3 rotationAxis;


        [SerializeField, BoxGroup("Sounds")] private AudioSource source;
        [SerializeField, BoxGroup("Sounds"), Space(10)] private AudioClip machineSound;
        [SerializeField, BoxGroup("Sounds")] private AudioClip startSound;
        [SerializeField, BoxGroup("Sounds")] private AudioClip stopSound;

        
        private float _currentSpeed;
        private Coroutine _speedCoroutine;
      
        private void Awake()
        {
            _currentSpeed = speed;
        }

        private void Update()
        {
            Rotate();

            if (!source) return;
            if (source.isPlaying) return;
            
            source.clip = machineSound;
            source.loop = true;
            source.Play();
        }

        private void Rotate()
        {
            float degreesPerSecond = _currentSpeed * 360f / 60f;
            float rotationAngle = degreesPerSecond * Time.deltaTime;
            transform.Rotate(rotationAxis, rotationAngle, Space.Self);
        }

        [Button("Stop Rotor", ButtonSizes.Medium), GUIColor(1f,0f,0f)]
        public void StopRotor()
        {
            if (source)
            {
                source.PlayOneShot(stopSound);
            }
            if(_speedCoroutine!=null) StopCoroutine(_speedCoroutine);
            _speedCoroutine = StartCoroutine(ChangeRotorSpeed(0));
        }
        
        [Button("Start Rotor", ButtonSizes.Medium), GUIColor(0.2f, 1f, 0.2f)]
        public void StartRotor()
        {
            if (source)
            {
                source.PlayOneShot(startSound);
            }
            if(_speedCoroutine!=null) StopCoroutine(_speedCoroutine);
            _speedCoroutine = StartCoroutine(ChangeRotorSpeed(speed));    
        }

        private IEnumerator ChangeRotorSpeed(float overrideSpeed)
        {
            float timeElapsed = 0;
            float startSpeed = _currentSpeed;

            while (timeElapsed < accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _currentSpeed = Mathf.Lerp(startSpeed, overrideSpeed, timeElapsed / accelerationTime);
                yield return null;
            }
        }


        public void ToggleRotor(bool start)
        {
            if (start)
            {
                _currentSpeed = speed;
            }
            else
            {
                _currentSpeed = 0;
            }
        }
    }

}