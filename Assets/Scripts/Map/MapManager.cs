using SimpleWargame.BattleManagement;
using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleWargame.Map
{
    /// <summary>
    /// This class handles interactions between classes inside and outside of Map namespace
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [Header("Links to Objects")]
        [SerializeField] private TilemapMain tilemapMain;
        [SerializeField] private MapTilesDictionary mapTilesDictionary;
        [SerializeField] private MapHighlighter mapHighlighter;

        public event EventHandler<OnSendClickedTileEventArgs> OnSendClickedTile;
        public class OnSendClickedTileEventArgs : EventArgs
        {
            public TileData ClickedTile;
            public bool IsHighlighted;
        }

        public Tilemap Tilemap => tilemapMain.Tilemap;
        public Dictionary<Vector3Int, TileData> MapTiles => mapTilesDictionary.MapTiles;
        public List<HighlightedTile> MovementArea => mapHighlighter.MovementArea;
        public List<HighlightedTile> AttackArea => mapHighlighter.AttackArea;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (tilemapMain == null) tilemapMain = GetComponentInChildren<TilemapMain>();
            if (tilemapMain == null) { Debug.LogError($"MapManager: Error - there is no TilemapMain"); return; }

            if (mapTilesDictionary == null) mapTilesDictionary = GetComponent<MapTilesDictionary>();
            if (mapTilesDictionary == null) { Debug.LogError($"MapManager: Error - there is no MapTilesDictionary"); return; }
            mapTilesDictionary.CreateDictionary(tilemapMain.Tilemap);

            if (mapHighlighter == null) mapHighlighter = GetComponent<MapHighlighter>();
            if (mapHighlighter == null) { Debug.LogError($"MapManager: Error - there is no MapHighlighter"); return; }

            BattleManager.Instance.OnSelectUnit += BattleManager_OnSelectUnit;
        }

        public void MapClicked(Vector3 clickPosition)
        {
            //find if tile exists in main tilemap (not sure if we really need this, but let it be - there is no such things as too much checks =)
            TileData tile = mapTilesDictionary.GetTile(tilemapMain.GetTilePositionInt(clickPosition));
            if (tile == null) return;

            bool isHighlighted = false;
            if (BattleManager.Instance.IsSpawnFinished)
            {
                //find if  the tile is highlighted for movement
                HighlightedTile highlightedTile = mapHighlighter.MovementArea.Find(x => x.Tile.CenterPosition == tile.CenterPosition);
                if (highlightedTile != null) isHighlighted = true;
            }
            else
            {
                isHighlighted = true;
            }

            OnSendClickedTile?.Invoke(this, new OnSendClickedTileEventArgs { ClickedTile = tile, IsHighlighted = isHighlighted });
        }

        /// <summary>
        /// return false if there is no action available (tagets to attack or cast spells)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckForActionPossibility(Unit unit, int range, List<TileData> targetPositions, bool highlightEnemyPositions = true)
        {
            await mapHighlighter.UnlightAllTiles();
            bool boolToReturn = false;

            if (targetPositions != null)
            {
                boolToReturn = mapHighlighter.TryToFindTargetsForAction(unit, range, targetPositions);

                if (highlightEnemyPositions) mapHighlighter.HighlightMap();
            }

            return boolToReturn;
        }

        public async Task SelectUnit(Unit unit)
        {
            //await mapHighlighter.UnlightAllTiles();

            await CheckForActionPossibility(unit, unit.UnitStats.Range, GetEnemyPositions(), false);

            await mapHighlighter.SetMovementArea(unit, unit.UnitStats.Speed, unit.MovementPenalties, GetEnemyPositions());
        }

        public List<TileData> GetEnemyPositions()
        {
            List<TileData> enemyPositions = new List<TileData>();

            foreach (Player player in BattleManager.Instance.Players)
            {
                if (player == BattleManager.Instance.ActivePlayer) continue;
                if (player.Units == null || player.Units.Count <= 0) continue;
                foreach (Unit unit in player.Units)
                {
                    enemyPositions.Add(unit.Tile);
                }
            }

            return enemyPositions;
        }

        public List<TileData> GetFriendlyPositions()
        {
            List<TileData> freindlyPositions = new List<TileData>();

            foreach (Player player in BattleManager.Instance.Players)
            {
                if (player != BattleManager.Instance.ActivePlayer) continue;
                if (player.Units == null || player.Units.Count <= 0) continue;
                foreach (Unit unit in player.Units)
                {
                    freindlyPositions.Add(unit.Tile);
                }
            }

            return freindlyPositions;
        }

        public async void UnlightAllTiles()
        {
            await mapHighlighter.UnlightAllTiles();
        }

        private async void BattleManager_OnSelectUnit(object sender, EventArgs e)
        {
            UnlightAllTiles();
            if (BattleManager.Instance.SelectedUnit == null) return;
            if (BattleManager.Instance.SelectedUnit.Player != BattleManager.Instance.ActivePlayer) return;

            if (BattleManager.Instance.SelectedUnit.Player == null) return;
            if (BattleManager.Instance.ActivePlayer.IsUnderAIControl) return;
            if (BattleManager.Instance.SelectedUnit.IsTurnFinished) return;

            await SelectUnit(BattleManager.Instance.SelectedUnit);

            mapHighlighter.HighlightMap();
        }
    }
}
