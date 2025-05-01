using SimpleWargame.AbilitySystem;
using SimpleWargame.BattleManagement;
using SimpleWargame.Map;
using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.AI
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField, Tooltip("movement cost can't be higher than this")] private int baseMovementCost = 50;

        [SerializeField, Tooltip("set it higher than baseMovementCost to increase priority of attacks")] private int baseAttackCost = 50;

        [SerializeField, Tooltip("increases difference between movement costs to make improve sub-optimal choices selection logic easier")] 
        private int movementCostMultiplier = 5;

        [SerializeField, Tooltip("how much cost of selected action can differ from optimal")] private int costRandom = 5;

        private Player player;
        private List<UnitAction> actions = new List<UnitAction>();

        public void SetPlayer(Player player)
        {
            this.player = player;
        }

        public async void DecideWhatToDo()
        {
            actions.Clear();
            actions.TrimExcess();

            if (player == null) { Debug.LogError("EnemyAI Error: player isn't set"); return; }
            if (player.Units.Count <= 0) return;

            //circle through each available unit
            foreach (Unit unit in player.Units)
            {
                if (unit.IsTurnFinished) continue;

                await FindAllAvailableActions(unit);
            }

            //get all available to this unit actions
            //nominate cost of every action
            //select one action from actions with higher cost
            //and do this action
            ExecuteUniteAction(SelectUnitAction());
        }

        private async Task FindAllAvailableActions(Unit unit)
        {
            await MapManager.Instance.SelectUnit(unit);
            List<TileData> enemyPositions = MapManager.Instance.GetEnemyPositions();

            UnitAction action;

            //get all available tiles to move
            foreach (HighlightedTile tile in MapManager.Instance.MovementArea)
            {
                action = new UnitAction();
                action.unit = unit;
                action.actionType = UnitActionType.Move;
                action.targetTile = tile.Tile;
                action.movementArea = new List<HighlightedTile>();
                foreach (HighlightedTile highlightedTile in MapManager.Instance.MovementArea) action.movementArea.Add(highlightedTile);
                action.actionCost = GetMovementCost(unit, tile.Tile, enemyPositions);
                actions.Add(action);
            }

            //get all available targets to attack
            if (await MapManager.Instance.CheckForActionPossibility(unit, unit.UnitStats.Range, MapManager.Instance.GetEnemyPositions(), false))
            {
                foreach (HighlightedTile tile in MapManager.Instance.AttackArea)
                {
                    if (tile.Tile.StandingUnit == null) { Debug.LogError("EnemyAI Error: trying to attack empty tile"); return; }

                    action = new UnitAction();
                    action.unit = unit;
                    action.actionType = UnitActionType.Attack;
                    action.targetTile = tile.Tile;
                    action.actionCost = GetAttackCost(unit, tile.Tile.StandingUnit);
                    actions.Add(action);
                }
            }

            //get all available abilities
            foreach (Ability ability in unit.Abilities)
            {
                action = new UnitAction();
                action.unit = unit;
                action.actionType = ability.UnitActionType;
                switch (ability.Target)
                {
                    case Ability.AbilityTarget.Self:
                        action.targetTile = unit.Tile;
                        break;
                    case Ability.AbilityTarget.Enemy:
                        Unit enemy = SelectEnemyAsTarget(unit);
                        if (enemy == null) { Debug.LogError("EnemyAI Error: faied to select ability target"); return; }
                        action.targetTile = enemy.Tile;
                        break;
                    case Ability.AbilityTarget.Ally:
                        Debug.Log("EnemyAI: ally as target for ability isn't implemented yet");
                        break;
                    default:
                        break;
                }
                action.actionCost = ability.AbilityAICost;
                action.ability = ability;
                actions.Add(action);
            }

            await Task.Yield();
        }

        private int GetMovementCost(Unit unit, TileData tile, List<TileData> enemyPositions)
        {
            int cost = 0;
            //we can't use just distance to get a cost, because closer to target distance decreases, but we need that cost increased
            //so we use the base movement cost out of which we subtract distance
            //also if enemy is in range of attack unit don't need to come closer
            int distanceToNearestEnemy = enemyPositions.Min(enemyPosition => Utilities.GetDistanceInTiles(tile, enemyPosition));

            int rangeModifier;
            if (distanceToNearestEnemy > unit.UnitStats.Range) rangeModifier = 0;
            else rangeModifier = unit.UnitStats.Range - distanceToNearestEnemy;

            cost = baseMovementCost - (distanceToNearestEnemy - rangeModifier) * movementCostMultiplier;

            if (cost < 0) cost = 0;

            return cost;
        }

        private int GetAttackCost(Unit unit, Unit target)
        {
            int cost = 0;

            cost = unit.UnitStats.Attack - target.UnitStats.Defense;

            if (cost < 0) cost = 0;

            cost += baseAttackCost;

            return cost;
        }

        private UnitAction SelectUnitAction()
        {
            if (actions == null) { Debug.LogError("EnemyAI Error: there is no available actions"); return new UnitAction(); }

            int maxActionCost = actions.Max(x => x.actionCost);

            List<UnitAction> unitActions = actions.FindAll(x => x.actionCost >= (maxActionCost - costRandom));

            int randomActionIndex = UnityEngine.Random.Range(0, unitActions.Count);

            return unitActions[randomActionIndex];
        }

        private Unit SelectEnemyAsTarget(Unit unit)
        {
            Unit enemy = null;

            float smallestDistance = Mathf.Infinity;
            foreach (TileData position in MapManager.Instance.GetEnemyPositions())
            {
                float distance = Vector3.Distance(position.CenterPosition, unit.Tile.CenterPosition);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    enemy = position.StandingUnit;
                }
            }

            return enemy;
        }

        private async void ExecuteUniteAction(UnitAction action)
        {
            if (BattleManager.Instance.SelectedUnit != action.unit) BattleManager.Instance.ChangeSelectedUnit(action.unit);
            //print($"Unit: {action.unit.gameObject.name}; Type: {action.actionType}; Target Tile: {action.targetTile.CenterPosition}; " +
            //    $"MovememantAreaCount: {action.movementArea?.Count}; Cost: {action.actionCost}");
            switch (action.actionType)
            {
                case UnitActionType.Move:
                    await action.unit.MoveUnit(action.targetTile, action.movementArea);

                    if (!action.unit.IsTurnFinished)
                    {
                        //it means that unit moved, but there is target to attack

                        //get all available targets
                        if (!await MapManager.Instance.CheckForActionPossibility
                            (action.unit, action.unit.UnitStats.Range, MapManager.Instance.GetEnemyPositions(), false)) break;

                        actions.Clear();
                        actions.TrimExcess();

                        foreach (HighlightedTile tile in MapManager.Instance.AttackArea)
                        {
                            if (tile.Tile.StandingUnit == null) { Debug.LogError("EnemyAI Error: trying to attack empty tile"); return; }

                            UnitAction attackAction = new UnitAction();
                            attackAction.unit = action.unit;
                            attackAction.actionType = UnitActionType.Attack;
                            attackAction.targetTile = tile.Tile;
                            attackAction.actionCost = GetAttackCost(action.unit, tile.Tile.StandingUnit);
                            actions.Add(attackAction);
                        }

                        //select target and attack it
                        ExecuteUniteAction(SelectUnitAction());
                    }
                    break;
                case UnitActionType.Attack:
                    if (action.ability != null) action.ability.Activate(action.unit);

                    await BattleManager.Instance.ExecuteCombatSequence
                        (BattleManager.Instance.SelectedUnit, action.targetTile.StandingUnit);
                    break;
                case UnitActionType.Defense:
                    action.ability.Activate(action.unit);
                    break;
                default:
                    break;
            }
        }

        public enum UnitActionType
        {
            Move,
            Attack,
            Defense
        }

        private class UnitAction
        {
            public Unit unit;
            public UnitActionType actionType;
            public TileData targetTile;
            public List<HighlightedTile> movementArea;
            public int actionCost;
            public Ability ability;
        }
    }
}
