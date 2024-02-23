using Player_Scripts;
using UnityEngine;

namespace Misc
{
    public class DamagingVolume : MonoBehaviour
    {
        public bool isActive = true;

        public bool isPlayed;
        public bool inverted;

        public void ApplyDamage(float damage)
        {
            if (!isActive || isPlayed) return;

            PlayerController.Instance.Player.Health.TakeDamage(damage);
            isPlayed = true;
        }

        public void Death(string animationName)
        {
            if (!isActive || isPlayed) return;
            PlayerController.Instance.Player.Health.PlayerDamage(animationName);
            isPlayed = true;
        }

        public void SetVolume(bool value)
        {
            if (inverted)
            {
                isActive = !value;
                return;
            }
            isActive = value;
        }
    }
}
