using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Map;

namespace SimpleWargame
{
    public class Utilities
    {
        public static int GetDistanceInTiles(TileData start, TileData finish)
        {
            int distance = 0;

            int x = Mathf.Abs(finish.CenterPosition.x - start.CenterPosition.x);
            int y = Mathf.Abs(finish.CenterPosition.y - start.CenterPosition.y);

            if (x > y) distance = x; else distance = y;

            return distance;
        }
    }
}
