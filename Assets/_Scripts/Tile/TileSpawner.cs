using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Lane Settings")]
    public Transform[] lanes; // List of lanes
    [Space]

    [Header("Tiles")]
    public Transform normalTile;
    public Transform sliderTile;

    [Header("Spawn Chances")]
    [Range(0f, 1f)]
    public float sliderChance = 0.2f; // 20% slider

    // Stores the last spawned lane index
    private int lastLaneIndex = -1;

    public void SpawnTiles()
    {
        int laneIndex = GetRandomLane();

        // Decide which tile to spawn
        Transform tileToSpawn;

        if (Random.value < sliderChance)
            tileToSpawn = sliderTile;   // 20%
        else
            tileToSpawn = normalTile;   // 80%

        ObjectPooler.Instance.Spawn(tileToSpawn, lanes[laneIndex], Quaternion.identity);

        Debug.Log("Tile Spawned!!");
    }

    // Generates random lane index but never same as last lane
    private int GetRandomLane()
    {
        int newLane;

        do
        {
            // Pick a random lane
            newLane = Random.Range(0, lanes.Length);

        } while (newLane == lastLaneIndex); // Repeat if same as previous lane

        // Save this lane as last used
        lastLaneIndex = newLane;

        return newLane;
    }
}
