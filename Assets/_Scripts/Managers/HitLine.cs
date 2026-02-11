using UnityEngine;

public class HitLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseTile tile = other.GetComponent<BaseTile>();
        if (tile != null)
        {
            tile.SetHitState(true); // Tile is now hittable
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BaseTile tile = other.GetComponent<BaseTile>();
        if (tile != null)
        {
            tile.SetHitState(false); // Too late
        }
    }
}
