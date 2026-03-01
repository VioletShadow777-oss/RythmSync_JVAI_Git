using UnityEngine;

public class HitLine : MonoBehaviour
{
    private void Start()
    {
        BaseTile.HitLineY = transform.position.y;
    }

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
            ScoreManager.Instance.AddScore(ScoreRatingType.Miss); // No score for a miss, but you could still trigger other effects here if desired
            Debug.Log("Missed lane");
            

        }
    }
}
