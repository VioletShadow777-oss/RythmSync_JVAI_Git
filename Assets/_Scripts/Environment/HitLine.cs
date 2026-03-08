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
            tile.SetHitState(true);
    }

    private void OnTriggerExit2D(Collider2D other)
{
    if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        return;

    BaseTile tile = other.GetComponent<BaseTile>();
    if (tile == null) return;

    tile.SetHitState(false);

    // Only register a Miss if the player never hit this tile
    // WasHit guards against the phantom Miss caused by despawning inside the trigger
    if (!tile.WasHit)
    {
        ScoreManager.Instance.AddScore(ScoreRatingType.Miss);

        if (WebSocketClientManager.Instance != null)
        {
            WebSocketClientManager.Instance.SendScore(ScoreRatingType.Miss.ToString());
        }

        Debug.Log("Missed lane");
    }
}
}
