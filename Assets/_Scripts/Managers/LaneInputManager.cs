using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LaneInputManager : MonoBehaviour
{
    public TilePressEffect tilePressEffect;
    public CameraShake cameraShake;

    public Key[] laneKeys = new Key[5]
    {
        Key.A,
        Key.S,
        Key.D,
        Key.F,
        Key.G
    };

    private void Update()
    {
        for (int i = 0; i < laneKeys.Length; i++)
        {
            if (Keyboard.current[laneKeys[i]].wasPressedThisFrame)
                CheckLaneHit(i);
        }
    }

    private void CheckLaneHit(int laneIndex)
    {
        List<BaseTile> tiles = TileRegistry.GetTilesInLane(laneIndex);

        if (tiles == null || tiles.Count == 0)
        {
            Debug.Log("No tiles in lane " + laneIndex);
            return;
        }

        // Iterate a copy to avoid mutation issues during despawn
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (tiles[i].TryHit(laneIndex))
            {
                tilePressEffect.PlayTilePressPartical(laneIndex);
                cameraShake.Shake();
                return;
            }
        }

        Debug.Log("Wrong key pressed on lane " + laneIndex);
    }
}
