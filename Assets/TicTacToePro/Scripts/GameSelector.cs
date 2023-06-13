using UnityEngine;

namespace TicTacToePro
{
    public class GameSelector : MonoBehaviour
    {
        public JuicyButton TTTButton;
        public JuicyButton BlocksButton;

        void Awake()
        {
            TTTButton.AddListener(OpenTTT);
            BlocksButton.AddListener(OpenBlocks);
        }

        void OpenTTT()
        {
            ScreenManager.shared.SelectScreen(ScreenType.TTTSelectGameMode);
        }

        void OpenBlocks()
        {
            ScreenManager.shared.SelectScreen(ScreenType.Main2048);
        }
    }
}