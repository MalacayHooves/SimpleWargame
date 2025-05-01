using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleWargame.Map
{
    /// <summary>
    ///Base class for interaction with tilemap
    /// </summary>
    public class TilemapController : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private Tilemap tilemap;
        public Tilemap Tilemap => tilemap;

        private void Start()
        {
            if (tilemap == null) tilemap = GetComponent<Tilemap>();
            if (tilemap == null)
            { Debug.LogError($"TilemapMain: Error - there is no tilemap"); return; }
        }

        public Vector3Int GetTilePositionInt(Vector3 position)
        {
            return tilemap.WorldToCell(position);
        }
    }
}
