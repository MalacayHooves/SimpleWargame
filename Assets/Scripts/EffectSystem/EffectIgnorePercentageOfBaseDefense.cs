using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    [CreateAssetMenu(fileName = "NewEffectIgnorePercentageOfBaseDefense", menuName = "SimpleWargame/Effects/EffectIgnorePercentageOfBaseDefense")]
    public class EffectIgnorePercentageOfBaseDefense : Effect
    {
        [SerializeField, Range(0, 100)] private int percentage;

        public override (UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects) GetModifiedAttackData
            (Unit attacker, Unit defender, UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects)
        {
            if (percentage > 0) defenseStats.Defense = defenseStats.Defense - (defenseStats.Defense / percentage * 100);

            return base.GetModifiedAttackData(attacker, defender, attackStats, defenseStats, attackEffects);
        }
    }
}
