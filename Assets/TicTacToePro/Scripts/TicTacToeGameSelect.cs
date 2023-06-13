using UnityEngine;

namespace TicTacToePro
{
    public class TicTacToeGameSelect : MonoBehaviour
    {
        public TTTGameMode GameMode;
        public ScrollRectGravity BoardSelector;

        public JuicyButton BackButton;
        public JuicyButton OptionsButton;

        public JuicyButton StartAIGameButton;
        public JuicyButton StartLocalGameButton;
        public JuicyButton OpenMultiplayerButton;


        void Awake()
        {
            BackButton.AddListener(Back);
            OptionsButton.AddListener(OpenOptions);

            StartAIGameButton.AddListener(StartAIGame);
            StartLocalGameButton.AddListener(StartLocalGame);
            OpenMultiplayerButton.AddListener(StartOnlineGame);
        }

        void StartOnlineGame()
        {
            var selectedMode = (BasicGameMode)BoardSelector.CurrentlySelectedIndex;

            GameMode.SelectBoardType(selectedMode);
            GameMode.StartOnlineGameAI();
        }

        void OpenOptions()
        {
            SettingsScreen.shared.Show();
        }

        void Back()
        {
            ScreenManager.shared.SelectScreen(ScreenType.GameSelector);
        }

        void OpenPrivacyPolicy()
        {
            Application.OpenURL("https://sites.google.com/view/arcane-raccoon/home");
        }

        void StartAIGame()
        {
            var selectedMode = (BasicGameMode)BoardSelector.CurrentlySelectedIndex;

            GameMode.SelectBoardType(selectedMode);
            GameMode.StartAIGame();
        }

        void StartLocalGame()
        {
            var selectedMode = (BasicGameMode)BoardSelector.CurrentlySelectedIndex;

            GameMode.SelectBoardType(selectedMode);
            GameMode.StartLocalGame();
        }
    }
}