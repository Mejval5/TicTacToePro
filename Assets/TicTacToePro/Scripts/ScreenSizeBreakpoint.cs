using UnityEngine;
using UnityEngine.UI;

namespace TicTacToePro
{
    [ExecuteAlways]
    [RequireComponent(typeof(LayoutElement))]
    public class ScreenSizeBreakpoint : MonoBehaviour
    {
        public float BreakPoint;
        public bool Invert;

        LayoutElement _layoutElement;

        Vector2Int _lastRes;
        float _lastBreakPoint;
        bool _lastInvert;

        void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            _layoutElement = GetComponent<LayoutElement>();
            UpdateToggle();
        }

        void OnValidate()
        {
            TryToggle();
        }

        // Update is called once per frame
        void Update()
        {
            TryToggle();
        }

        void TryToggle()
        {
            if (_lastRes.x != Screen.width || _lastRes.y != Screen.height || _lastBreakPoint != BreakPoint || _lastInvert != Invert)
                UpdateToggle();
        }


        void UpdateToggle()
        {
            if (_layoutElement == null)
                _layoutElement = GetComponent<LayoutElement>();

            _lastInvert = Invert;
            _lastBreakPoint = BreakPoint;
            _lastRes = new Vector2Int(Screen.width, Screen.height);
            var screenAspect = _lastRes.y / (float)_lastRes.x;
            if (screenAspect < BreakPoint && (!_layoutElement.ignoreLayout || Invert))
            {
                _layoutElement.ignoreLayout = !Invert;
            }

            if (screenAspect > BreakPoint && (_layoutElement.ignoreLayout || Invert))
            {
                _layoutElement.ignoreLayout = Invert;
            }
        }
    }
}