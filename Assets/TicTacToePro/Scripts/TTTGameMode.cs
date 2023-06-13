using System.Collections;
using TicTacToePro.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToePro
{
    public class TTTGameMode : MonoBehaviour
    {
        public TTTPowerups Powerups;
        public JuicyButton GoBackButton;
        public JuicyButton OptionsButton;
        public TMP_Dropdown DifficultyDropDown;

        public TTTWinVisualizer WinVisualizer;
        public GameModeManager ModeManager;
        public TTTGrid Grid;
        public GridButton GridButton;
        public ObjectPooler TilesPooler;
        public PooledObject GridTile;
        public Transform WinLengthVisual;
        public Image FirstPlayerMark;
        public Image SecondPlayerMark;
        public Sprite[] Marks;
        public TextMeshProUGUI FirstPlayerText;
        public TextMeshProUGUI SecondPlayerText;
        public string PlayerTerm;
        public string AiTerm;
        public GameScreen LoadingScreen;
        public ConnectionLostScreen ConnectionLostScreen;

        TTTAI _currentAI;
        GameSettings _currentSettings;
        GameBoard _board;
        GameObject[,] _tiles;

        bool _inputBlocked;

        bool _firstCrosses;
        bool _isAICross;
        bool _waitingForAI;
        bool _lookingForMatch;
        bool _checkingInternet;
        GameMode _gameMode;

        public GameBoard Board => _board;

        public bool CanReceiveInput
        {
            get
            {
                if (_board == null)
                    return false;

                if (_inputBlocked)
                    return false;

                if (_lookingForMatch)
                    return false;

                if (_currentAI != null)
                {
                    return !_waitingForAI && !_board.IsGameOver && _isAICross != _board.IsCrossTurn;
                }
                else
                {
                    return !_board.IsGameOver;
                }
            }
        }

        void Update()
        {
            OptionsButton.Interactable = !_inputBlocked;
        }

        void Awake()
        {
            var time = System.DateTime.Now;
            var initString = time.Hour * time.Minute * time.Second * time.Millisecond;
            Random.InitState(initString);
            OptionsButton.AddListener(OpenOptions);
            GoBackButton.AddListener(GoBack);
            DifficultyDropDown.onValueChanged.AddListener(ChangeDifficulty);
            WinVisualizer.Init(this, Grid);

            LocalUser.shared.SavedData.SettingsData.OnUserIDChanged.AddListener(UpdateUsernameFirstPlayer);
        }

        void OpenOptions()
        {
            if (_inputBlocked)
                return;

            SettingsScreen.shared.Show();
            SettingsScreen.shared.ShowHomeButton(GoBackSettings);
            SettingsScreen.shared.ShowRestartButton(RestartSettings);
        }

        void GoBackSettings()
        {
            SettingsScreen.shared.Hide();
            GoBackForce();
        }

        void RestartSettings()
        {
            SettingsScreen.shared.Hide();
            RestartGame();
        }

        void GoBack()
        {
            if (_inputBlocked && !_lookingForMatch)
                return;

            if (_lookingForMatch)
                ToggleLoading(false);

            GoBackForce();
        }

        public void GoBackForce()
        {
            if (_board != null)
                _board.GameOver.RemoveListener(GameEnd);

            StopAllCoroutines();
            _lookingForMatch = false;
            _board = null;
            ScreenManager.shared.SelectScreen(ScreenType.TTTSelectGameMode);
        }

        public void SelectBoardType(BasicGameMode mode)
        {
            ModeManager.CurrentGameMode = mode;
        }

        void SetGameMode(GameMode gameMode)
        {
            _gameMode = gameMode;
        }

        public void StartAIGame()
        {
            SetGameMode(GameMode.AI);
            InitDifficulty();
            OpenScreen();
            GetAI();
            AssignSides();
            GetSettings();
            SetupBoard();
            SetupPlayerNamesAndMarks();
            ConnectAI();
            AIMoveFirst();
        }

        public void StartOnlineGameAI()
        {
            SetGameMode(GameMode.OnlineAI);
            OpenScreen();
            SetupOnline();
            GetSettings();
            SetupBoard();
            StartCoroutine(TryToFindMatchOnline());
        }

        void ToggleLoading(bool toggle)
        {
            BlockInput(toggle);
            _lookingForMatch = toggle;
            if (toggle)
                LoadingScreen.Show();
            else
                LoadingScreen.Hide();
        }

        void UpdateUsernameFirstPlayer()
        {
            FirstPlayerText.text = LocalUser.shared.SavedData.SettingsData.UserID;
        }

        IEnumerator TryToFindMatchOnline()
        {
            UpdateUsernameFirstPlayer();
            SecondPlayerText.text = "???";


            _checkingInternet = true;
            StartCoroutine(OnlineManager.shared.CheckInternetConnection(HasConnection));

            ToggleLoading(true);

            var waitSeconds = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitSeconds);

            ToggleLoading(false);

            SetupOnlineGameAI();
        }

        void HasConnection(bool hasConn)
        {
            _checkingInternet = false;


            if (hasConn)
                return;

            if (_board.IsGameOver)
                return;

            ConnectionLostScreen.Show();
            GoBack();
        }

        void SetupOnlineGameAI()
        {
            AssignSides();
            SetupPlayerNamesAndMarksOnline();
            ConnectAI();
            AIMoveFirst();
        }

        public void StartLocalGame()
        {
            SetGameMode(GameMode.Human);
            OpenScreen();
            AssignSides();
            SetupLocal();
            GetSettings();
            SetupBoard();
            SetupPlayerNamesAndMarks();
        }

        public void VisualTurn()
        {
            if (_currentAI == null)
            {
                if (_board.IsCrossTurn == _firstCrosses)
                {
                    FirstPlayerMark.GetComponent<Animator>().SetTrigger("Show");
                    SecondPlayerMark.GetComponent<Animator>().SetTrigger("Hide");
                }
                else
                {
                    FirstPlayerMark.GetComponent<Animator>().SetTrigger("Hide");
                    SecondPlayerMark.GetComponent<Animator>().SetTrigger("Show");
                }
            }
            else
            {
                if (_board.IsCrossTurn == !_isAICross)
                {
                    FirstPlayerMark.GetComponent<Animator>().SetTrigger("Show");
                    SecondPlayerMark.GetComponent<Animator>().SetTrigger("Hide");
                }
                else
                {
                    FirstPlayerMark.GetComponent<Animator>().SetTrigger("Hide");
                    SecondPlayerMark.GetComponent<Animator>().SetTrigger("Show");
                }
            }
        }

        void SetupPlayerNamesAndMarks()
        {
            UpdateUsernameFirstPlayer();
            if (_currentAI != null)
            {
                var firstMark = _isAICross ? 0 : 1;
                FirstPlayerMark.sprite = Marks[firstMark];
                var secondMark = _isAICross ? 1 : 0;
                SecondPlayerMark.sprite = Marks[secondMark];

                SecondPlayerText.text = AiTerm.ToString();
            }
            else
            {
                var firstMark = _firstCrosses ? 1 : 0;
                FirstPlayerMark.sprite = Marks[firstMark];
                var secondMark = _firstCrosses ? 0 : 1;
                SecondPlayerMark.sprite = Marks[secondMark];

                SecondPlayerText.text = PlayerTerm.ToString();
            }

            VisualTurn();
        }

        void SetupPlayerNamesAndMarksOnline()
        {
            UpdateUsernameFirstPlayer();
            if (_currentAI != null)
            {
                var firstMark = _isAICross ? 0 : 1;
                FirstPlayerMark.sprite = Marks[firstMark];
                var secondMark = _isAICross ? 1 : 0;
                SecondPlayerMark.sprite = Marks[secondMark];

                SecondPlayerText.text = NameGenerator.GetUserName();
            }

            VisualTurn();
        }

        public void RestartGame()
        {
            switch (_gameMode)
            {
                case GameMode.AI:
                    StartAIGame();
                    break;
                case GameMode.Human:
                    StartLocalGame();
                    break;
                case GameMode.OnlineHuman:
                    break;
                case GameMode.OnlineAI:
                    StartOnlineGameAI();
                    break;
                default:
                    break;
            }
        }

        void AssignSides()
        {
            _firstCrosses = Random.Range(0f, 1f) > 0.5f;
            var aiStarts = Random.Range(0f, 1f) > 0.7f;
            if (aiStarts)
                _isAICross = _firstCrosses;
            if (_board != null)
                _board.Init(true, _firstCrosses);
        }

        void GetSettings()
        {
            _currentSettings = ModeManager.CurrentGameSettings;
        }

        void SetupOnline()
        {
            _currentAI = new TTTAI();
            _waitingForAI = false;
            DifficultyDropDown.gameObject.SetActive(false);
        }

        void SetupLocal()
        {
            _currentAI = null;
            DifficultyDropDown.gameObject.SetActive(false);
        }

        void GetAI()
        {
            _currentAI = new TTTAI();
            _waitingForAI = false;
        }

        void InitDifficulty()
        {
            DifficultyDropDown.gameObject.SetActive(true);
            var difficulty = (int)ModeManager.CurrentAIDifficultyLevel;
            DifficultyDropDown.value = difficulty;
        }

        void ChangeDifficulty(int newDifficulty)
        {
            ModeManager.ChangeDifficulty((DifficultyLevel)newDifficulty);
        }

        void ConnectAI()
        {
            _currentAI.Init(_board, _isAICross, ModeManager.CurrentAIDifficulty);
        }

        void AIMoveFirst()
        {
            if (_isAICross == _firstCrosses)
            {
                var delay = 0f;

                if (_gameMode == GameMode.AI)
                    delay = ModeManager.AIFirstWait;
                if (_gameMode == GameMode.OnlineAI)
                    delay = ModeManager.OnlineFirstWait;

                StartCoroutine(AIPlayAfterDelay(delay));
            }
        }

        IEnumerator AIPlayAfterDelay(float delay)
        {
            if (_waitingForAI)
                yield break;

            _waitingForAI = true;
            yield return new WaitForSeconds(delay);

            if (_board == null)
                yield break;

            int result;
            if (_board.IsFirstMove)
                result = _currentAI.PlayFirstMove();
            else
                result = _currentAI.MakeMove();
            _waitingForAI = false;

            if (result == 0)
                VisualTurn();
        }

        public void PlayerPlay(int x, int y)
        {
            if (_board.IsGameOver)
                return;

            if (_currentAI != null)
            {
                AIPlayerPlayLoop(x, y);
            }
            else
            {
                Player2PlayerPlayLoop(x, y);
            }
        }

        void Player2PlayerPlayLoop(int x, int y)
        {
            var result = _board.PlayerPlay(x, y, _board.IsCrossTurn);

            if (result == 0)
                VisualTurn();

        }


        void SetupBoard()
        {
            _board = new GameBoard();
            _board.GameOver.RemoveAllListeners();
            _board.GameOver.AddListener(GameEnd);
            var width = _currentSettings.BoardWidth;
            _tiles = new GameObject[width, width];
            TilesPooler.DisableAllPooledObjects();

            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    var tile = TilesPooler.GetPooledObject(GridTile);
                    tile.SetActive(true);
                    _tiles[i, k] = tile;
                }
            }

            _board.Init(this, _tiles, _currentSettings, true, _firstCrosses);

            Grid.ResizeNow(width);
            GridButton.Init(this, width);

            SetupWinLength();
        }

        void SetupWinLength()
        {
            for (int i = 0; i < WinLengthVisual.childCount; i++)
            {
                var visible = i < _currentSettings.WinningLength;
                WinLengthVisual.GetChild(i).gameObject.SetActive(visible);

                var playerMark = 0;
                if (_currentAI != null)
                {
                    playerMark = _isAICross ? 0 : 1;
                }
                else
                {
                    playerMark = _firstCrosses ? 1 : 0;
                }

                WinLengthVisual.GetChild(i).GetComponent<Image>().sprite = Marks[playerMark];
            }
        }

        public void BlockInput(bool toggle)
        {
            _inputBlocked = toggle;
        }

        void GameEnd(int result)
        {
            BlockInput(true);

            var playerResult = 1;
            if (_currentAI != null)
            {
                var unoReverse = _isAICross ? -1 : 1;
                playerResult = result * unoReverse;
                ModeManager.RecalculateUserAIDiffMod(playerResult);
            }
            else
            {
                var unoReverse = _firstCrosses ? 1 : -1;
                playerResult = result * unoReverse;
            }

            var lastMark = _board.LastPlacedMark();
            var winMarksPos = _board.GetPositionsOfWinningMarks();
            WinVisualizer.ShowWin(result, winMarksPos, lastMark, playerResult);
        }

        void OpenScreen()
        {
            StopAllCoroutines();
            BlockInput(false);
            ScreenManager.shared.SelectScreen(ScreenType.TTTGame);
        }

        public void Undo()
        {
            _board.UndoLastMove();
        }

        public void Hint()
        {
            var ai = new TTTAI();
            var isCross = !_isAICross;
            if (_currentAI == null)
                isCross = _board.IsCrossTurn;


            ai.Init(_board, isCross, ModeManager.AISettings(DifficultyLevel.Hard));

            var aiDiffHard = new UserAIDifficultySettings();
            aiDiffHard.ChangeAllBlunders(0f);
            aiDiffHard.ChangeAllScoreDiffs(0f);

            ai.OverrideUserDiff(aiDiffHard);

            int result;
            if (_board.IsFirstMove)
                result = ai.PlayFirstMove();
            else
                result = ai.MakeMove();

            if (result == 0)
                VisualTurn();

            if (_currentAI != null)
            {
                PlayAI();
            }

        }

        void PlayAI()
        {
            if (_board.IsGameOver)
                return;

            if (_gameMode == GameMode.OnlineAI && !_checkingInternet)
            {
                _checkingInternet = true;
                StartCoroutine(OnlineManager.shared.CheckInternetConnection(HasConnection));
            }


            var delayVec = new Vector2();

            if (_gameMode == GameMode.AI)
                delayVec = ModeManager.AIDelay;
            if (_gameMode == GameMode.OnlineAI)
                delayVec = ModeManager.OnlineDelay;


            var delay = Random.Range(delayVec.x, delayVec.y);
            StartCoroutine(AIPlayAfterDelay(delay));
        }

        void AIPlayerPlayLoop(int x, int y)
        {
            var result = _board.PlayerPlay(x, y, !_isAICross);

            if (result != 0)
                return;

            VisualTurn();

            PlayAI();
        }

    }

    public enum GameMode
    {
        AI,
        Human,
        OnlineHuman,
        OnlineAI
    }
}