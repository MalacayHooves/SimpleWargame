using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    [CreateAssetMenu(fileName = "NewEffectIncreasedDefense", menuName = "SimpleWargame/Effects/EffectIncreasedDefense")]
    public class EffectIncreasedDefense : Effect
    {
        [SerializeField] private int defenseBonus;

        public override void Activate(Unit unit)
        {
            base.Activate(unit);

            UnitStats unitStats = unit.UnitStats;

            unitStats.Defense += defenseBonus;

            unit.ChangeUnitsStats(unitStats);
        }

        public override void Deactivate(Unit unit)
        {
            base.Deactivate(unit);
            UnitStats unitStats = unit.UnitStats;

            unitStats.Defense -= defenseBonus;

            unit.ChangeUnitsStats(unitStats);
        }
    }
}
