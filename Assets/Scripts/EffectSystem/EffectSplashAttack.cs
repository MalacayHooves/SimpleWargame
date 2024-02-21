using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    [CreateAssetMenu(fileName = "NewEffectSplashAttack", menuName = "SimpleWargame/Effects/EffectSplashAttack")]
    public class EffectSplashAttack : Effect
    {
        [SerializeField, Tooltip("How many targets will be hit by each entity in the unit")] private int targetsHitByEntityAttack;

        public override (UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects) GetModifiedAttackData
            (Unit attacker, Unit defender, UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects)
        {
            if (defenseStats.UnitCount >= targetsHitByEntityAttack)
            {
                attackStats.UnitCount *= targetsHitByEntityAttack;
            }
            else
            {
                attackStats.UnitCount *= defenseStats.UnitCount;
            }

            return base.GetModifiedAttackData(attacker, defender, attackStats, defenseStats, attackEffects);
        }
    }
}
