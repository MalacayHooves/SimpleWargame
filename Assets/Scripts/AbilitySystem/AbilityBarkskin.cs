using SimpleWargame.Map;
using SimpleWargame.Units;
using SimpleWargame.BattleManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SimpleWargame.EffectSystem;

namespace SimpleWargame.AbilitySystem
{
    /// <summary>
    /// Increases defense of a friendly units for a few turns
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbilityBarkskin", menuName = "SimpleWargame/Abilities/AbilityBarkskin")]
    public class AbilityBarkskin : Ability
    {
        [SerializeField] private GameObject barkskinGameObject;

        [SerializeField] private int abilityRange;

        public override async void Activate(Unit unit)
        {
            base.Activate(unit);

            if (unit.ActivatedAbility != null)
            {
                unit.AwaitsForAttackCommand = false;
                unit.ActivatedAbility = null;
                BattleManager.Instance.ChangeSelectedUnit(unit);    //two times because it's an easy way
                BattleManager.Instance.ChangeSelectedUnit(unit);    //to reselect unit
                return;
            }

            unit.ActivatedAbility = this;

            //highlight friendly units
            await MapManager.Instance.CheckForActionPossibility(unit, abilityRange, MapManager.Instance.GetFriendlyPositions(), true);
            //when friendly unit clicked play animation
            while (BattleManager.Instance.TargetUnit == null) await Task.Yield();

            GameObject gameObject = Instantiate(barkskinGameObject, unit.transform);
            AbilityGameObject barkskin = gameObject.GetComponent<AbilityGameObject>();
            barkskin.transform.position = BattleManager.Instance.TargetUnit.transform.position;
            barkskin.SetSprite(AbilityIcon);
            await barkskin.PlayAnimationAndDestroySelf();
            //and put effect on the unit
            foreach (Effect effect in effects.Dictionary.Keys)
            {
                BattleManager.Instance.TargetUnit.AddEffect(effect, effects.Dictionary[effect]);
            }

            unit.EndTurn();
        }
    }
}
