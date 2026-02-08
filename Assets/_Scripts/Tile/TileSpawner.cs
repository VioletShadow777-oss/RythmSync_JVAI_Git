using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public LaneManager laneManager;

    [Header("Tile Prefabs (Pool Keys)")]
    public Transform normalTilePrefab;
    public Transform sliderTilePrefab;

    [Header("Spawn Settings")]
    public float fallSpeed = 5f;
    [Range(0f, 1f)]
    public float sliderChance = 0.2f;

    public void SpawnTile(float songTime)
    {
        int lane = laneManager.GetRandomLane();

        if (!laneManager.CanSpawnInLane(lane, songTime))
            return;

        Vector3 spawnPos = laneManager.GetLanePosition(lane);

        bool spawnSlider = Random.value < sliderChance;

        if (spawnSlider)
        {
            Transform obj = ObjectPooler.Instance.Spawn(sliderTilePrefab, spawnPos, Quaternion.identity);
            SliderTile tile = obj.GetComponent<SliderTile>();
            tile.Initialize(fallSpeed);
            tile.SetLength(Random.Range(1.5f, 3f));
        }
        else
        {
            Transform obj = ObjectPooler.Instance.Spawn(normalTilePrefab, spawnPos, Quaternion.identity);
            NormalTile tile = obj.GetComponent<NormalTile>();
            tile.Initialize(fallSpeed);
        }
    }
}
