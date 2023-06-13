using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToePro
{
    public class TTTWinVisualizer : MonoBehaviour
    {
        public ScaleConnectors WinLine;
        public Canvas WinScreen;
        public float LineTime;
        public RectTransform WinPanel;
        public GameObject Board;
        public string EffectsLayer;
        public JuicyButton HomeButton;
        public JuicyButton RestartButton;

        public List<WinLineColors> LineColors;
        public List<WinParticleColors> ParticleColors;

        public WinLineWidths LineWidths(BasicGameMode mode)
        {
            switch (mode)
            {
                case BasicGameMode._3x3:
                    return _3x3LineWidths;
                case BasicGameMode._6x6:
                    return _6x6LineWidths;
                case BasicGameMode._9x9:
                    return _9x9LineWidths;
                case BasicGameMode._11x11:
                    return _11x11LineWidths;
                default:
                    return _3x3LineWidths;
            }
        }

        public WinLineWidths _3x3LineWidths;
        public WinLineWidths _6x6LineWidths;
        public WinLineWidths _9x9LineWidths;
        public WinLineWidths _11x11LineWidths;
        public GameScreen WinGameScreen;

        public GameObject WinHeader;
        public GameObject DrawHeader;
        public GameObject LoseHeader;

        TTTGameMode _gameMode;
        TTTGrid _gridBoard;
        int _lastPlayerResult;

        void Awake()
        {
            HomeButton.AddListener(GoHome);
            RestartButton.AddListener(Restart);

            WinLine.gameObject.SetActive(false);
            WinGameScreen.Idle();
        }

        public void Init(TTTGameMode gameMode, TTTGrid gridBoard)
        {
            _gameMode = gameMode;
            _gridBoard = gridBoard;
        }

        void GoHome()
        {
            HideWin();
            _gameMode.GoBackForce();
        }

        void ResetHeaders()
        {
            WinHeader.SetActive(false);
            DrawHeader.SetActive(false);
            LoseHeader.SetActive(false);
        }

        public void ShowWin(int result, List<int[]> marks, int[] lastMark, int playerResult)
        {
            VibrationsManager.shared.Vibrate();

            ResetHeaders();

            _lastPlayerResult = playerResult;

            if (_lastPlayerResult == 1)
            {
                WinHeader.SetActive(true);
                SoundManager.shared.PlaySFX(SFXName.Win);
                LocalUser.shared.SavedData.AnalyticsData.TimesWon += 1;
                LocalUser.shared.SessionData.TimesWon += 1;
            }

            if (_lastPlayerResult == 0)
            {
                DrawHeader.SetActive(true);
                SoundManager.shared.PlaySFX(SFXName.Draw);
                LocalUser.shared.SavedData.AnalyticsData.TimesDraw += 1;
                LocalUser.shared.SessionData.TimesDraw += 1;
            }

            if (_lastPlayerResult == -1)
            {
                LoseHeader.SetActive(true);
                SoundManager.shared.PlaySFX(SFXName.Lose);
                LocalUser.shared.SavedData.AnalyticsData.TimesLost += 1;
                LocalUser.shared.SessionData.TimesLost += 1;
            }

            LocalUser.shared.Save();

            if (result == 0)
            {
                ShowEndScreen(result, lastMark);
                return;
            }

            foreach (var markPos in marks)
            {
                HighlightMark(markPos);
            }

            StartCoroutine(WinCoroutine(result, marks, lastMark));
        }

        IEnumerator WinCoroutine(int result, List<int[]> marks, int[] lastMark)
        {
            yield return StartCoroutine(ShowLine(result, marks, lastMark));
        }

        IEnumerator ShowLine(int result, List<int[]> marks, int[] lastMark)
        {
            int[] firstMark = marks[0];
            var lastMarkPos = lastMark;
            if (firstMark[0] == lastMarkPos[0] && firstMark[1] == lastMarkPos[1])
            {
                firstMark = marks[marks.Count - 1];
                lastMarkPos = marks[0];
            }
            else
            {
                lastMarkPos = marks[marks.Count - 1];
            }

            var xStart = firstMark[1];
            var yStart = _gameMode.Board.BoardWidth - 1 - firstMark[0];

            WinLine.StartPos = new Vector2Int(xStart, yStart);

            var xEnd = lastMarkPos[1];
            var yEnd = _gameMode.Board.BoardWidth - 1 - lastMarkPos[0];

            WinLine.EndPos = new Vector2Int(xEnd, yEnd);
            WinLine.BoardSize = _gameMode.Board.BoardWidth;

            var lineColors = LineColors[result == 1 ? 1 : 0];
            WinLine.EdgeLineColor = lineColors.EdgeColor;
            WinLine.InnerLineColor = lineColors.InnerColor;

            var lineWidth = LineWidths(_gameMode.ModeManager.CurrentGameMode);
            WinLine.EdgeLineWidth = lineWidth.EdgeWidth;
            WinLine.InnerLineWidth = lineWidth.InnerWidth;


            foreach (var particles in WinLine.Particles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Stop();
                var particleColors = ParticleColors[result == 1 ? 1 : 0];
                ParticleSystem.MainModule settings = particles.main;
                settings.startColor = new ParticleSystem.MinMaxGradient(particleColors.ParticleColor);

                ParticleSystem.TrailModule trails = particles.trails;
                trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(particleColors.TrailColor);
            }



            WinLine.CurrentSize = 0f;
            WinLine.UpdateSizeNow();

            //particles.Clear();
            WinLine.gameObject.SetActive(true);

            foreach (var particles in WinLine.Particles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Play();
            }

            var playTime = 0f;
            while (playTime < LineTime)
            {
                var t = playTime / LineTime;
                WinLine.CurrentSize = t;
                WinLine.UpdateSizeNow();
                yield return null;
                playTime += Time.deltaTime;
            }

            WinLine.CurrentSize = 1f;
            WinLine.UpdateSizeNow();

            yield return new WaitForSeconds(1f);
            InterstitialDefault.shared.OpenInterstitial();
            ShowEndScreen(result, lastMark);
        }


        void HighlightMark(int[] pos)
        {

        }

        void ShowRateUs()
        {
            LocalUser.shared.SessionData.AlreadyRated = true;
            LocalUser.shared.SavedData.AnalyticsData.TimesShownRating += 1;
            StartCoroutine(PlayReviewManager.shared.ReviewLoop());
        }


        void ShowEndScreen(int result, int[] lastMark)
        {
            var timesPlayedSession = LocalUser.shared.SessionData.TimesPlayed;
            var timesShownRating = LocalUser.shared.SavedData.AnalyticsData.TimesShownRating;
            if (timesPlayedSession >= 2 && _lastPlayerResult == 1 || timesPlayedSession >= 5)
            {
                if (!LocalUser.shared.SessionData.AlreadyRated && timesShownRating <= 2)
                    ShowRateUs();
            }

            WinGameScreen.Show();
            var winBoard = Instantiate(Board, WinPanel);
            winBoard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            winBoard.GetComponent<TTTGrid>().ResizeNow(_gameMode.Board.BoardWidth);
            winBoard.GetComponent<GridButton>().enabled = false;

            var connectors = GetComponentInChildren<ScaleConnectors>();
            if (connectors == null)
                return;

            foreach (var line in connectors.Lines)
            {
                line.sortingLayerName = EffectsLayer;
            }

            connectors.BGLine.sortingLayerName = EffectsLayer;
        }

        void HideWin()
        {
            StartCoroutine(HideWinTimeout());
            WinLine.gameObject.SetActive(false);
            WinGameScreen.Hide();
        }

        IEnumerator HideWinTimeout()
        {
            yield return new WaitForSeconds(0.1f);
            DestroyOldWin();
        }

        void DestroyOldWin()
        {
            for (int i = WinPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(WinPanel.GetChild(i).gameObject);
            }
        }

        void Restart()
        {
            HideWin();
            _gameMode.RestartGame();
        }
    }

    [Serializable]
    public class WinLineColors
    {
        public Color EdgeColor;
        public Color InnerColor;
    }

    [Serializable]
    public class WinParticleColors
    {
        public Color ParticleColor;
        public Color TrailColor;
    }

    [Serializable]
    public class WinLineWidths
    {
        public float EdgeWidth;
        public float InnerWidth;
    }
}