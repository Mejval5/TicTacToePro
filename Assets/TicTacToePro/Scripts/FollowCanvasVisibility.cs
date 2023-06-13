using UnityEngine;

namespace TicTacToePro
{
    [ExecuteAlways]
    public class FollowCanvasVisibility : MonoBehaviour
    {
        public Canvas DaddyCanvas;
        public CanvasGroup DaddyCanvasGroup;


        CanvasGroup _canvasGroup;
        Canvas _canvas;

        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        void Update()
        {
            if (DaddyCanvasGroup != null)
            {
                _canvasGroup.alpha = DaddyCanvasGroup.alpha;
            }

            if (_canvas != null)
            {
                if (!DaddyCanvas.enabled && _canvas.enabled)
                    _canvas.enabled = false;
                if (DaddyCanvas.enabled && !_canvas.enabled)
                    _canvas.enabled = true;
            }

        }
    }
}