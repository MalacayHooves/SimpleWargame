using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.BattleManagement;
using SimpleWargame.Map;
using System.Threading.Tasks;
using System;
using SimpleWargame.AbilitySystem;
using SimpleWargame.EffectSystem;
using SimpleWargame.UnitSpawn;

namespace SimpleWargame.Units
{
    /// <summary>
    /// This class provides interaction with unit and store its data
    /// </summary>
    public class Unit : MonoBehaviour
    {
        private const int DEFAULT_CHANCE_TO_HIT = 50;

        [Header("Links to Objects")]
        [SerializeField] private Sprite unitSprite;
        [SerializeField] private UnitVisuals unitVisuals;
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private UnitHealthbar unitHealthbar;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Unit Stats")]
        [SerializeField] private string unitName;
        [SerializeField, TextArea(3, 10)] private string unitDescription;
        [SerializeField] SerializableDictionary.Scripts.SerializableDictionary<Map.ENUM_TileType, MovementPenalties> movementPenalties;
        [SerializeField] UnitStats baseUnitStats;
        [SerializeField] List<Ability> abilities;
        [SerializeField] List<Effect> innateEffects;

        [Header("Variables")]
        [SerializeField, Tooltip("determines how fast unit moves between 2 tiles")] private float moveSpeed = 1f;

        public string UnitName => unitName;
        public string UnitDescription => unitDescription;
        public Sprite UnitSprite => unitSprite;
        public UnitVisuals UnitVisuals => unitVisuals;
        public Player Player { get; private set; }
        public Map.TileData Tile { get; set; }
        public bool IsTurnFinished { get; private set; }
        public bool IsUnderAttack { get; private set; }
        public bool AwaitsForAttackCommand { get; set; }
        public Ability ActivatedAbility { get; set; }

        /// <summary>
        /// list containing effects inflicted on unit with their cooldowns, number of stacked effects
        /// </summary>
        public List<EffectWithParameters> EffectsWithParameters { get; private set; } = new List<EffectWithParameters>();
        public List<Effect> InnateEffects => innateEffects;

        [Serializable]
        public class EffectWithParameters
        {
            public Effect Effect;
            public int Cooldown;
            public bool IsUnlimitedDuration;
        }
        public Dictionary<ENUM_TileType, MovementPenalties> MovementPenalties => movementPenalties.Dictionary;
        public UnitStats UnitStats => unitStats;
        public UnitStats BaseUnitStats => baseUnitStats;
        public List<Ability> Abilities => abilities;

        public event EventHandler OnHealthChanged;
        public static event EventHandler<OnClickedUnitEventsArgs> OnClickedUnit;
        public class OnClickedUnitEventsArgs: EventArgs
        {
            public Unit Unit;
        }

        private UnitStats unitStats;
        private Vector3 beginDragPosition;
        private bool isDragging;


        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) { Debug.LogError($"Unit ({gameObject.name}) Error: - there is no CanvasGroup"); return; }

            if (unitVisuals == null) unitVisuals = GetComponent<UnitVisuals>();
            if (unitVisuals == null) { Debug.LogError($"Unit ({gameObject.name}) Error: - there is no UnitAnimations"); return; }
            unitVisuals.SetUnit(this);

            if (unitMovement == null) unitMovement = GetComponent<UnitMovement>();
            if (unitMovement == null) { Debug.LogError($"Unit ({gameObject.name}) Error: there is no UnitMovementComponent"); return; }
            unitMovement.SetUnit(this);

            if (unitHealthbar == null) unitHealthbar = GetComponentInChildren<UnitHealthbar>();
            if (unitHealthbar == null) { Debug.LogError($"Unit ({gameObject.name}) Error: there is no UnitHealthbarComponent"); return; }
        }

        private void Start()
        {
            if (BattleManager.Instance != null) BattleManager.Instance.OnSelectUnit += BattleManager_OnSelectUnit;

            SetStatsDefaultValues();
            unitHealthbar.SetUnit(this);

            foreach (Effect effect in innateEffects)
            {
                if (effect == null) continue;

                AddEffect(effect, 0, true);
            }

            SetUnitActive(true);
        }

        private void OnDisable()
        {
            if (BattleManager.Instance != null) BattleManager.Instance.OnSelectUnit -= BattleManager_OnSelectUnit;
        }

        /// <summary>
        /// set units stats values equal to the base values
        /// </summary>
        public void SetStatsDefaultValues()
        {
            unitStats = baseUnitStats;
        }

        public void ClickUnit()
        {
            OnClickedUnit?.Invoke(this, new OnClickedUnitEventsArgs { Unit = this });

            if (IsUnderAttack)
            {
                BattleManager.Instance.SetTargetUnit(this);

                return;
            }

            BattleManager.Instance.ChangeSelectedUnit(this);
        }

        public void BeginDrag()
        {
            if (IsUnderAttack) return;
            if (BattleManager.Instance.ActivePlayer != Player) return;
            if (BattleManager.Instance.SelectedUnit != this) BattleManager.Instance.ChangeSelectedUnit(this);

            canvasGroup.blocksRaycasts = false;
            unitVisuals.SetUnitHalfTransparent(true);
            beginDragPosition = transform.position;
            isDragging = true;
        }

        public void OnDrag(Vector3 position)
        {
            if (IsUnderAttack) return;
            transform.position = position;
        }

        public void EndDrag()
        {
            transform.position = beginDragPosition;
            canvasGroup.blocksRaycasts = true;
            unitVisuals.SetUnitHalfTransparent(false);
            isDragging = false;
        }

        public void SetPlayer(Player player)
        { 
            Player = player;
            unitVisuals.SetPlayerColor(Player.PlayerColor);
        }

        public void ChangeUnitsStats(UnitStats newStats)
        {
            unitStats = newStats;
        }

        public void SetUnitActive(bool isActive)
        {
            unitVisuals.SetUnitActivitySpriteState(isActive);
        }

        /// <summary>
        /// restarts this unit turn, refreshesh effects on it
        /// </summary>
        public void StartNewTurn()
        {
            IsTurnFinished = false;

            //when unit is ready to move again, decrease cooldown of every effect
            //if effect cooldown is less than 1, then remove effect
            List<EffectWithParameters> effectsToRemove = new List<EffectWithParameters>();
            foreach (EffectWithParameters effect in EffectsWithParameters)
            {
                if (effect == null) continue;

                if (effect.Effect.ActivatesAt == Effect.EffectActivationType.NewTurn) effect.Effect.Activate(this);

                if (effect.IsUnlimitedDuration) continue;
                effect.Cooldown--;
                
                if (effect.Cooldown <= 0) effectsToRemove.Add(effect);
            }

            foreach (EffectWithParameters effect in effectsToRemove)
            {
                RemoveAllCopiesOfTheEffect(effect.Effect);
            }
        }

        public async Task AttackEnemy(Unit enemy, UnitStats attackStats, UnitStats defenseStats)
        {
            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;

            if (ActivatedAbility == null) await unitVisuals.PlayAttackAnimation(enemy.transform.position);
            else
            {
                await ActivatedAbility.PlayAbilityAnimation(this, enemy.transform.position);
                ActivatedAbility = null;
            }

            //foreach effect, which affect attack, modify attack stats by this effect
            //and add effects, which will be casted on the enemy to the list of effects to be send
            List<EffectWithParameters> attackEffects = new List<EffectWithParameters>();

            foreach (EffectWithParameters effect in EffectsWithParameters)
            {
                if (effect.Effect.ActivatesAt != Effect.EffectActivationType.Attack) continue;

                attackStats = effect.Effect.GetModifiedAttackData(this, enemy, attackStats, defenseStats, attackEffects).attackStats;
                defenseStats = effect.Effect.GetModifiedAttackData(this, enemy, attackStats, defenseStats, attackEffects).defenseStats;
                foreach (EffectWithParameters attackEffect in
                    effect.Effect.GetModifiedAttackData(this, enemy, attackStats, defenseStats, attackEffects).attackEffects)
                {
                    attackEffects.Add(attackEffect);
                }
                (attackStats, defenseStats, attackEffects) =
                effect.Effect.GetModifiedAttackData(this, enemy, attackStats, defenseStats, attackEffects);
            }

            if (enemy != null && enemy.gameObject != null)
                await enemy.DefendAgainstAttack(this, attackStats, defenseStats, attackEffects);

            await Task.Yield();
        }

        public async Task DefendAgainstAttack
            (Unit attacker, UnitStats attackStats, UnitStats defenseStats, List<EffectWithParameters> attackEffects)
        {
            foreach (EffectWithParameters effect in EffectsWithParameters)
            {
                if (effect.Effect.ActivatesAt != Effect.EffectActivationType.Defense) continue;

                (attackStats, defenseStats, attackEffects) = 
                    effect.Effect.GetModifiedAttackData(attacker, this, attackStats, defenseStats, attackEffects);
            }

            for (int i = 0; i < attackStats.UnitCount; i++)
            {
                if (DEFAULT_CHANCE_TO_HIT + attackStats.Attack > UnityEngine.Random.Range(1, 100) + defenseStats.Defense)
                {
                    unitStats.Health -= attackStats.Damage;
                    if (unitStats.Health <= 0)
                    {
                        unitStats.UnitCount--;
                        unitStats.Health = baseUnitStats.Health;
                    }
                }
            }

            await unitVisuals.PlayTakeDamageAnimation();

            OnHealthChanged?.Invoke(this, EventArgs.Empty);

            if (UnitStats.UnitCount <= 0)
            {
                await Die();
            }

            await Task.Yield();
        }

        /// <summary>
        /// used for cases where unit can't defend against the attack
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="numberOfAttacks"></param>
        /// <param name="chanceToHit"></param>
        
        //don't realy like to make 2 methods for the same things, need to refactor
        public async Task TakeDamage(int damage, int numberOfAttacks, int chanceToHit)
        {
            if (numberOfAttacks > unitStats.UnitCount) numberOfAttacks = unitStats.UnitCount;

            for (int i = 0; i < numberOfAttacks; i++)
            {
                if (UnityEngine.Random.Range(1, 100) > chanceToHit) continue;

                unitStats.Health -= damage;
                if (unitStats.Health <= 0)
                {
                    unitStats.UnitCount--;
                    unitStats.Health = baseUnitStats.Health;
                }
            }

            await unitVisuals.PlayTakeDamageAnimation();

            OnHealthChanged?.Invoke(this, EventArgs.Empty);

            if (UnitStats.UnitCount <= 0)
            {
                await Die();
            }
        }

        public async void FinishMovement()
        {
            if (Player == null)
            {
                AwaitsForAttackCommand = false;
                DeselectUnit();
                return;
            }
            
            List<EffectWithParameters> effectsToActivate = new List<EffectWithParameters>();
            foreach (EffectWithParameters effect in EffectsWithParameters)
            {
                if (effect.Effect.ActivatesAt != Effect.EffectActivationType.Move) continue;

                effectsToActivate.Add(effect);
            }

            foreach (EffectWithParameters effect in effectsToActivate)
            {
                effect.Effect.Activate(this);
            }

            if (baseUnitStats.AttackAfterMovement && 
                await MapManager.Instance.CheckForActionPossibility
                (this, unitStats.Range, MapManager.Instance.GetEnemyPositions(), !Player.IsUnderAIControl)) return;

            EndTurn();
        }

        public void SetUnitUnderAttack(bool isUnderAttack)
        {
            IsUnderAttack = isUnderAttack;
        }

        public void EndTurn()
        {
            DeselectUnit();
            IsTurnFinished = true;
            AwaitsForAttackCommand = false;
            SetUnitActive(false);
            if (BattleManager.Instance.IsSpawnFinished) BattleManager.Instance.SetNextActivePlayer();
        }

        public void AddEffect(Effect effect, int cooldown = 0, bool isUnlimitedDuration = false)
        {
            EffectWithParameters effectWithParameters = EffectsWithParameters.Find(x => x.Effect == effect);
            if (effectWithParameters == null)
            {
                EffectsWithParameters.Add(new EffectWithParameters 
                { Effect = effect, Cooldown = cooldown, IsUnlimitedDuration = isUnlimitedDuration });
            }
            else
            {
                effectWithParameters.Cooldown = cooldown;
            }

            effect.Activate(this);
        }

        public void RemoveAllCopiesOfTheEffect(Effect effect)
        {
            effect.Deactivate(this);
            EffectsWithParameters.RemoveAll(x => x.Effect == effect);
        }

        private async Task Die()
        {
            Player.RemoveUnitFromList(this);

            Tile.StandingUnit = null;

            await unitVisuals.PlayDeathAnimation();

            if (BattleManager.Instance != null) BattleManager.Instance.OnSelectUnit -= BattleManager_OnSelectUnit;

            Destroy(gameObject);
        }

        private void DeselectUnit()
        {
            unitVisuals.SetUnitSelectedSpriteState(false);
            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;

            BattleManager.Instance.ChangeSelectedUnit(null);
        }

        public async Task MoveUnit(TileData clickedTile, List<HighlightedTile> highlightedForMovementTiles)
        {
            await MoveToTile(clickedTile, highlightedForMovementTiles);
        }

        /// <summary>
        /// when MapManager sends data of clicked highlighted for movement tile
        /// we move unit and unsubscribe from the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MapManager_OnSendClickedTile(object sender, MapManager.OnSendClickedTileEventArgs e)
        {
            while (isDragging) await Task.Yield();
            if (e.IsHighlighted) await MoveToTile(e.ClickedTile, MapManager.Instance.MovementArea);
        }

        private async Task MoveToTile(TileData clickedTile, List<HighlightedTile> highlightedForMovementTiles)
        {
            if (Pathfinder.Instance == null) { Debug.LogError("Unit Error: Pathfinder == null"); return; }
            if (unitMovement == null) { Debug.LogError("Unit Error: UnitMovementComponent == null"); return; }

            AwaitsForAttackCommand = true;

            Dictionary<Vector3, int> path = new Dictionary<Vector3, int>();

            if (Tile == null)   //this code is needed to move unit from spawn panel to map
            {
                MoveFromSpawnPanelToMap(clickedTile, path);

                return;
            }

            foreach (HighlightedTile tile in highlightedForMovementTiles)
            {
                if (tile == null) { Debug.LogError("Unit Error: HighlightedForMovementTiles returned null tile"); return; }

                if (tile.Interceptors.Count > 0 && tile.Tile != clickedTile) continue;  //ignore tile with interceptors

                int movementExtraCost = 0;

                if (movementPenalties.Dictionary.ContainsKey(tile.Tile.TileType))
                    movementExtraCost = movementPenalties.Dictionary[tile.Tile.TileType].MovementExtraCost;

                path.Add(tile.Tile.CenterPosition, movementExtraCost);
            }

            //if there is enemy unit close by, tell it to attack this unit
            //get interceptor
            HighlightedTile highlightedTile = highlightedForMovementTiles.Find(x => x.Tile == Tile);
            Unit interceptor = null;
            if (highlightedTile != null && highlightedTile.Interceptors.Count > 0)
            {
                interceptor = highlightedTile.Interceptors[0];
            }
            //if there is one, tell it to attack this unit
            if (interceptor != null)
            {
                //wait for attack
                await interceptor.AttackEnemy(this, interceptor.UnitStats, unitStats);
            }
            //and move only after it

            //clear unit from all ground effects
            foreach (Effect effect in Tile.Effects)
            {
                RemoveAllCopiesOfTheEffect(effect);
            }

            //clear tile on which this unit stands now
            Tile.StandingUnit = null;

            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;

            await unitMovement.MoveUnit(clickedTile.CenterPosition,
                Pathfinder.Instance.GetPath(path, Tile.CenterPosition, clickedTile.CenterPosition), moveSpeed);

            //set this unit as standing on the new tile
            Tile = clickedTile;
            clickedTile.StandingUnit = this;

            //add effects from the new tile
            foreach (Effect effect in Tile.Effects)
            {
                AddEffect(effect);
            }
        }

        private async void MoveFromSpawnPanelToMap(TileData clickedTile, Dictionary<Vector3, int> path)
        {
            if (UnitSpawner.Instance == null) { Debug.LogError("Unit Error: there is no UnitSpawner"); return; }

            SetUnitActive(false);

            movementPenalties.Dictionary.TryGetValue(clickedTile.TileType, out MovementPenalties value);
            if (value.IsNotWalkable) return;

            List<Vector3> spawnPath = new List<Vector3>();
            spawnPath.Add(transform.position);
            spawnPath.Add(clickedTile.CenterPosition);

            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;

            await unitMovement.MoveUnit(clickedTile.CenterPosition, spawnPath, moveSpeed);

            UnitSpawner.Instance.MoveUnitToPlayer(this);
        }

        private void BattleManager_OnSelectUnit(object sender, EventArgs e)
        {
            if (BattleManager.Instance.SelectedUnit == this)
            {
                if (IsTurnFinished) return;

                if (Player == null || !Player.IsUnderAIControl)
                {
                    MapManager.Instance.OnSendClickedTile += MapManager_OnSendClickedTile;
                    unitVisuals.SetUnitSelectedSpriteState(true);

                    return;
                }
            }

            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;
            unitVisuals.SetUnitSelectedSpriteState(false);
        }
    }
}
