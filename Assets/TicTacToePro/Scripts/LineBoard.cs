using System.Collections.Generic;

namespace TicTacToePro
{
    public class LineBoard
    {
        List<Line> _lines;
        GameBoard _board;
        List<LineTile> _lineTiles;

        public List<Line> Lines => _lines;

        public void Init(GameBoard board)
        {
            _board = board;
        }

        public void Create()
        {
            _lineTiles = new();
            _lines = new();
            var width = _board.BoardWidth;
            var winLength = _board.WinningLength;
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var lineTile = new LineTile();
                    lineTile.Init(row, col);
                    _lineTiles.Add(lineTile);
                }
            }


            GenerateLines(width, winLength);
        }

        void AddLineToTiles(Line newLine)
        {
            var width = _board.BoardWidth;
            for (int i = 0; i < newLine.Params.MaxLength; i++)
            {
                var position = newLine.GetPosLinear(i, width);
                _lineTiles[position].AddLine(newLine);
            }
        }

        public void PlayMark(int row, int col, int mark)
        {
            var width = _board.BoardWidth;
            var index = row * width + col;
            foreach (var line in _lineTiles[index].Lines)
            {
                int lineIndex;
                if (line.Params.XRate != 0)
                    lineIndex = line.GetIndexFromX(row);
                else
                    lineIndex = line.GetIndexFromY(col);

                line.PlayAt(lineIndex, mark);
            }
        }

        public List<int> GetBasicBestMoves(float maxDiff)
        {
            List<TileValue> tileValues = new();
            int highestScore = 0;
            for (int i = 0; i < _lineTiles.Count; i++)
            {
                if (_board.IsTileMarked(i))
                    continue;

                var lineTile = _lineTiles[i];
                int score = 0;
                foreach (var line in lineTile.Lines)
                {
                    if (!line.Winnable)
                        continue;

                    if (line.ContainsX)
                        score += 1;
                    if (line.ContainsY)
                        score += 1;
                }

                if (score == 0)
                    continue;

                if (highestScore < score)
                    highestScore = score;
                var tileValue = new TileValue()
                {
                    score = score,
                    position = i
                };

                tileValues.Add(tileValue);
            }

            List<int> bestTiles = new();
            for (int i = 0; i < tileValues.Count; i++)
            {
                var tileValue = tileValues[i];
                if ((highestScore - tileValue.score) <= maxDiff)
                {
                    bestTiles.Add(tileValue.position);
                }
            }

            return bestTiles;
        }

        public List<int> GetDangerZoneBestMoves(bool crosses, float maxDiff)
        {
            List<TileValue> tileValues = new();
            int highestScore = 0;
            for (int i = 0; i < _lineTiles.Count; i++)
            {
                if (_board.IsTileMarked(i))
                    continue;

                var lineTile = _lineTiles[i];
                int score = 0;
                foreach (var line in lineTile.Lines)
                {
                    if (!line.Winnable)
                        continue;

                    List<int> dangerZone;
                    if (crosses)
                        dangerZone = line.DangerZoneX;
                    else
                        dangerZone = line.DangerZoneO;

                    if (dangerZone.Count == 0)
                        continue;

                    foreach (var dangerIndex in dangerZone)
                    {
                        var posIndex = line.GetPosLinear(dangerIndex, _board._boardWidth);
                        if (posIndex == i)
                            score += 1;
                    }
                }

                if (score == 0)
                    continue;

                if (highestScore < score)
                    highestScore = score;
                var tileValue = new TileValue()
                {
                    score = score,
                    position = i
                };

                tileValues.Add(tileValue);
            }

            List<int> bestTiles = new();
            for (int i = 0; i < tileValues.Count; i++)
            {
                var tileValue = tileValues[i];
                if ((highestScore - tileValue.score) <= maxDiff)
                {
                    bestTiles.Add(tileValue.position);
                }
            }

            return bestTiles;
        }

        public List<int> GetWinZoneBestMoves(bool crosses, float maxDiff)
        {
            List<TileValue> tileValues = new();
            int highestScore = 0;
            for (int i = 0; i < _lineTiles.Count; i++)
            {
                if (_board.IsTileMarked(i))
                    continue;

                var lineTile = _lineTiles[i];
                int score = 0;
                foreach (var line in lineTile.Lines)
                {
                    if (!line.Winnable)
                        continue;

                    List<int> winZone;
                    if (crosses)
                        winZone = line.WinZoneX;
                    else
                        winZone = line.WinZoneO;

                    if (winZone.Count == 0)
                        continue;

                    foreach (var winIndex in winZone)
                    {
                        var posIndex = line.GetPosLinear(winIndex, _board._boardWidth);
                        if (posIndex == i)
                            score += 1;
                    }
                }

                if (score == 0)
                    continue;

                if (highestScore < score)
                    highestScore = score;
                var tileValue = new TileValue()
                {
                    score = score,
                    position = i
                };

                tileValues.Add(tileValue);
            }

            List<int> bestTiles = new();
            for (int i = 0; i < tileValues.Count; i++)
            {
                var tileValue = tileValues[i];
                if ((highestScore - tileValue.score) <= maxDiff)
                {
                    bestTiles.Add(tileValue.position);
                }
            }

            return bestTiles;
        }

        public List<int> GetExpansionBestMoves(bool crosses, float maxDiff)
        {
            List<TileValue> tileValues = new();
            int highestScore = 0;
            for (int i = 0; i < _lineTiles.Count; i++)
            {
                if (_board.IsTileMarked(i))
                    continue;

                var lineTile = _lineTiles[i];
                int score = 0;
                foreach (var line in lineTile.Lines)
                {
                    if (!line.Winnable)
                        continue;

                    List<int> expansionZone;
                    if (crosses)
                        expansionZone = line.ExpansionZoneX;
                    else
                        expansionZone = line.ExpansionZoneO;

                    if (expansionZone.Count == 0)
                        continue;

                    foreach (var expansionIndex in expansionZone)
                    {
                        var posIndex = line.GetPosLinear(expansionIndex, _board._boardWidth);
                        if (posIndex == i)
                            score += 1;
                    }
                }

                if (score == 0)
                    continue;

                if (highestScore < score)
                    highestScore = score;
                var tileValue = new TileValue()
                {
                    score = score,
                    position = i
                };

                tileValues.Add(tileValue);
            }

            List<int> bestTiles = new();
            for (int i = 0; i < tileValues.Count; i++)
            {
                var tileValue = tileValues[i];
                if ((highestScore - tileValue.score) <= maxDiff)
                {
                    bestTiles.Add(tileValue.position);
                }
            }

            return bestTiles;
        }

        void GenerateLines(int boardWidth, int winLength)
        {
            int x = 0;
            for (int y = 0; y < boardWidth; y++)
            {
                GenerateOneLine(x, y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.Horizontal]);
                GenerateOneLine(x, y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.Diagonal]);
            }

            int _y = 0;
            for (int _x = 0; _x < boardWidth; _x++)
            {
                GenerateOneLine(_x, _y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.Vertical]);
                GenerateOneLine(_x, _y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.InverseDiagonal]);
                if (_x == 0)
                    continue;
                GenerateOneLine(_x, _y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.Diagonal]);
            }

            int __x = boardWidth - 1;
            for (int __y = 0; __y < boardWidth; __y++)
            {
                if (__y == 0)
                    continue;
                GenerateOneLine(__x, __y, winLength, boardWidth, LineDirectionsVectors.Dirs[LineDirection.InverseDiagonal]);
            }
        }

        void GenerateOneLine(int row, int col, int winLength, int boardWidth, int[] dir)
        {
            var possible = Line.CheckPossibleLine(row, col, winLength, boardWidth, dir[0], dir[1], out LineParams lineParams);
            if (!possible)
                return;
            Line newLine = new Line();

            newLine.Init(lineParams, winLength);
            _lines.Add(newLine);
            AddLineToTiles(newLine);
        }
    }

    public struct TileValue
    {
        public int score;
        public int position;
    }
}