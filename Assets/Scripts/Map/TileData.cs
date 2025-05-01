using System;
using UnityEngine;
using SimpleWargame.Units;
using System.Collections.Generic;
using SimpleWargame.EffectSystem;

namespace SimpleWargame.Map
{
    /// <summary>
    /// This class contains information of tile
    /// </summary>
    [Serializable]
    public class TileData
    {
        public MapRuleTile MapRuleTile { get; private set; }
        public ENUM_TileType TileType { get; private set; }
        public Vector3Int CenterPosition { get; private set; }
        public Unit StandingUnit { get; set; }
        private List<Effect> effects = new List<Effect>();
        public List<Effect> Effects => effects;

        public void SetTileData(MapRuleTile mapRuleTile, ENUM_TileType tileType, Vector3Int centerPosition)
        {
            this.MapRuleTile = mapRuleTile;
            this.TileType = tileType;
            this.CenterPosition = centerPosition;
        }

        public void AddEffect(Effect effect)
        {
            if (effects.Exists(x => x == effect)) return;
            effects.Add(effect);

            if (StandingUnit == null) return;
            StandingUnit.AddEffect(effect, 0, true);
        }

        public void RemoveEffect(Effect effect)
        {
            if (!effects.Exists(x => x == effect)) return;
            effects.RemoveAll(x => x == effect);

            if (StandingUnit == null) return;
            StandingUnit.RemoveAllCopiesOfTheEffect(effect);
        }
    }
}
