using System;
using UnityEngine;

namespace TicTacToePro.Pooling
{
        [CreateAssetMenu(menuName = "_Vars/PooledObject")]
        [Serializable] 
        public class PooledObject : ScriptableObject
        {
                public int amountToPool;
                public GameObject objectToPool;
                public bool shouldExpand;
        }
}