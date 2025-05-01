using SimpleWargame.BattleManagement;
using SimpleWargame.EffectSystem;
using SimpleWargame.Map;
using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.AbilitySystem
{
    /// <summary>
    /// Thunderstorm decrease range of all ranged units in the area and deals damage to few units in the area each turn 
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbilityThunderstorm", menuName = "SimpleWargame/Abilities/AbilityThunderstorm")]
    public class AbilityThunderstorm : Ability
    {
        [SerializeField] private GameObject thunderstormGO;
        [SerializeField] protected Sprite lightningSprite;

        [SerializeField] private int thunderstormDuration;
        [Header("Lightning properties")]
        [SerializeField] private int lightningChanceToSpawn;
        [SerializeField] private int lightningChanceToHit;
        [SerializeField] private int lightningNumberOfTargets;
        [SerializeField] private int lightningDamage;

        private ThunderstormGameobject thunderstorm;

        public override async void Activate(Unit unit)
        {
            base.Activate(unit);

            MapManager.Instance.UnlightAllTiles();

            //if player press ability for a second time, despawn marker
            if (thunderstorm != null)
            {
                Destroy(thunderstorm);

                BattleManager.Instance.ChangeSelectedUnit(unit);
                BattleManager.Instance.ChangeSelectedUnit(unit);

                return;
            }

            //spawn AOE-marker
            GameObject gameObject = Instantiate(thunderstormGO, unit.Player.transform);
            thunderstorm = gameObject.GetComponent<ThunderstormGameobject>();
            thunderstorm.transform.position = new Vector3 { x = 2 , y = 6, z = 0 };

            thunderstorm.SetThunderstormProperties
                (thunderstormDuration, lightningSprite, lightningChanceToSpawn, 
                lightningChanceToHit, lightningNumberOfTargets, lightningDamage);

            foreach (Effect effect in effects.Dictionary.Keys)
            {
                thunderstorm.AddEffect(effect);
            }
            //wait unitl player moves it and confirm using of ability

            await thunderstorm.SpawnConfirmed();

            //finish unit turn
            unit.EndTurn();            
        }
    }
}
