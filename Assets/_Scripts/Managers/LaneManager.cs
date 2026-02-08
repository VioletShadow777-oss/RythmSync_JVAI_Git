using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public Transform[] lanes;   // Assign 4 lanes in Inspector

    private float[] laneLastSpawnTime;

    [Header("Spawn Spacing")]
    public float minTimeBetweenTiles = 0.4f;

    private void Awake()
    {
        laneLastSpawnTime = new float[lanes.Length];
    }

    public bool CanSpawnInLane(int laneIndex, float currentSongTime)
    {
        if (currentSongTime - laneLastSpawnTime[laneIndex] > minTimeBetweenTiles)
        {
            laneLastSpawnTime[laneIndex] = currentSongTime;
            return true;
        }

        return false;
    }

    public Vector3 GetLanePosition(int laneIndex)
    {
        return lanes[laneIndex].position;
    }

    public int GetRandomLane()
    {
        return Random.Range(0, lanes.Length);
    }
}
