using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToePro
{
    public class ConnectionLostScreen : MonoBehaviour
    {
        public JuicyButton HideScreenButton;

        GameScreen _gameScreen;

        void Awake()
        {
            _gameScreen = GetComponent<GameScreen>();
        }

        public void Show()
        {
            HideScreenButton.AddListener(Hide);
            _gameScreen.Show();
        }

        void Hide()
        {
            _gameScreen.Hide();
            HideScreenButton.RemoveAllListeners();
        }
    }
}