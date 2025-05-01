using SimpleWargame.BattleManagement;
using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleWargame.Map
{
    /// <summary>
    /// This class highlights specific tiles and contains list of this tiles
    /// </summary>
    public class MapHighlighter : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private TilemapHighlight tilemapHighlight;
        [SerializeField] private TileBase highlightRuleTile;

        [Header("Tile colors")]
        [SerializeField] private Color deselectedColor = Color.white;
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color interceptionColor = Color.white;
        [SerializeField] private Color attackZoneColor = Color.white;

        private List<HighlightedTile> movementArea = new List<HighlightedTile>();
        public List<HighlightedTile> MovementArea => movementArea;
        private List<HighlightedTile> attackArea = new List<HighlightedTile>();
        public List<HighlightedTile> AttackArea => attackArea;

        private List<HighlightedTile> queuedTiles = new List<HighlightedTile>();
        private Dictionary<Map.ENUM_TileType, Units.MovementPenalties> movementPenalties = new Dictionary<ENUM_TileType, Units.MovementPenalties>();
        private List<TileData> targetPositions = new List<TileData>();
        private TileData startingTile;

        private Vector3Int[] neighbouringTilesOffset;


        private void Start()
        {
            if (tilemapHighlight == null) tilemapHighlight = GetComponentInChildren<TilemapHighlight>();
            if (tilemapHighlight == null) { Debug.LogError($"MapManager: Error - there is no tilemapHighlight"); return; }

            SetNeighbouringTilesOffset();
        }

        public bool TryToFindTargetsForAction
            (Unit unit, int range, List<TileData> targetPositions)
        {
            if (targetPositions.Count <= 0) return false;
            this.targetPositions = targetPositions;
            this.startingTile = unit.Tile;

            foreach (TileData tileData in targetPositions)
            {
                if (Utilities.GetDistanceInTiles(unit.Tile, tileData) > range) continue;

                SetHighlightedState(new HighlightedTile 
                { Tile = tileData, Distance = range, Interceptors = new List<Unit>() }, ENUM_TileHighlightState.AttackZone);
            }

            //await MoveAttackQueue(unit);

            if (attackArea.Count > 0) return true;
            else return false;
        }

        public async Task SetMovementArea
            (Unit unit, int speed, 
            Dictionary<ENUM_TileType, MovementPenalties> movementPenalties, List<TileData> enemyPositions)
        {
            this.movementPenalties = movementPenalties;
            this.targetPositions = enemyPositions;
            this.startingTile = unit.Tile;
            queuedTiles.Add(
                new HighlightedTile { Tile = startingTile, Distance = speed, Interceptors = new List<Unit>() });
            await MoveMovementQueue(unit);
        }

        public void HighlightMap()
        {
            if (movementArea != null && movementArea.Count > 0)
            {
                foreach (HighlightedTile tile in movementArea)
                {
                    HighlightTile(tile);
                }
            }

            if (attackArea != null && attackArea.Count > 0)
            {
                foreach(HighlightedTile tile in attackArea)
                {
                    HighlightTile(tile);
                }
            }
        }

        public async Task UnlightAllTiles()
        {
            if (queuedTiles != null)
            {
                queuedTiles.Clear();
                queuedTiles.TrimExcess();
            }

            if (movementArea != null)
            {
                foreach (HighlightedTile tile in movementArea)
                {
                    SetHighlightedState(tile, ENUM_TileHighlightState.NotHighlighted);
                    HighlightTile(tile);
                }
                movementArea.Clear();
                movementArea.TrimExcess();
            }

            if (attackArea != null)
            {
                foreach (HighlightedTile tile in attackArea)
                {
                    SetHighlightedState(tile, ENUM_TileHighlightState.NotHighlighted);
                    HighlightTile(tile);
                }
                attackArea.Clear();
                attackArea.TrimExcess();

                targetPositions.Clear();
                targetPositions.TrimExcess();
            }

            await Task.Yield();
        }

        private async Task MoveMovementQueue(Unit unit)
        {
            while (queuedTiles.Any())
            {
                //highlight first queued tile
                SelectTileHighlightForMovement(unit, queuedTiles[0]);

                //if there is movement distance left then add all of it's neighbours to the queue and set their movement distances
                if (queuedTiles[0].Distance > 0)
                    SetNeighboursMovementDistance(unit, queuedTiles[0], queuedTiles[0].Distance - 1);

                //remove first item from queue
                queuedTiles.RemoveAt(0);
            }

            await Task.Yield();
        }

        /// <summary>
        /// select type of highlight depending on movement distance and penalties
        /// </summary>
        /// <param name="tileToHighlight"></param>
        private void SelectTileHighlightForMovement(Unit unit, HighlightedTile tileToHighlight)
        {
            //if it's in range of movement (unit can always move on a tile next to it)
            if (tileToHighlight.Distance < 0 && (Utilities.GetDistanceInTiles(unit.Tile, tileToHighlight.Tile) > 1)) return;
            //and walkable area
            if (unit.MovementPenalties.
                TryGetValue(tileToHighlight.Tile.TileType, out MovementPenalties value) && value.IsNotWalkable)
                return;
            //and there is no other unit
            //except the unit we are want to move, because we need to check for interceptors for it
            if (tileToHighlight.Tile.StandingUnit != null && tileToHighlight.Tile.StandingUnit != unit) return;

            //and if there is enemies around, then set interception highlight
            if (targetPositions.Count > 0)
            {
                foreach (Vector3Int offset in neighbouringTilesOffset)
                {
                    //if tile exists
                    if (!MapManager.Instance.MapTiles.TryGetValue(tileToHighlight.Tile.CenterPosition + offset, out TileData neighbouringTile)) continue;

                    //and there is a unit
                    if (neighbouringTile.StandingUnit == null) continue;

                    //and it's an enemy
                    if (neighbouringTile.StandingUnit.Player == BattleManager.Instance.ActivePlayer) continue;

                    tileToHighlight.Interceptors.Add(neighbouringTile.StandingUnit);
                }
            }

            if (tileToHighlight.Interceptors.Count > 0)
            {
                SetHighlightedState(tileToHighlight, ENUM_TileHighlightState.InterceptionZone);
                return;
            }

            //and there is no unit, then set movement highlight
            SetHighlightedState(tileToHighlight, ENUM_TileHighlightState.MovementHighlight);
        }

        /// <summary>
        /// add neighbouring tiles to queue and set their movement distances
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="movementDistance"></param>
        private async void SetNeighboursMovementDistance(Unit unit, HighlightedTile highlightedTile, int movementDistance)
        {
            if (highlightedTile.Interceptors.Count > 0 && highlightedTile.Tile != startingTile) return;
            if (highlightedTile.Tile.StandingUnit != null && highlightedTile.Tile != startingTile) return;
            foreach (Vector3Int offset in neighbouringTilesOffset)
            {
                //if tile exists
                if (!MapManager.Instance.MapTiles.TryGetValue(highlightedTile.Tile.CenterPosition + offset, out TileData neighbouringTile)) continue;
                //and we haven't added it to queue
                if (queuedTiles.Exists(x => x.Tile == neighbouringTile)) continue;
                //and we haven't highlighted it already
                if (movementArea.Exists(x => x.Tile == neighbouringTile)) continue;
                if (attackArea.Exists(x => x.Tile == neighbouringTile)) continue;
                //if it's not starting tile
                if (neighbouringTile == startingTile) continue;

                //then we add it to the queue
                queuedTiles.Add(new HighlightedTile { Tile = neighbouringTile,
                    Distance = GetTileMovementDistance(unit, neighbouringTile, movementDistance ), Interceptors = new List<Units.Unit>()});
            }
            await Task.Yield();
        }

        /// <summary>
        /// get movement distance for this tile, considering movement penalties (if it isn't starting tile)
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="movementDistance"></param>
        private int GetTileMovementDistance(Unit unit, TileData tile, int movementDistance)
        {
            int movementExtraCost = 0;
            unit.MovementPenalties.TryGetValue(tile.TileType, out Units.MovementPenalties value);
            movementExtraCost = value.MovementExtraCost;
            movementDistance -= movementExtraCost;

            return movementDistance;
        }

        private void SetHighlightedState(HighlightedTile tileToHighlight, ENUM_TileHighlightState tileHighlightState)
        {
            tileToHighlight.TileHighlightState = tileHighlightState;
            switch (tileHighlightState)
            {
                case ENUM_TileHighlightState.NotHighlighted:
                    break;
                case ENUM_TileHighlightState.MovementHighlight:
                    movementArea.Add(tileToHighlight);
                    break;
                case ENUM_TileHighlightState.InterceptionZone:
                    movementArea.Add(tileToHighlight);
                    break;
                case ENUM_TileHighlightState.AttackZone:
                    attackArea.Add(tileToHighlight);
                    break;
                default:
                    break;
            }
        }

        private void HighlightTile(HighlightedTile tileToHighlight)
        {
            if (!tilemapHighlight.Tilemap.HasTile(tileToHighlight.Tile.CenterPosition))
            {
                tilemapHighlight.Tilemap.SetTile(tileToHighlight.Tile.CenterPosition, highlightRuleTile);
            }

            tilemapHighlight.Tilemap.SetTileFlags(tileToHighlight.Tile.CenterPosition, TileFlags.None);

            switch (tileToHighlight.TileHighlightState)
            {
                case ENUM_TileHighlightState.NotHighlighted:
                    tilemapHighlight.Tilemap.SetColor(tileToHighlight.Tile.CenterPosition, deselectedColor);
                    if (tileToHighlight.Tile.StandingUnit != null) tileToHighlight.Tile.StandingUnit.SetUnitUnderAttack(false);
                    break;
                case ENUM_TileHighlightState.MovementHighlight:
                    tilemapHighlight.Tilemap.SetColor(tileToHighlight.Tile.CenterPosition, selectedColor);
                    break;
                case ENUM_TileHighlightState.InterceptionZone:
                    tilemapHighlight.Tilemap.SetColor(tileToHighlight.Tile.CenterPosition, interceptionColor);
                    break;
                case ENUM_TileHighlightState.AttackZone:
                    tilemapHighlight.Tilemap.SetColor(tileToHighlight.Tile.CenterPosition, attackZoneColor);
                    if (tileToHighlight.Tile.StandingUnit != null) tileToHighlight.Tile.StandingUnit.SetUnitUnderAttack(true);
                    break;
                default:
                    break;
            }
        }

         private void SetNeighbouringTilesOffset()
         {
            neighbouringTilesOffset = new Vector3Int[8];

            neighbouringTilesOffset[0].y++;

            neighbouringTilesOffset[1].x++;
            neighbouringTilesOffset[1].y++;

            neighbouringTilesOffset[2].x++;

            neighbouringTilesOffset[3].x++;
            neighbouringTilesOffset[3].y--;

            neighbouringTilesOffset[4].y--;

            neighbouringTilesOffset[5].x--;
            neighbouringTilesOffset[5].y--;

            neighbouringTilesOffset[6].x--;

            neighbouringTilesOffset[7].x--;
            neighbouringTilesOffset[7].y++;
         }
    }

    public class HighlightedTile
    {
        public TileData Tile;
        public ENUM_TileHighlightState TileHighlightState;
        public int Distance;
        public List<Units.Unit> Interceptors;
    }
}