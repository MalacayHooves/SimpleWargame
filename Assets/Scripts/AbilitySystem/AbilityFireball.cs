using SimpleWargame.BattleManagement;
using SimpleWargame.EffectSystem;
using SimpleWargame.Map;
using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.AbilitySystem
{
    /// <summary>
    /// Fireball deals moderate damage to multiple entities in a single unit
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbilityFireball", menuName = "SimpleWargame/Abilities/AbilityFireball")]
    public class AbilityFireball : Ability
    {
        [SerializeField] private int abilityRange;
        [SerializeField] private int abilityTargetsNumber;
        [SerializeField] private int abilityDamage;
        [SerializeField] private float animationSpeed;
        [SerializeField] private GameObject animationPrefab;
        [SerializeField] private Effect rangeAttackEffect;

        public override async void Activate(Unit unit)
        {
            base.Activate(unit);

            //if ability button is pressed for the second time
            //then we deactivate ability
            if (unit.ActivatedAbility != null)
            {
                unit.AwaitsForAttackCommand = false;
                unit.ActivatedAbility = null;
                BattleManager.Instance.ChangeSelectedUnit(unit);    //two times because it's an easy way
                BattleManager.Instance.ChangeSelectedUnit(unit);    //to reselect unit
                return;
            }

            unit.ActivatedAbility = this;

            //highlight enemies
            if (!await MapManager.Instance.CheckForActionPossibility
                (unit, abilityRange, MapManager.Instance.GetEnemyPositions(), !unit.Player.IsUnderAIControl)) return;

            //wait for input
            unit.AwaitsForAttackCommand = true;

            //when enemy unit clicked attack it with fireball-specific attack profile
            while (BattleManager.Instance.TargetUnit == null) await Task.Yield();
            unit.AddEffect(rangeAttackEffect);
            await BattleManager.Instance.ExecuteCombatSequence
                (BattleManager.Instance.SelectedUnit, BattleManager.Instance.TargetUnit);
            unit.RemoveAllCopiesOfTheEffect(rangeAttackEffect);
        }

        public override UnitStats GetAttackStatsModifiedByAbility(UnitStats attackStats)
        {
            attackStats.Range = abilityRange;
            attackStats.Damage = abilityDamage;
            attackStats.UnitCount = abilityTargetsNumber;

            return base.GetAttackStatsModifiedByAbility(attackStats);
        }

        public override async Task PlayAbilityAnimation(Unit unit, Vector3 targetPosition)
        {
            GameObject fireball = Instantiate(animationPrefab, unit.transform);

            fireball.transform.position = unit.transform.position;

            Quaternion rotation = Quaternion.LookRotation(targetPosition - unit.transform.position, Vector3.right);
            fireball.transform.rotation = rotation;

            Vector3 startPosition = fireball.transform.position;
            float startTime = Time.time;
            float moveDistance = Vector3.Distance(fireball.transform.position, targetPosition);
            float distanceCovered = 0;

            while (distanceCovered < moveDistance)
            {
                distanceCovered = (Time.time - startTime) * animationSpeed;
                fireball.transform.position = Vector3.Lerp(startPosition, targetPosition, distanceCovered / moveDistance);

                await Task.Yield();
            }

            Destroy(fireball);
        }
    }
}
