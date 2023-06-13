using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace TicTacToePro
{
    public class GameBoard
    {
        public MonoBehaviour Owner;
        public int _boardWidth = 3;
        public int WinningLength = 3;
        public int MaxIterations = 100;
        public int AvailableMoves;
        public int PossibleMoves;
        public UnityEvent<int> GameOver = new();

        public GameObject[,] BoardTiles;
        public Mark[,] Board;
        public List<int> BoardInts;
        List<int[]> _moves;
        private Mark _winningMark;
        private bool _crossTurn, _gameOver;
        private bool _display;
        private LineBoard _lineBoard;

        int[] _lastVisualSpot = new int[2] { 0, 0 };
        int[] _lastPlayedSpot = new int[2] { 0, 0 };

        public bool IsFirstMove => PossibleMoves == AvailableMoves;
        public LineBoard LineBoard => _lineBoard;
        public bool IsCrossTurn => _crossTurn;

        public int BoardWidth => _boardWidth;

        public bool IsGameOver => _gameOver;

        public Mark WinningMark => _winningMark;

        public void Init(MonoBehaviour owner, GameObject[,] tiles, GameSettings settings, bool display, bool crossStart)
        {
            BoardTiles = tiles;
            Owner = owner;
            _boardWidth = settings.BoardWidth;
            WinningLength = settings.WinningLength;
            Init(display, crossStart);
        }

        public void Init(bool display, bool crossStart)
        {
            _display = display;
            AvailableMoves = _boardWidth * _boardWidth;
            PossibleMoves = _boardWidth * _boardWidth;
            Board = new Mark[_boardWidth, _boardWidth];
            _crossTurn = crossStart;
            _gameOver = false;
            _winningMark = Mark.BLANK;
            _moves = new List<int[]>();
            InitialiseBoard();
            CreateLineBoard();
        }

        void CreateLineBoard()
        {
            _lineBoard = new();
            _lineBoard.Init(this);
            _lineBoard.Create();
        }


        private void InitialiseBoard()
        {
            BoardInts = new List<int>();
            for (int row = 0; row < _boardWidth; row++)
            {
                for (int col = 0; col < _boardWidth; col++)
                {
                    Board[row, col] = Mark.BLANK;
                    BoardInts.Add(0);
                    if (_display)
                    {
                        BoardTiles[row, col].transform.GetChild(0).gameObject.SetActive(true);
                        BoardTiles[row, col].transform.GetChild(1).gameObject.SetActive(false);
                        BoardTiles[row, col].transform.GetChild(2).gameObject.SetActive(false);
                        BoardTiles[row, col].transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
            }
        }

        public void ShowVisual(int x, int y)
        {
            if (Board[x, y] != Mark.BLANK)
                return;
            HideVisual();
            BoardTiles[x, y].transform.GetChild(1).gameObject.SetActive(true);
            _lastVisualSpot[0] = x;
            _lastVisualSpot[1] = y;
        }

        public void HideVisual()
        {
            var lastSpotTile = BoardTiles[_lastVisualSpot[0], _lastVisualSpot[1]];
            lastSpotTile.transform.GetChild(1).gameObject.SetActive(false);
        }

        private bool IsBoardFull()
        {
            for (int row = 0; row < _boardWidth; row++)
            {
                for (int col = 0; col < _boardWidth; col++)
                {
                    if (Board[row, col] == Mark.BLANK)
                        return false;
                }
            }

            return true;
        }

        /**
         * Attempt to mark tile at the given coordinates if they are valid and it is
         * possible to do so, toggle the player and check if the placing the mark
         * has resulted in a win.
         *
         * @param row Row coordinate to attempt to mark
         * @param col Column coordinate to attempt to mark
         * @return true if mark was placed successfully
         */
        //public void PlayerPlay(int row, int col)
        //{
        //    if (_crossTurn)
        //    {
        //        if (!PlaceMark(row, col))
        //            return;
        //        var aiMove = AI.GetBestMove(this);
        //        PlaceMark(aiMove[0], aiMove[1]);
        //    }
        //}

        public void Restart()
        {
            _crossTurn = UnityEngine.Random.Range(0f, 1f) > 0.5f;
            _gameOver = false;
            _winningMark = Mark.BLANK;
            BoardInts = new List<int>();
            AvailableMoves = _boardWidth * _boardWidth;
            _moves = new List<int[]>();
            InitialiseBoard();
            CreateLineBoard();
        }

        public int PlayRandomMove(bool crosses)
        {
            var list = new List<Vector2Int>();
            for (int row = 0; row < _boardWidth; row++)
            {
                for (int col = 0; col < _boardWidth; col++)
                {
                    if (Board[row, col] == Mark.BLANK)
                        list.Add(new Vector2Int(row, col));
                }
            }

            if (list.Count == 0)
                return -1;

            var index = UnityEngine.Random.Range(0, list.Count);
            var move = list[index];
            return PlayerPlay(move.x, move.y, crosses);
        }

        public int PlayerPlay(int index, bool crosses)
        {
            int row = Mathf.FloorToInt(index / _boardWidth);
            int col = index % _boardWidth;
            return PlayerPlay(row, col, crosses);
        }

        public int PlayerPlay(int row, int col, bool crosses)
        {

            if (_gameOver)
                return -1;

            if (crosses != _crossTurn)
                return -1;

            if (IsBoardFull())
            {
                FullBoard();
                return -1;
            }

            if (!PlaceMark(row, col))
                return -1;


            var result = EvaluateBoard();
            if (result == 0 && IsBoardFull())
            {
                FullBoard();
                return 1;
            }

            if (MathF.Abs(result) == 1)
            {
                Win(result);
                return 1;
            }

            return 0;
        }

        void Win(int win)
        {
            GameOver.Invoke(win);
        }

        void FullBoard()
        {
            GameOver.Invoke(0);
        }

        public bool PlaceMark(int row, int col)
        {
            if (row < 0 || row >= _boardWidth || col < 0 || col >= _boardWidth || IsTileMarked(row, col) || _gameOver)
            {
                return false;
            }

            AvailableMoves--;

            var mark = _crossTurn ? 1 : -1;
            Board[row, col] = _crossTurn ? Mark.X : Mark.O;

            BoardInts[row * _boardWidth + col] = mark;
            _lineBoard.PlayMark(row, col, mark);

            _lastPlayedSpot = new int[2] { row, col };
            _moves.Add(_lastPlayedSpot);
            if (_display)
            {
                SoundManager.shared.PlaySFX(SFXName.Bubble);
                BoardTiles[row, col].transform.GetChild(0).gameObject.SetActive(false);

                if (_crossTurn)
                    BoardTiles[row, col].transform.GetChild(2).gameObject.SetActive(true);
                else
                    BoardTiles[row, col].transform.GetChild(3).gameObject.SetActive(true);
            }

            TogglePlayer();
            var result = EvaluateBoard();
            if (result == 1)
            {
                //Debug.Log("X win");
                _gameOver = true;
                _winningMark = Mark.X;
            }

            if (result == -1)
            {
                //Debug.Log("O win");
                _gameOver = true;
                _winningMark = Mark.O;
            }

            return true;
        }

        /**
         * Check row and column provided and diagonals for win.
         *
         * @param row Row to check
         * @param col Column to check
         */
        public int EvaluateBoard()
        {
            int width = BoardWidth;
            int currentScore = 0;
            Mark currentState = Mark.BLANK;
            // Horizontal

            for (int row = 0; row < width; row++)
            {
                currentState = Mark.BLANK;
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(row, cell);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                currentState = Mark.BLANK;
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(cell, column);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }

            var maxLength = 0;
            for (int column = 0; column < width; column++)
            {
                maxLength = width - column;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = column + cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }

            for (int row = 1; row < width; row++)
            {
                maxLength = width - row;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = cell;
                    var posY = row + cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                if (column < WinningLength - 1)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell <= column; cell++)
                {
                    var posX = column - cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }


            for (int column = 1; column < width; column++)
            {
                if (column >= width - WinningLength + 1)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < width - column; cell++)
                {
                    var posX = cell + column;
                    var posY = width - cell - 1;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= WinningLength)
                        {
                            if (currentState == Mark.X)
                                return 1;
                            if (currentState == Mark.O)
                                return -1;
                        }
                    }
                }
            }

            return 0;
        }

        public int GetTotalBoardScore()
        {
            int currentScore = 0;

            for (int i = 2; i <= WinningLength; i++)
            {
                currentScore += GetBoardScoreForLength(i) * i * i * i * i * i;
            }

            currentScore -= GetBoardScoreForLength(1);
            return currentScore;
        }

        int GetBoardScoreForLength(int desiredLength)
        {
            int width = BoardWidth;
            int boardScore = 0;
            int currentScore = 0;
            Mark currentState;

            for (int row = 0; row < width; row++)
            {
                currentState = Mark.BLANK;
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(row, cell);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                currentState = Mark.BLANK;
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(cell, column);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }

            var maxLength = 0;
            for (int column = 0; column < width; column++)
            {
                maxLength = width - column;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = column + cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }

            for (int row = 1; row < width; row++)
            {
                maxLength = width - row;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = cell;
                    var posY = row + cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                if (column < WinningLength - 1)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell <= column; cell++)
                {
                    var posX = column - cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }


            for (int column = 1; column < width; column++)
            {
                if (column >= width - WinningLength + 1)
                    continue;

                currentState = Mark.BLANK;
                for (int cell = 0; cell < width - column; cell++)
                {
                    var posX = cell + column;
                    var posY = width - cell - 1;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 0;
                        currentState = mark;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        if (currentScore >= desiredLength)
                        {
                            if (currentState == Mark.X)
                                boardScore += 1;
                            if (currentState == Mark.O)
                                boardScore += -1;

                            currentScore = 0;
                        }
                    }
                }
            }

            return boardScore;
        }

        public int[] LastPlacedMark()
        {
            return _lastPlayedSpot;
        }

        public List<int[]> GetPositionsOfWinningMarks()
        {
            List<int[]> outputList = new();
            int width = BoardWidth;
            int currentScore = 0;
            Mark currentState;

            for (int row = 0; row < width; row++)
            {
                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(row, cell);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { row, cell });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { row, cell });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell < width; cell++)
                {
                    var mark = GetMarkAt(cell, column);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { cell, column });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { cell, column });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }

            var maxLength = 0;
            for (int column = 0; column < width; column++)
            {
                maxLength = width - column;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = column + cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { posX, posY });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { posX, posY });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }

            for (int row = 1; row < width; row++)
            {
                maxLength = width - row;
                if (maxLength < WinningLength)
                    continue;

                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell < maxLength; cell++)
                {
                    var posX = cell;
                    var posY = row + cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { posX, posY });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { posX, posY });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }

            for (int column = 0; column < width; column++)
            {
                if (column < WinningLength - 1)
                    continue;

                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell <= column; cell++)
                {
                    var posX = column - cell;
                    var posY = cell;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { posX, posY });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { posX, posY });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }


            for (int column = 1; column < width; column++)
            {
                if (column >= width - WinningLength + 1)
                    continue;

                currentState = Mark.BLANK;
                outputList = new();
                for (int cell = 0; cell < width - column; cell++)
                {
                    var posX = cell + column;
                    var posY = width - cell - 1;
                    var mark = GetMarkAt(posX, posY);
                    if (mark == Mark.BLANK)
                    {
                        currentScore = 0;
                        currentState = Mark.BLANK;
                        outputList = new();
                        continue;
                    }

                    if (currentState != mark)
                    {
                        currentScore = 1;
                        currentState = mark;
                        outputList = new();
                        outputList.Add(new int[2] { posX, posY });
                        continue;
                    }

                    if (currentState == mark)
                    {
                        currentScore += 1;
                        outputList.Add(new int[2] { posX, posY });
                        if (currentScore >= WinningLength)
                        {
                            return outputList;
                        }
                    }
                }
            }

            return outputList;
        }

        private void TogglePlayer()
        {
            _crossTurn = !_crossTurn;
        }

        public bool AnyMovesAvailable()
        {
            return AvailableMoves > 0;
        }

        public Mark GetMarkAt(int row, int column)
        {
            return Board[row, column];
        }

        public bool IsTileMarked(int row, int column)
        {
            return Board[row, column] != Mark.BLANK;
        }

        public bool IsTileMarked(int index)
        {
            int row = Mathf.FloorToInt(index / _boardWidth);
            int col = index % _boardWidth;
            return IsTileMarked(row, col);
        }

        public void SetMarkAt(int row, int column, Mark newMark)
        {
            if (newMark == Mark.BLANK)
                AvailableMoves++;
            else
                AvailableMoves--;

            Board[row, column] = newMark;

            int x = 0;
            switch (newMark)
            {
                case Mark.X:
                    x = 1;
                    break;
                case Mark.O:
                    x = -1;
                    break;
            }

            BoardInts[row * _boardWidth + column] = x;
        }

        public void UndoLastMove()
        {
            UndoOnce();
            UndoOnce();
        }

        void UndoOnce()
        {
            if (_moves.Count == 0)
                return;

            var move = _moves.Last();
            _moves.RemoveAt(_moves.Count - 1);
            SetMarkAt(move[0], move[1], Mark.BLANK);
            _lineBoard.PlayMark(move[0], move[1], 0);

            if (_display)
            {
                BoardTiles[move[0], move[1]].transform.GetChild(0).gameObject.SetActive(true);

                BoardTiles[move[0], move[1]].transform.GetChild(2).gameObject.SetActive(false);
                BoardTiles[move[0], move[1]].transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }
}
