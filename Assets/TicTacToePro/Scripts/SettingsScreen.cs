using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TicTacToePro
{
    public class SettingsScreen : MonoBehaviour
    {
        public static SettingsScreen shared;

        public JuicyButton CloseButton;

        public JuicyButton LanguageButton;


        public JuicyToggle SFXToggle;
        public JuicyToggle MusicToggle;
        public JuicyToggle VibrationsToggle;

        public JuicyButton HomeButton;
        public JuicyButton RestartButton;

        public LanguagesScreen LanguageScreenPopup;

        public TextMeshProUGUI UserID;
        public JuicyButton ChangeUsername;
        public ChangeUserIDScreen ChangeUsernameScreenOpen;

        GameScreen _screen;


        void Awake()
        {
            shared = this;
            _screen = GetComponent<GameScreen>();
        }

        void Start()
        {
            CloseButton.AddListener(Hide);

            LanguageButton.AddListener(OpenLanguages);

            SFXToggle.AddListener(ToggleSFX);
            MusicToggle.AddListener(ToggleMusic);
            VibrationsToggle.AddListener(ToggleVibrations);
            ChangeUsername.AddListener(ChangeUsernameScreen);

            InitToggles();

            DisableButtons();

            InitUserID();
        }

        void ChangeUsernameScreen()
        {
            ChangeUsernameScreenOpen.Show();
        }

        void InitUserID()
        {
            LocalUser.shared.SavedData.SettingsData.OnUserIDChanged.AddListener(UpdateUsername);
            UpdateUsername();
        }

        void UpdateUsername()
        {
            UserID.text = LocalUser.shared.SavedData.SettingsData.UserID;
        }

        void OpenLanguages()
        {
            LanguageScreenPopup.Show(this);
        }

        void InitToggles()
        {
            var sfx = LocalUser.shared.SavedData.SettingsData.SFXEnabled;
            SFXToggle.ToggleInstant(sfx);

            var music = LocalUser.shared.SavedData.SettingsData.MusicEnabled;
            MusicToggle.ToggleInstant(music);

            var vibrations = LocalUser.shared.SavedData.SettingsData.VibrationEnabled;
            VibrationsToggle.ToggleInstant(vibrations);
        }

        public void Hide()
        {
            _screen.Hide();
            RestartButton.RemoveAllListeners();
            HomeButton.RemoveAllListeners();
        }

        public void Show()
        {
            InitToggles();
            InitLanguage();
            DisableButtons();
            _screen.Show();
        }

        public void InitLanguage()
        {
            var languages = LanguagesScreen.shared.GetAllLanguagesCodes();
            var code = LanguagesScreen.shared.CurrentLanguageCode;
            var i = languages.IndexOf(code);

            var languageSprite = LanguageScreenPopup.Languages[i].GetComponent<Image>().sprite;
            LanguageButton.GetComponent<Image>().sprite = languageSprite;
        }

        void ToggleVibrations(bool toggle)
        {
            LocalUser.shared.SavedData.SettingsData.VibrationEnabled = toggle;
            LocalUser.shared.Save();
        }

        void ToggleSFX(bool toggle)
        {
            SoundManager.shared.ToggleSFXVolume(toggle);
        }

        void ToggleMusic(bool toggle)
        {
            SoundManager.shared.ToggleMusicVolume(toggle);
        }

        public void ShowHomeButton(UnityAction action)
        {
            HomeButton.RemoveAllListeners();
            HomeButton.AddListener(action);

            HomeButton.transform.parent.GetComponent<Canvas>().enabled = true;
        }

        public void ShowRestartButton(UnityAction action)
        {
            RestartButton.RemoveAllListeners();
            RestartButton.AddListener(action);

            RestartButton.transform.parent.GetComponent<Canvas>().enabled = true;
        }

        void DisableButtons()
        {
            RestartButton.transform.parent.GetComponent<Canvas>().enabled = false;
            HomeButton.transform.parent.GetComponent<Canvas>().enabled = false;
        }
    }
}