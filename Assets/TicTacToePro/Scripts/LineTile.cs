using System.Collections.Generic;

namespace TicTacToePro
{
    public class LineTile
    {
        List<Line> _passingTiles = new List<Line>();

        public List<Line> Lines => _passingTiles;

        public int Row;
        public int Col;

        public void Init(int row, int col)
        {
            Row = row;
            Col = col;
            _passingTiles = new List<Line>();
        }

        public void AddLine(Line line)
        {
            if (!_passingTiles.Contains(line))
                _passingTiles.Add(line);
        }
    }
}