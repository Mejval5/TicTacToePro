using UnityEngine;

namespace TicTacToePro
{
    public class SoundEffectPlayer : MonoBehaviour
    {
        public SFXName SFXSound;

        public void PlaySound()
        {
            if (SoundManager.shared != null)
                SoundManager.shared.PlaySFX(SFXSound);
        }
    }
}