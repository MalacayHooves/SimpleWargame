using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    /// <summary>
    /// if unit moved, this effect adds another, which increases attack
    /// </summary>
    [CreateAssetMenu(fileName = "NewEffectChargeAttack", menuName = "SimpleWargame/Effects/EffectChargeAttack")]
    public class EffectChargeAttack : Effect
    {
        [SerializeField] private Effect chargeAttackStack;

        public override void Activate(Unit unit)
        {
            base.Activate(unit);

            //Unit.EffectWithParameters effectWithParameters = unit.EffectsWithParameters.Find(x => x.Effect == this);
            if (unit.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectChargeAttackStack))) return;

            if (chargeAttackStack == null) { Debug.LogError($"Effect {effectName} Error: there is no reference to EffectChargeAttackStack"); return; }

            unit.AddEffect(chargeAttackStack);
        }
    }
}
