using SimpleWargame.Units;
using SimpleWargame.EffectSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.AbilitySystem
{
    /// <summary>
    /// This is brace ability. Every unit has it. It increases unit defense until the next turn, and ends unit turn
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbilityBrace", menuName = "SimpleWargame/Abilities/AbilityBrace")]
    public class AbilityBrace : Ability
    {
        public override async void Activate(Unit unit)
        {
            base.Activate(unit);

            foreach (Effect effect in effects.Dictionary.Keys) unit.AddEffect(effect, effects.Dictionary[effect]);

            await unit.UnitVisuals.PlayDefenseAnimation();

            unit.EndTurn();
        }
    }
}
