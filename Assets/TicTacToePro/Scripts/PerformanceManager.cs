using UnityEngine;

namespace TicTacToePro
{
    public class PerformanceManager : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}