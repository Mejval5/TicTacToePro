using UnityEngine;

namespace TicTacToePro
{
    public class VibrationsManager : MonoBehaviour
    {
        public static VibrationsManager shared;

        void Awake()
        {
            shared = this;
        }

        public void Vibrate()
        {
            bool vibrate = LocalUser.shared.SavedData.SettingsData.VibrationEnabled;

            if (vibrate)
            {

#if !UNITY_EDITOR
     Handheld.Vibrate();
#endif
            }
        }
    }
}