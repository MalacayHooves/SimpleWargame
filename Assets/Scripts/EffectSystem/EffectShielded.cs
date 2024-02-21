using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;

namespace SimpleWargame.EffectSystem
{
    [CreateAssetMenu(fileName = "NewEffectShielded", menuName = "SimpleWargame/Effects/EffectShielded")]
    public class EffectShielded : Effect
    {
        [SerializeField, Tooltip("By how much will be increased (flat) defense against ranged attacks")] private int bonusFromEffect;

        public override (UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects) GetModifiedAttackData
            (Unit attacker, Unit enemy, UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects)
        {
            //if attacker uses ranged weapons
            if (enemy.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectRangedWeapons)))
            {
                defenseStats.Defense += bonusFromEffect;
            }

            return base.GetModifiedAttackData(attacker, enemy, attackStats, defenseStats, attackEffects);
        }
    }
}
