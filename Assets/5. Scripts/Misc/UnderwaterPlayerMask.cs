using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class UnderwaterPlayerMask : MonoBehaviour
    {
        
        public Animator animator;
        private Transform _targetBone;
        private Coroutine _equipCoroutine;
        private static Player Player => PlayerMovementController.Instance.player;
        
        private void Start()
        {
            //Get target bone
            _targetBone = Player.AnimationController.GetBoneTransform(HumanBodyBones.Head);
            transform.parent = _targetBone;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            
            Player.waterMovement.onSurfaceAction += SurfaceUpdate;
            Player.waterMovement.CanDamage = false;
            Player.Health.ResetHealth();
        }
        private void OnDisable()
        {
            Player.waterMovement.CanDamage = true;
            Player.waterMovement.onSurfaceAction -= SurfaceUpdate;
        }
        private void OnDestroy()
        {
            Player.waterMovement.CanDamage = true;
        }
        
        [Button]
        public void Equip(bool equip)
        {
            animator.Play(equip? "Equip" : "UnEquip");
        }
        
        private void SurfaceUpdate(bool onSurface)
        {
            Equip(!onSurface);
        }
        
        
    }
}
