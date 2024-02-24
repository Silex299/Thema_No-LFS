using Thema_Camera;
using UnityEngine;

namespace Managers.Checkpoints
{

    public class CameraOffsetTracker : Tracker
    {

        [SerializeField] private ChangeOffset offset;

        public override void ResetItem(CheckPoint checkpoint)
        {
            offset.ChangeCameraOffsetInstantaneous();
            CameraManager.Instance.Reset();
        }

        public override void InitialSetup(CheckPoint checkPoint)
        {
            offset.ChangeCameraOffsetInstantaneous();
        }

    }

}