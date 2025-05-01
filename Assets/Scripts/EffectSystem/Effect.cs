using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    /// <summary>
    /// This class provide base Effect class
    /// </summary>
    public class Effect : ScriptableObject
    {
        [SerializeField] protected string effectName;
        public string EffectName => effectName;
        [SerializeField, TextArea(15, 20)] protected string effectDescription;
        public string EffectDescription => effectDescription;

        [SerializeField] protected Sprite effectIcon;
        public Sprite EffectSprite => effectIcon;

        [SerializeField] protected EffectActivationType activatesAt;
        public EffectActivationType ActivatesAt => activatesAt;

        /// <summary>
        /// main function to activate effect
        /// </summary>
        /// <param name="unit"></param>
        public virtual void Activate(Unit unit)
        {
            if (unit == null) { Debug.LogError($"Effect {effectName} Error: failed to activate - there is no unit"); return; }
        }

        /// <summary>
        /// disables effect
        /// </summary>
        /// <param name="unit"></param>
        public virtual void Deactivate(Unit unit)
        {
            if (unit == null) { Debug.LogError($"Effect {effectName} Error: failed to deactivate - there is no unit"); return; }
        }


        public virtual (UnitStats attackStats, UnitStats defenseStats, List<Unit.EffectWithParameters> attackEffects)
            GetModifiedAttackData(Unit attacker, Unit defender, UnitStats attackStats, UnitStats defenseStats, 
            List<Unit.EffectWithParameters> attackEffects)
        {
            if (attacker == null) { Debug.LogError($"Effect {effectName} Error: failed to ModifyAttackData - there is no attacker"); 
                return (new UnitStats(), new UnitStats(), null); }
            if (defender == null) { Debug.LogError($"Effect {effectName} Error: failed to ModifyAttackData - there is no enemy");
                return (new UnitStats(), new UnitStats(), null); }
            if (attackEffects == null) { Debug.LogError($"Effect {effectName} Error: failed to ModifyAttackData - there is no effects"); 
                return (new UnitStats(), new UnitStats(), null); }

            return (attackStats, defenseStats, attackEffects);
        }

        public enum EffectActivationType
        {
            None,
            Move,
            Attack,
            Defense,
            NewTurn
        }
    }
}
