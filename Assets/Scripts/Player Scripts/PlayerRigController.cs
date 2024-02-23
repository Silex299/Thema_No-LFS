using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player_Scripts
{
    public class PlayerRigController : MonoBehaviour
    {

        #region variables

        [SerializeField, BoxGroup("Rigs")] private TwoBoneIKConstraint leftHandIK;
        [SerializeField, BoxGroup("Rigs")] private TwoBoneIKConstraint rightHandIK;
        [SerializeField, BoxGroup("Rigs")] private Rig headAim;


        [SerializeField, BoxGroup("Rig Target")] private Transform leftHandIKTarget;
        [SerializeField, BoxGroup("Rig Target")] private Transform rightHandIKTarget;
        [SerializeField, BoxGroup("Rig Target")] private Transform headAimTarget;

        [SerializeField, BoxGroup("Misc")] private float weightAdjustmentSpeed;


        #region Non exposed variables

        private Transform _leftHandTarget;
        public Transform LeftHandTarget
        {
            set => _leftHandTarget = value;
        }
        private Transform _rightHandTarget;
        public Transform RightHandTarget
        {
            set => _rightHandTarget = value;
        }

        private Transform _headAimTarget;


        private bool _followLeftHand;
        private bool _followRightHand;
        private bool _followHead;

        #endregion


        #endregion

        #region Builtin methods

        private void FixedUpdate()
        {
            //Left hand IK
            if (_followLeftHand)
            {
                leftHandIKTarget.position = _leftHandTarget.position;
                leftHandIKTarget.rotation = _leftHandTarget.rotation;

                if (leftHandIK.weight != 1)
                {
                    leftHandIK.weight = Mathf.MoveTowards(leftHandIK.weight, 1, Time.deltaTime * weightAdjustmentSpeed);
                }

            }
            else if (leftHandIK.weight != 0)
            {
                leftHandIK.weight = Mathf.MoveTowards(leftHandIK.weight, 0, Time.deltaTime * weightAdjustmentSpeed);
            }

            //Right hand IK
            if (_followRightHand)
            {
                rightHandIKTarget.position = _rightHandTarget.position;
                rightHandIKTarget.rotation = _rightHandTarget.rotation;


               //print(rightHandIKTarget.position == _rightHandTarget.position);
                if (rightHandIK.weight != 1)
                {
                    rightHandIK.weight = Mathf.MoveTowards(rightHandIK.weight, 1, Time.deltaTime * weightAdjustmentSpeed);
                }
            }
            else if (rightHandIK.weight != 0)
            {
                rightHandIK.weight = Mathf.MoveTowards(rightHandIK.weight, 0, Time.deltaTime * weightAdjustmentSpeed);
            }

            //Head aim
            if (_followHead)
            {
                headAimTarget.position = _headAimTarget.position;

                if (headAim.weight != 1)
                {
                    headAim.weight = Mathf.MoveTowards(headAim.weight, 1, Time.deltaTime * weightAdjustmentSpeed);
                }
            }
            else if (headAim.weight != 0)
            {
                headAim.weight = Mathf.MoveTowards(headAim.weight, 0, Time.deltaTime * weightAdjustmentSpeed);
            }

        }

        #endregion

        #region methods

        public void SetLeftHandTarget(Transform target)
        {
            _leftHandTarget = target;
        }
        public void SetRightHandTarget(Transform target)
        {
            _rightHandTarget = target;
        }

        public void SetLeftHandIK()
        {
            _followLeftHand = true;
        }
        public void ResetLeftHandIK()
        {
            _followLeftHand = false;
        }

        public void SetRightHandIK()
        {
            _followRightHand = true;
        }
        public void ResetRightHandIK()
        {
            _followRightHand = false;
        }


        public void SetHeadAimTarget(Transform target)
        {
            _headAimTarget = target;
            _followHead = true;
        }

        public void ResetHeadAimTarget()
        {
            _followHead = false;
        }


        #endregion

    }
}