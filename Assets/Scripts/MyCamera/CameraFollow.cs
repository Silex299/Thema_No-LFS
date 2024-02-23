using System;
using System.Collections;
using Interactions;
using Path_Scripts;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyCamera
{
    public class CameraFollow : MonoBehaviour
    {
        #region Variables



        [TabGroup("Camera Movement", "Reference")]
        public Camera mainCamera;

        [TabGroup("Camera Movement", "Reference")]
        public Transform target;


        [TabGroup("Camera Movement", "Camera Shake"), SerializeField]
        private float shakeFrequency;

        [TabGroup("Camera Movement", "Camera Shake"), SerializeField]
        private float shakeAmplitude;

        [TabGroup("Camera Movement", "Camera Shake"), SerializeField]
        private float shakeSmoothness = 100;

        [TabGroup("Camera Movement", "Camera Shake"), SerializeField]
        private bool shakeCamera;

        public bool ShakeCamera
        {
            set => shakeCamera = value;
        }

        [TabGroup("Camera Movement", "Follow Movement"), SerializeField]
        private bool restrictYFollow;

        [TabGroup("Camera Movement", "Follow Movement")]
        public float collisionOffset = 1f;

        [TabGroup("Camera Movement", "Follow Movement")]
        public LayerMask layerMask;
        //[SerializeField, BoxGroup("Camera Movement")] public bool hardLook;


        [TabGroup("Camera Movement", "Follow Movement")]
        public float followSmoothness;

        [TabGroup("Camera Movement", "Follow Movement"), SerializeField]
        private Vector3 followOffset;


        [SerializeField, FoldoutGroup("Misc")] private Vector3 focusOffset;
        [SerializeField, FoldoutGroup("Misc")] private Vector3 focusRotation;

        #region Editor Variables

#if UNITY_EDITOR

        [FoldoutGroup("Misc"), Button("Set Focus Button")]
        public void SetFocusOffset()
        {
            var transform1 = transform;
            focusOffset = -target.position + transform1.position;
            focusRotation = transform1.rotation.eulerAngles;
        }

        [TabGroup("Camera Movement", "Follow Movement"), Button("Set Offset", ButtonSizes.Large),
         GUIColor(0.4f, 0.8f, 1f)]
        public void SetOffset()
        {
            if (target)
            {
                followOffset = -target.position + transform.position;
            }
            else
            {
                Debug.LogError("Target is not set");
            }
        }


        //TODO: REMOVE
        [SerializeField, Space(10), FoldoutGroup("Editor Preview")]
        private PlayerPathController path;

        [SerializeField, FoldoutGroup("Editor Preview"), PropertySpace(0, 10)]
        private int previewIndex;


        private Vector3 _prevPosition;
        private Quaternion _prevRotation;

        [Button("Set Camera Offset", ButtonSizes.Medium), GUIColor(0.5f, 0.1f, 0.1f), FoldoutGroup("Editor Preview")]
        public void SetCameraOffset()
        {
            if (path.PathPoints[previewIndex].TryGetComponent<ChangeCameraOffset>(out var cameraOffsetChange))
            {
                followOffset = cameraOffsetChange.PositionOffset;
                PreviewCamera();
            }
            else
            {
                Debug.LogWarning("No <ChangeCameraOffset> component found on the index");
            }
        }

        [Button("Set Point Offset", ButtonSizes.Medium), GUIColor(0.1f, 0.5f, 0.1f), FoldoutGroup("Editor Preview")]
        public void SetPointOffset()
        {
            if (path.PathPoints[previewIndex].TryGetComponent<ChangeCameraOffset>(out var cameraOffsetChange))
            {
                var transform1 = this.transform;

                cameraOffsetChange.PositionOffset = transform1.position - path.PathPoints[previewIndex].position;
                cameraOffsetChange.RotationOffset = transform1.rotation.eulerAngles;
            }
            else
            {
                Debug.LogWarning("No <ChangeCameraOffset> component found on the index");
            }
        }

        [Button("Preview", ButtonSizes.Large), GUIColor(0.4f, 1f, 0.4f), FoldoutGroup("Editor Preview")]
        public void PreviewCamera()
        {
            if (path.PathPoints[previewIndex].TryGetComponent<ChangeCameraOffset>(out var cameraOffsetChange))
            {
                var transform1 = transform;

                _prevPosition = transform1.position;
                _prevRotation = transform1.rotation;

                transform1.position = path.PathPoints[previewIndex].position + cameraOffsetChange.PositionOffset;

                if (cameraOffsetChange.RotationOffset != Vector3.zero)
                {
                    this.transform.rotation = Quaternion.Euler(cameraOffsetChange.RotationOffset);
                }
            }
            else
            {
                Debug.LogWarning("No <ChangeCameraOffset> component found on the index");
            }
        }

        [Button("Revert Back", ButtonSizes.Large), GUIColor(1f, 0f, 0f), FoldoutGroup("Editor Preview")]
        public void Revert()
        {
            var transform1 = transform;

            transform1.position = _prevPosition;
            transform1.rotation = _prevRotation;
        }


#endif

        #endregion


        private bool _changeOffset;
        private float _offsetChangeSmoothness;
        private Vector3 _newOffset;
        private Quaternion _newRotation;

        #endregion

        #region Built in methods

        private void Start()
        {
            //PlayerController.Instance.Player.Health.PlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
           // PlayerController.Instance.Player.Health.PlayerDeath -= OnPlayerDeath;
        }

        private void Update()
        {
            if (!target) return;


            Vector3 newPos = target.position + followOffset;

            if (restrictYFollow)
            {
                newPos.y = followOffset.y;
            }

            //Collision
            if (Physics.Linecast(target.position + new Vector3(0, 1.5f, 0), newPos, out RaycastHit hit, layerMask))
            {
                newPos = hit.point + transform.forward * collisionOffset;
            }


            transform.position = Vector3.Lerp(this.transform.position, newPos, Time.deltaTime * followSmoothness);

            //if(hardLook) this.transform.LookAt(target);

            if (!_changeOffset) return;


            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation,
                Time.deltaTime * _offsetChangeSmoothness);

            followOffset = Vector3.Lerp(followOffset, _newOffset, Time.deltaTime * _offsetChangeSmoothness);


            if (Mathf.Abs((followOffset - _newOffset).magnitude) < 0.1f &&
                Quaternion.Angle(transform.rotation, _newRotation) == 0f)
            {
                _changeOffset = false;
            }
        }

        private void LateUpdate()
        {
            if (shakeCamera)
            {
                CameraShake();
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Vector3 newPos = target.position + followOffset;
                Gizmos.color = Color.blue;

                Gizmos.DrawWireSphere(newPos, 0.4f);
            }
        }

        #endregion

        #region Custom Methods

        public void Focus(Transform t, float time = 0.5f)
        {
            target = t;
            ChangeOffset(focusOffset, focusRotation, time);
        }

        public void ChangeOffset(Vector3 offset, Vector3 rotation, float smoothness = 2)
        {
            _newOffset = offset;
            _newRotation = Quaternion.Euler(rotation);
            _changeOffset = true;
            _offsetChangeSmoothness = smoothness;
        }

        public void ChangeOffset(Vector3 offset, float smoothness = 2)
        {
            _newOffset = offset;
            _changeOffset = true;
            _offsetChangeSmoothness = smoothness;
        }


        private float _time;
        private Vector3 _cameraShakePos;
        private void CameraShake()
        {

            if (_time < Time.time)
            {
                _time = Time.time + (1 / shakeFrequency);
                var x = UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude);
                var y = UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude);
                var z = UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude);
                _cameraShakePos = new Vector3(x, y, z);
            }

            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, _cameraShakePos,
                Time.deltaTime * shakeSmoothness);

        }

        private void OnPlayerDeath()
        {
            StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            float lerp = 0;
            while (shakeAmplitude > 0.001f)
            {
                shakeAmplitude = Mathf.Lerp(shakeAmplitude, 0, lerp);
                lerp += 0.01f * 0.01f;
                yield return new WaitForSeconds(0.01f);
            }

            shakeCamera = false;
        }

        #endregion
    }
}