using System.Collections.Generic;
using UnityEngine;

namespace TicTacToePro
{
    public class LanguagesScreen : MonoBehaviour
    {
        public static LanguagesScreen shared;

        public JuicyButton CloseButton;
        public List<JuicyButton> Languages;

        SettingsScreen _settingsScreen;

        GameScreen _screen;

        public List<string> GetAllLanguagesCodes()
        {
            return new List<string> { "en", "ar", "zh", "ja", "ko", "tr", "sv", "fr", "el", "de", "es", "ru" };
        }

        public string CurrentLanguageCode { get; private set; } = "en";

        void Awake()
        {
            shared = this;
            _screen = GetComponent<GameScreen>();
            if (GetAllLanguagesCodes().Contains(LocalUser.shared.SavedData.SettingsData.Language))
            {
                CurrentLanguageCode = LocalUser.shared.SavedData.SettingsData.Language;
            }
            else
            {
                LocalUser.shared.SavedData.SettingsData.Language = CurrentLanguageCode;
            }

            LocalUser.shared.Save();
        }

        void Start()
        {
            CloseButton.AddListener(Hide);

            InitLanguageButtons();
        }

        void InitLanguageButtons()
        {
            var i = 0;
            foreach (var lang in Languages)
            {
                var k = i;
                lang.AddListener(() => SelectLanguage(k));
                lang.transform.GetChild(0).gameObject.SetActive(false);
                i += 1;
            }
        }

        public void Hide()
        {
            _screen.Hide();
        }

        public void Show(SettingsScreen settingsScreen)
        {
            _settingsScreen = settingsScreen;
            Show();
        }

        public void Show()
        {
            _screen.Show();
            ShowCurrentLanguage();
        }

        public void SelectLanguage(int i)
        {
            var languages = GetAllLanguagesCodes();
            var code = languages[i];
            CurrentLanguageCode = code;

            foreach (var lang in Languages)
            {
                lang.transform.GetChild(0).gameObject.SetActive(false);
            }

            ShowCurrentLanguage();

            if (_settingsScreen != null)
                _settingsScreen.InitLanguage();

            LocalUser.shared.SavedData.SettingsData.Language = CurrentLanguageCode;
            LocalUser.shared.Save();
        }

        void ShowCurrentLanguage()
        {
            var languages = GetAllLanguagesCodes();
            var code = CurrentLanguageCode;
            var i = languages.IndexOf(code);
            Languages[i].transform.GetChild(0).gameObject.SetActive(true);
        }

    }
}