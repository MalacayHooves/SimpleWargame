using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;
using System.Threading.Tasks;
using SimpleWargame.Map;
using SimpleWargame.AI;

namespace SimpleWargame.BattleManagement
{
    /// <summary>
    /// Contains list of their units
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private Color playerColor = Color.black;
        [SerializeField] private EnemyAI enemyAI;
        public Color PlayerColor => playerColor;

        private List<Unit> units;
        public List<Unit> Units => units;

        public bool IsTurnFinished { get; private set; }
        public bool IsUnderAIControl => enemyAI != null;

        private void Awake()
        {
            IsTurnFinished = false;

            if (enemyAI != null) enemyAI.SetPlayer(this);
        }

        public void SetPlayerActive()
        {            
            foreach (Unit unit in units)
            {
                if (!unit.IsTurnFinished)
                {
                    unit.SetUnitActive(true);
                }
            }

            if (enemyAI == null) return;
            enemyAI.DecideWhatToDo();
        }

        public void SetPlayerInactive()
        {
            bool isAnyUnitActive = false;
            foreach (Unit unit in units)
            {
                unit.SetUnitActive(false);

                if (!unit.IsTurnFinished)
                {
                    isAnyUnitActive = true;
                }
            }

            if (!isAnyUnitActive) IsTurnFinished = true;
        }

        public async Task StartNewTurn()
        {
            IsTurnFinished = false;
            foreach (Unit unit in units) { unit.StartNewTurn(); }

            await Task.Yield();
        }

        public void RemoveUnitFromList(Unit unit)
        {
            units.Remove(unit);

            if (units.Count <= 0)
            {
                BattleManager.Instance.FinishBattle(this);
            }
        }

        public void SetUnitsStartPositions()
        {
            units = new List<Unit>();
            Unit[] unitArray = GetComponentsInChildren<Unit>();
            foreach (Unit unit in unitArray)
            {
                units.Add(unit);
            }

            foreach (Unit unit in units)
            {
                Vector3Int position = MapManager.Instance.Tilemap.WorldToCell(unit.transform.position);

                if (!MapManager.Instance.MapTiles.ContainsKey(position))
                {
                    Debug.LogError($"Player ({gameObject.name}): There is no tile with this position!");
                    return;
                }
                Map.TileData tile = MapManager.Instance.MapTiles[position];

                //set unit position perfectly equal to tile position in case if unit position is slightly off
                unit.transform.position = tile.CenterPosition;

                tile.StandingUnit = unit;
                unit.Tile = tile;
                unit.SetPlayer(this);
            }
        }
    }
}
