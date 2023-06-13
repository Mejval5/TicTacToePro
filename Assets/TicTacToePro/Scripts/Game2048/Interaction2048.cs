using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToePro.Game2048
{
    public class Interaction2048 : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Game2048 Game;

        public void OnDrag(PointerEventData eventData)
        {
            Game.MoveCube(eventData.delta.x);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Game.Shoot();
        }
    }
}