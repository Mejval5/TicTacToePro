using System.Collections.Generic;
using UnityEngine;

namespace TicTacToePro.Game2048
{
    [CreateAssetMenu(menuName = "_Vars/2048 Settings")]
    public class Settings2048 : ScriptableObject
    {
        public List<int> StartValues;
        public List<int> PossibleValues;
        public List<Color> CubeColors;
    }
}