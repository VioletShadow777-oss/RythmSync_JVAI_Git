using System.Collections.Generic;

public static class TileRegistry
{
    private static readonly Dictionary<int, List<BaseTile>> _laneMap = new();

    /// <summary>Registers a tile into its lane bucket.</summary>
    public static void Register(int laneIndex, BaseTile tile)
    {
        if (!_laneMap.ContainsKey(laneIndex))
            _laneMap[laneIndex] = new List<BaseTile>();

        _laneMap[laneIndex].Add(tile);
    }

    /// <summary>Removes a tile from its lane bucket.</summary>
    public static void Unregister(int laneIndex, BaseTile tile)
    {
        if (_laneMap.TryGetValue(laneIndex, out List<BaseTile> tiles))
            tiles.Remove(tile);
    }

    /// <summary>Returns all active tiles in the given lane.</summary>
    public static List<BaseTile> GetTilesInLane(int laneIndex)
    {
        if (_laneMap.TryGetValue(laneIndex, out List<BaseTile> tiles))
            return tiles;

        return null;
    }
}
