using System;
using UnityEngine;

namespace SimpleWargame.Units
{
    /// <summary>
    /// This struct contains information about unit stats
    /// </summary>
    [Serializable]
    public struct UnitStats
    {
        public int Speed;

        [Tooltip("number of entities in the unit")] public int UnitCount;

        [Tooltip("health of the \"first\" entity in the unit (like in Heroes of Might and Magic)")] public int Health;

        [Tooltip("damage of one entity in the unit")] public int Damage;

        [Range(1, 100), Tooltip("attack of the unit, increases chances to hit an enemy")] public int Attack;

        [Range(1, 100), Tooltip("defense of the unit, decreases chances to be hit by enemy")] public int Defense;

        [Tooltip("range of attack of the unit")] public int Range;

        [Tooltip("not showed in UI; true, if unit can't attack after it moved")] public bool AttackAfterMovement;

        [Tooltip("how much unit costs when player selects them before battle")] public int UnitCost;
    }
}
