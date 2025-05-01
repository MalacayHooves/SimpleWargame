using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SimpleWargame.Map
{
    /// <summary>
    /// This class is needed to contain information about tile in tilemap
    /// </summary>
    [CreateAssetMenu(fileName = "NewMapRuleTile", menuName = "SimpleWargame/Map/MapRuleTile")]
    public class MapRuleTile : RuleTile<MapRuleTile.Neighbor>
    {
        [SerializeField] private ENUM_TileType tileType;

        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Null = 3;
            public const int NotNull = 4;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            switch (neighbor)
            {
                case Neighbor.Null: return tile == null;
                case Neighbor.NotNull: return tile != null;
            }
            return base.RuleMatch(neighbor, tile);
        }

        public ENUM_TileType GetTileType() { return tileType; }
    }
}