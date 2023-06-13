using UnityEngine;

namespace TicTacToePro
{
    [ExecuteAlways]
    public class ScaleConnectors : MonoBehaviour
    {
        public Canvas DaddyCanvas;
        public LineRenderer[] Lines;
        public LineRenderer BGLine;
        public Color BGColor = Color.black;
        public Vector2Int StartPos;
        public Vector2Int EndPos;
        public int BoardSize;
        public Color EdgeLineColor;
        public Color InnerLineColor;
        public float EdgeLineWidth = 0.25f;
        public float InnerLineWidth = 0.05f;
        public float CurrentSize = 1f;
        public RectTransform Particles;

        RectTransform _rect;
        CanvasGroup _group;
        float _width = -1f;
        float _height = -1f;
        float _visibility = -1f;
        bool _canvasActive = true;
        bool _updated = false;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _group = GetComponentInParent<CanvasGroup>();
            Init();
        }

        void OnEnable()
        {
            Init();
        }

        void OnValidate()
        {
            Init();
        }

        void Init()
        {
            _rect = GetComponent<RectTransform>();
            _group = GetComponentInParent<CanvasGroup>();
            _width = -1f;
            _height = -1f;
            _visibility = -1f;
            _canvasActive = !DaddyCanvas.enabled;
        }

        void Update()
        {
            if (!_updated)
            {
                _updated = true;
                Init();
            }

            UpdateSize();
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            if (_group.alpha == _visibility && _canvasActive == DaddyCanvas.enabled)
                return;

            _canvasActive = DaddyCanvas.enabled;

            _visibility = _group.alpha;
            var visible = _visibility;
            if (!_canvasActive)
                visible = 0f;

            for (int i = 0; i < Lines.Length; i++)
            {
                var line = Lines[i];
                SetLineColor(i, line);

                line.startColor *= visible;
                line.endColor *= visible;
            }

            BGLine.startColor = BGColor * visible;
            BGLine.endColor = BGColor * visible;
        }

        void UpdateSize()
        {
            var rectSize = _rect.rect.size;
            if (rectSize.x == _width && rectSize.y == _height)
                return;

            UpdateSizeNow();
        }

        public void UpdateSizeNow()
        {
            if (_rect == null)
                _rect = GetComponent<RectTransform>();

            var rectSize = _rect.rect.size;
            _width = rectSize.x;
            _height = rectSize.y;

            var cellSize = rectSize / BoardSize;
            var middleOffset = new Vector2(BoardSize / 2f - 0.5f, BoardSize / 2f - 0.5f);

            var centralIndexStart = StartPos - middleOffset;
            var centralIndexEnd = EndPos - middleOffset;

            var vectorToEnd = centralIndexEnd - centralIndexStart;
            centralIndexStart -= vectorToEnd.normalized * 0.3f;
            centralIndexEnd += vectorToEnd.normalized * 0.3f;

            var startPos = centralIndexStart * cellSize;
            var endPos = centralIndexEnd * cellSize;

            var vectorToEndFinal = endPos - startPos;

            endPos = startPos + vectorToEndFinal * CurrentSize;

            if (Particles != null)
            {
                MoveParticles(endPos);
            }

            for (int i = 0; i < Lines.Length; i++)
            {
                var line = Lines[i];

                var t = (float)i / (Lines.Length - 1);
                var width = Mathf.Lerp(EdgeLineWidth, InnerLineWidth, t);
                line.startWidth = width;
                line.endWidth = width;

                line.SetPosition(0, startPos);
                line.SetPosition(1, endPos);
            }

            BGLine.SetPosition(0, startPos);
            BGLine.SetPosition(1, endPos);
        }

        void MoveParticles(Vector2 pos)
        {
            Particles.anchoredPosition = pos;
        }

        private void SetLineColor(int i, LineRenderer line)
        {
            var t = (float)i / (Lines.Length - 1);
            t = t * t;
            var color = Color.Lerp(EdgeLineColor, InnerLineColor, t);
            line.startColor = color;
            line.endColor = color;
        }
    }
}