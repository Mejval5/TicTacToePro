using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TicTacToePro
{
    public class OnTouch : MonoBehaviour
    {
        public UnityEvent Clicked = new UnityEvent();

        void OnMouseDown()
        {
            Clicked.Invoke();
        }
    }
}
