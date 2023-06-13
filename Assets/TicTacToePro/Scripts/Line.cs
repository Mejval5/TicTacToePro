using System.Collections.Generic;

namespace TicTacToePro
{
    public struct LineParams
    {
        public int StartX;
        public int StartY;
        public int XRate;
        public int YRate;
        public int MaxLength;
    }

    public enum LineDirection
    {
        Horizontal,
        Vertical,
        Diagonal,
        InverseDiagonal
    }

    public struct LineDirectionsVectors
    {
        public static Dictionary<LineDirection, int[]> Dirs = new Dictionary<LineDirection, int[]>()
        {
            { LineDirection.Horizontal, new int[2] { 1, 0 } },
            { LineDirection.Vertical, new int[2] { 0, 1 } },
            { LineDirection.Diagonal, new int[2] { 1, 1 } },
            { LineDirection.InverseDiagonal, new int[2] { -1, 1 } },
        };
    }

    public class Line
    {
        public int[] Contents;
        public LineParams Params = new();
        public int WinLength;
        public bool ContainsX;
        public bool ContainsY;
        public bool Winnable => WinLength <= Params.MaxLength;

        public List<int> ExpansionZoneX = new();
        public List<int> DangerZoneX = new();
        public List<int> WinZoneX = new();

        public List<int> ExpansionZoneO = new();
        public List<int> DangerZoneO = new();
        public List<int> WinZoneO = new();

        public int[] GetPos(int index)
        {
            var x = Params.StartX + index * Params.XRate;
            var y = Params.StartY + index * Params.YRate;
            return new int[2] { x, y };
        }

        public int GetPosLinear(int index, int boardWidth)
        {
            var x = Params.StartX + index * Params.XRate;
            var y = Params.StartY + index * Params.YRate;
            var linearPos = x * boardWidth + y;
            return linearPos;
        }



        public int GetIndexFromX(int x)
        {
            var index = (x - Params.StartX) / Params.XRate;
            return index;
        }

        public int GetIndexFromY(int y)
        {
            var index = (y - Params.StartY) / Params.YRate;
            return index;
        }

        public void Init(LineParams parameters, int winLength)
        {
            Params = parameters;
            WinLength = winLength;
            InitContents();
        }

        public void PlayAt(int pos, int mark)
        {
            Contents[pos] = mark;
            UpdateContents();
        }

        public void UnPlayAt(int pos)
        {
            Contents[pos] = 0;
            UpdateContents();
        }

        void UpdateContents()
        {
            UpdateMarks();
            UpdateZones();
        }

        string ContentsToString()
        {
            string contents = "";
            foreach (var mark in Contents)
            {
                if (mark == -1)
                    contents += "o";
                if (mark == 0)
                    contents += " ";
                if (mark == 1)
                    contents += "x";
            }

            return contents;
        }

        void UpdateZones()
        {
            if (!Winnable)
                return;

            string contents = ContentsToString();

            ExpansionZoneX = GetExpansionList(contents, 1);
            DangerZoneX = GetDangerList(contents, 1);
            WinZoneX = GetWinList(contents, 1);

            ExpansionZoneO = GetExpansionList(contents, -1);
            DangerZoneO = GetDangerList(contents, -1);
            WinZoneO = GetWinList(contents, -1);
        }

        private List<int> GetExpansionList(string contents, int mark)
        {
            List<int> expansionList = new();
            for (int i = 1; i < WinLength; i++)
            {
                foreach (var pos in GetMatchedPatterns(contents, mark, WinLength, i))
                    expansionList.Add(pos);
            }

            return expansionList;
        }

        private List<int> GetDangerList(string contents, int mark)
        {
            List<int> expansionList = new();
            foreach (var pos in GetMatchedPatterns(contents, mark, WinLength, WinLength - 2))
                expansionList.Add(pos);
            return expansionList;
        }

        private List<int> GetWinList(string contents, int mark)
        {
            List<int> expansionList = new();
            foreach (var pos in GetMatchedPatterns(contents, mark, WinLength, WinLength - 1))
                expansionList.Add(pos);
            return expansionList;
        }

        private List<int> GetMatchedPatterns(string contents, int mark, int winLength, int amount)
        {
            List<int> matchedIndexes = new();

            char thisMark;
            char oppositeMark;
            if (mark == 1)
            {
                thisMark = 'x';
                oppositeMark = 'o';
            }
            else
            {
                thisMark = 'o';
                oppositeMark = 'x';
            }

            for (int i = 0; i < contents.Length; i++)
            {
                if (i + winLength > contents.Length)
                    break;

                var substring = contents.Substring(i, winLength);
                if (substring.Contains(oppositeMark))
                    continue;

                int amountThisMark = 0;
                for (int k = 0; k < substring.Length; k++)
                {
                    if (substring[k] == thisMark)
                        amountThisMark += 1;
                }

                if (amountThisMark != amount)
                    continue;

                for (int k = 0; k < substring.Length; k++)
                {
                    if (substring[k] == ' ')
                        matchedIndexes.Add(k + i);
                }
            }

            return matchedIndexes;
        }


        void UpdateMarks()
        {
            for (int i = 0; i < Params.MaxLength; i++)
            {
                if (Contents[i] == 1)
                    ContainsX = true;
                if (Contents[i] == -1)
                    ContainsY = true;
            }
        }

        void InitContents()
        {
            Contents = new int[Params.MaxLength];
            for (int i = 0; i < Params.MaxLength; i++)
            {
                Contents[i] = 0;
            }
        }

        public static bool CheckPossibleLine(int x, int y, int winLength, int boardWidth, int xRate, int yRate, out LineParams parameters)
        {
            parameters = new LineParams
            {
                XRate = xRate,
                YRate = yRate,
                StartX = x,
                StartY = y,
                MaxLength = 0
            };

            if (boardWidth < 1 || winLength < 1)
                return false;

            int lineLength = 0;
            for (int i = 0; i < boardWidth; i++)
            {
                int _x = x + i * xRate;
                int _y = y + i * yRate;

                if (_x < 0 || _y < 0 || _x > boardWidth - 1 || _y > boardWidth - 1)
                    break;

                lineLength += 1;
            }

            parameters.MaxLength = lineLength;

            return true;
        }
    }
}