using UnityEngine;
using UnityEngine.InputSystem;

public class LaneInputManager : MonoBehaviour
{
    public Key[] laneKeys = new Key[5]
    {
        Key.A,
        Key.S,
        Key.D,
        Key.F,
        Key.G
    };

    void Update()
    {
        for (int i = 0; i < laneKeys.Length; i++)
        {
            if (Keyboard.current[laneKeys[i]].wasPressedThisFrame)
            {
                CheckLaneHit(i);
            }
        }
    }

    void CheckLaneHit(int laneIndex)
    {
        BaseTile[] tiles = FindObjectsByType<BaseTile>(FindObjectsSortMode.None);

        foreach (BaseTile tile in tiles)
        {
            if (tile.TryHit(laneIndex))
                return; // Correct tile hit
        }

        Debug.Log("wrong key pressed");
    }
}
