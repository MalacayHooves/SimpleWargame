
namespace SimpleWargame.Map
{
    public enum ENUM_TileHighlightState
    {
        NotHighlighted,
        /// <summary>
        /// highlight tile for movement
        /// </summary>
        MovementHighlight,
        /// <summary>
        /// when unit moves from this tile it will be attacked
        /// </summary>
        InterceptionZone,
        /// <summary>
        /// unit standing on this tile can be attacked
        /// </summary>
        AttackZone
    }
}
