using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TicTacToePro
{
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class TTTGrid : UIBehaviour
    {
        GridLayoutGroup _grid;
        RectTransform RectTransform;

        int _boardWidth = 3;

        protected override void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            RectTransform = (RectTransform)transform;
        }

        protected override void OnRectTransformDimensionsChange() => Resized();

        public void Resized()
        {
            if (_grid == null)
                _grid = GetComponent<GridLayoutGroup>();
            if (RectTransform == null)
                RectTransform = (RectTransform)transform;

            var size = RectTransform.rect.size / _boardWidth;

            _grid.cellSize = size - _grid.spacing;
        }

        public void ResizeNow(int boardWidth)
        {
            _boardWidth = boardWidth;

            Resized();

            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        }
    }
}