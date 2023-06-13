using UnityEngine;

namespace TicTacToePro
{
    public class VisualizeGaussDistribution : MonoBehaviour
    {
        public GameObject Point;
        public Vector2 MainOffset = new Vector2(-500f, -1000f);
        public int Range = 11;
        public float Offset = 50f;
        public float OffsetY = 20f;
        public int PointCount = 1000;

        int[] _rangeArray;

        void Start()
        {
            _rangeArray = new int[Range];
            for (int i = 0; i < PointCount; i++)
            {
                var x = Mathf.RoundToInt(HM.RandomGaussian(0, Range - 1));
                var xPos = x * Offset + MainOffset.x;
                var yPos = _rangeArray[x] * OffsetY + MainOffset.y;
                _rangeArray[x] += 1;
                var point = Instantiate(Point, transform);
                point.SetActive(true);
                point.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            }
        }
    }
}