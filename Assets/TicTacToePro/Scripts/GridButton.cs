using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TicTacToePro
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler, IEndDragHandler
    {
        GridLayoutGroup _grid;
        RectTransform _rectTrans;
        Canvas _mainCanvas;
        int _gridWidth = 3;
        TTTGameMode _gameMode;

        int _lastX;
        int _lastY;

        public void Init(TTTGameMode gameMode, int width)
        {
            ResetLastPos();
            _gameMode = gameMode;
            _gridWidth = width;
        }

        void ResetLastPos()
        {
            _lastX = -1;
            _lastY = -1;
        }

        void Awake()
        {
            _rectTrans = GetComponent<RectTransform>();
            _mainCanvas = GetComponentInParent<Canvas>().rootCanvas;
            _grid = GetComponent<GridLayoutGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UpdateVisual(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateVisual(eventData);
        }

        void TryToPlaceMark(PointerEventData eventData)
        {
            int indexX, indexY;
            GetIndexes(eventData, out indexX, out indexY);
            var isXOK = indexX >= 0 && indexX <= _gridWidth - 1;
            var isYOK = indexY >= 0 && indexY <= _gridWidth - 1;
            if (isXOK && isYOK)
                PlaceMark(indexX, indexY);
        }

        void UpdateVisual(PointerEventData eventData)
        {
            if (!_gameMode.CanReceiveInput)
            {
                HideVisual();
                return;
            }

            int indexX, indexY;
            GetIndexes(eventData, out indexX, out indexY);
            var isXOK = indexX >= 0 && indexX <= _gridWidth - 1;
            var isYOK = indexY >= 0 && indexY <= _gridWidth - 1;
            if (isXOK && isYOK)
                ShowVisual(indexX, indexY);
        }

        private void GetIndexes(PointerEventData eventData, out int indexX, out int indexY)
        {
            var pos = PointerDataToRelativePos(eventData);
            var indexPosX = (pos.x - _grid.padding.left) / (_grid.cellSize.x + _grid.spacing.x);
            var indexPosY = (pos.y - _grid.padding.top) / (_grid.cellSize.y + _grid.spacing.y);
            indexX = Mathf.FloorToInt(indexPosX);
            indexY = _gridWidth - 1 - Mathf.FloorToInt(indexPosY);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            TryToPlaceMark(eventData);
            HideVisual();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateVisual(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TryToPlaceMark(eventData);
            HideVisual();
        }

        void PlaceMark(int x, int y)
        {
            _gameMode.PlayerPlay(y, x);
        }

        void ShowVisual(int x, int y)
        {
            if (_lastX == x && _lastY == y)
                return;
            _gameMode.Board.ShowVisual(y, x);
            _lastX = x;
            _lastY = y;
        }

        void HideVisual()
        {
            ResetLastPos();
            _gameMode.Board.HideVisual();
        }


        private Vector2 PointerDataToRelativePos(PointerEventData eventData)
        {
            Vector2 result;
            Vector2 clickPosition = eventData.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTrans, clickPosition, _mainCanvas.worldCamera, out result);
            result += _rectTrans.rect.size * _rectTrans.pivot;

            return result;
        }
    }
}