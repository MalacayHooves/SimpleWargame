using SerializableDictionary.Scripts;
using SimpleWargame.EffectSystem;
using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.AbilitySystem
{
    /// <summary>
    /// This class provide base Ability class abilities can change unit stats directly (heal, damage dealing...) 
    /// or enable effects (which buff or debuff units)
    /// </summary>
    public class Ability : ScriptableObject
    {
        [SerializeField] protected string abilityName;
        public string AbilityName => abilityName;
        [SerializeField, TextArea(15, 20)] protected string abilityDescription;
        public string AbilityDescription => abilityDescription;

        [SerializeField] protected Sprite abilityIcon;
        public Sprite AbilityIcon => abilityIcon;

        /// <summary>
        /// serializable dictionary containing effects which ability inflicts (as keys) and their cooldowns (values)
        /// </summary>
        [SerializeField, Tooltip("serializable dictionary containing effects which ability inflicts (as keys) and their cooldowns (values)")] 
        protected SerializableDictionary<Effect, int> effects = new SerializableDictionary<Effect, int>();

        [SerializeField] protected AI.EnemyAI.UnitActionType unitActionType;
        public AI.EnemyAI.UnitActionType UnitActionType => unitActionType;

        [SerializeField] private AbilityTarget target;
        public AbilityTarget Target => target;
        [SerializeField] private int abilityAICost;
        public int AbilityAICost => abilityAICost;

        public virtual void Activate(Unit unit)
        {
            if (unit == null) { Debug.LogError($"Ability {abilityName} Error: failed to activate - there is no unit"); return; }
        }

        public virtual UnitStats GetAttackStatsModifiedByAbility(UnitStats attackStats)
        {
            return attackStats;
        }

        public virtual async Task PlayAbilityAnimation(Unit unit, Vector3 targetPosition)
        {
            await Task.Yield();
        }

        public enum AbilityTarget
        {
            None,
            Self,
            Enemy,
            Ally
        }
    }
}
