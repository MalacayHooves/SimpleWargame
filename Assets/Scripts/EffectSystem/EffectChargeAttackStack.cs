using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    /// <summary>
    /// increases attack if enemy has no charge defense
    /// </summary>
    [CreateAssetMenu(fileName = "NewEffectChargeAttackStack", menuName = "SimpleWargame/Effects/EffectChargeAttackStack")]
    public class EffectChargeAttackStack : Effect
    {
        [SerializeField] private int attackBonus;

        public override (UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects) GetModifiedAttackData
            (Unit attacker, Unit defender, UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects)
        {
            if (defender.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectChargeDefense)))
                return (attackStats, defenseStats, attackEffects);

            attackStats.Attack += attackBonus;

            return base.GetModifiedAttackData(attacker, defender, attackStats, defenseStats, attackEffects);
        }
    }
}
