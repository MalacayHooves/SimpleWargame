using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleWargame.Map
{
    /// <summary>
    /// Contains dictionary with all TileDatas (tiles, for short) and thier positions
    /// </summary>
    public class MapTilesDictionary : MonoBehaviour
    {
        private Dictionary<Vector3Int, TileData> mapTiles = new Dictionary<Vector3Int, TileData>();
        public Dictionary<Vector3Int, TileData> MapTiles => mapTiles;


        public void CreateDictionary(Tilemap tilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null && tile is MapRuleTile mapRuleTile)
                    {
                        Vector3Int position = new Vector3Int
                            (x + tilemap.origin.x, y + tilemap.origin.y, tilemap.origin.z);

                        mapTiles.Add(position,
                            CreateNewTileData(mapRuleTile, mapRuleTile.GetTileType(), position));
                    }
                }
            }
        }

        public TileData GetTile(Vector3Int position) 
        {
            if (mapTiles.TryGetValue(position, out TileData tileData)) return tileData;
            else return null;
        }

        private TileData CreateNewTileData(MapRuleTile mapRuleTile, ENUM_TileType tileType, Vector3Int position)
        {
            TileData tileData = new TileData();
            tileData.SetTileData(mapRuleTile, tileType, position);
            return tileData;
        }
    }
}
