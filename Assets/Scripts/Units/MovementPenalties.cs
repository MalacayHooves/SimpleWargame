using System;
using UnityEngine;

namespace SimpleWargame.Units
{
    /// <summary>
    /// This struct contains data about penalties for unit movement
    /// </summary>
    [Serializable]
    public struct MovementPenalties
    {
        [SerializeField] private bool isNotWalkable;
        [SerializeField] private int movementExtraCost;
        public bool IsNotWalkable => isNotWalkable;
        public int MovementExtraCost => movementExtraCost;
    }
}
