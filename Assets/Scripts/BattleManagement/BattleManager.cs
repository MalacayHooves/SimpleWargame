using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;
using SimpleWargame.Map;
using System.Threading.Tasks;
using SimpleWargame.UnitUI;
using SimpleWargame.EffectSystem;

namespace SimpleWargame.BattleManagement
{
    /// <summary>
    /// This class keeps info about active player, selected unit, manages turn order
    /// </summary>
    public class BattleManager : MonoBehaviour, ICanSendSelectedUnit
    {
        public static BattleManager Instance { get; private set; }

        [Header("Links to Objects")]
        [SerializeField] private List<Player> players;
        [SerializeField] private GameObject indicatorOfAITurnPanel;

        public event EventHandler<EventArgs> OnStartNewTurn;

        public List<Player> Players => players;

        private Unit selectedUnit;
        public Unit SelectedUnit { get => selectedUnit; }

        private Unit targetUnit;
        public Unit TargetUnit { get => targetUnit; }

        public event EventHandler OnSelectUnit;

        public Player ActivePlayer { get; private set; }
        public bool IsSpawnFinished { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (players.Count <= 0) { Debug.LogError("BattleManager: Error - there is no players"); return; }

            indicatorOfAITurnPanel.SetActive(false);
        }

        private void OnDisable()
        {
            UnsubscribeUIFromManagerEvents();
        }

        public void UnsubscribeUIFromManagerEvents()
        {
            UnitUI.UnitUI unitUI = MonoBehaviour.FindAnyObjectByType<UnitUI.UnitUI>();
            if (unitUI != null) unitUI.UnsubscribeFromManagerEvents();
        }

        /// <summary>
        /// sets clicked unit as selected, or deselects it if it's already selected. 
        /// If it's unit of an active player, MapManager will highlight this unit movement and attacks
        /// </summary>
        public void ChangeSelectedUnit(Unit clickedUnit)
        {
            if (selectedUnit != null && selectedUnit.AwaitsForAttackCommand) return;
            if (UnitVisuals.IsAwaitingForAnimations) return;

            if (selectedUnit == clickedUnit) selectedUnit = null;
            else selectedUnit = clickedUnit;

            OnSelectUnit?.Invoke(this, EventArgs.Empty);
        }

        public async void SetTargetUnit(Unit clickedUnit)
        {
            if (UnitVisuals.IsAwaitingForAnimations) { Debug.LogError("BattleManager AttackUnit Error: wait for animation to finish"); return; }
            if (selectedUnit == null) { Debug.LogError("BattleManager AttackUnit Error: there is no selected unit"); return; }

            targetUnit = clickedUnit;

            if (selectedUnit.ActivatedAbility == null)
            {
                await ExecuteCombatSequence(SelectedUnit, TargetUnit);
            }
        }

        public async void SetNextActivePlayer()
        {
            if (UnitVisuals.IsAwaitingForAnimations) { Debug.LogError("BattleManager SetNextActivePlayer Error: wait for animation to finish"); return; }
            if (ActivePlayer != null) ActivePlayer.SetPlayerInactive();

            ChangeSelectedUnit(null);
            targetUnit = null;

            Player newActivePlayer = players.Find(x => x.IsTurnFinished == false && x != ActivePlayer);

            if (newActivePlayer == null)
            {
                if (!ActivePlayer.IsTurnFinished) newActivePlayer = ActivePlayer;
            }

            if (newActivePlayer == null)
            {
                List<Task> tasks = new List<Task>();

                foreach (Player p in players)
                {
                    tasks.Add(p.StartNewTurn());
                }

                await Task.WhenAll(tasks);

                newActivePlayer = players.Find(x => x.IsTurnFinished == false);

                OnStartNewTurn?.Invoke(this, EventArgs.Empty);
            }

            ActivePlayer = newActivePlayer;

            if (ActivePlayer == null) { Debug.LogError("BattleManager Error: there is no Active Player"); return; }
            ActivePlayer.SetPlayerActive();
            if (ActivePlayer.IsUnderAIControl) indicatorOfAITurnPanel.SetActive(true);
            else indicatorOfAITurnPanel.SetActive(false);
        }

        public void FinishUnitSpawn()
        {
            IsSpawnFinished = true;
            foreach (Player player in BattleManager.Instance.Players)
            {
                player.SetUnitsStartPositions();
            }

            SetNextActivePlayer();
        }

        public void FinishBattle(Player loser)
        {
            if (loser == players[0])
            {
                BattleFinishedUI.Instance.ShowFinishBattleScreen("You lost!");
            }
            else
            {
                BattleFinishedUI.Instance.ShowFinishBattleScreen("You won!");
            }
        }

        public async Task ExecuteCombatSequence(Unit attacker, Unit defender)
        {
            UnitStats attackStats = attacker.UnitStats;
            UnitStats defenseStats = defender.UnitStats;

            //check if it's normal attack or use of ability
            if (attacker.ActivatedAbility != null)
            {
                attackStats = attacker.ActivatedAbility.GetAttackStatsModifiedByAbility(attackStats);
            }

            //decide will it be one-sided attack or two-side combat
            //if attacker uses melee weapons
            if (!attacker.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectRangedWeapons)))
            {
                //then it's two-sided
                //decide who will attack first

                //if defender has Long Weapons
                if (defender.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectLongWeapons)))
                {
                    //then defender is first to attack
                    await defender.AttackEnemy(attacker, defenseStats, attackStats);

                    if (attacker != null) await attacker.AttackEnemy(defender, attackStats, defenseStats);
                }
                else
                {
                    await attacker.AttackEnemy(defender, attackStats, defenseStats);
                    if (defender != null) await defender.AttackEnemy(attacker, defenseStats, attackStats);
                }
            }
            else
            {
                await attacker.AttackEnemy(defender, attackStats, defenseStats);
            }

            if (attacker != null) attacker.EndTurn();
            else
            {
                BattleManager.Instance.ChangeSelectedUnit(null);
                BattleManager.Instance.SetNextActivePlayer();
            }

            await Task.Yield();
        }
    }
}
