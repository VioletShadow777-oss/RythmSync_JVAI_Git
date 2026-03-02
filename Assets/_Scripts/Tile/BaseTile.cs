using UnityEngine;

public class BaseTile : MonoBehaviour
{
    protected float tileSpeed;
    public float despawnY = -6f;

    public static float HitLineY = 0f;

    protected int laneIndex;
    protected bool canBeHit;
    public bool WasHit { get; private set; } // Prevents phantom Miss on despawn

    /// <summary>Initializes tile speed, lane, and resets hit state.</summary>
    public void Initialize(float fallSpeed, int lane)
    {
        tileSpeed = fallSpeed;
        laneIndex = lane;
        canBeHit = false;
        WasHit = false; // Reset every time tile is recycled from pool

        TileRegistry.Register(laneIndex, this);
    }

    private void Update()
    {
        Move(tileSpeed);
    }

    public void Move(float fallSpeed)
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < despawnY)
            DespawnTile();
    }

    public void SetHitState(bool state)
    {
        canBeHit = state;
    }

    private ScoreRatingType GetRating()
    {
        float distance = Mathf.Abs(transform.position.y - HitLineY);

        if (distance < 0.5f) return ScoreRatingType.Perfect;
        if (distance < 0.85f) return ScoreRatingType.Great;
        if (distance < 1.1f) return ScoreRatingType.Good;

        return ScoreRatingType.Miss;
    }

    public bool TryHit(int pressedLane)
    {
        if (!canBeHit || pressedLane != laneIndex)
            return false;

        WasHit = true; // Mark before despawn so OnTriggerExit2D ignores this tile
        ScoreRatingType rating = GetRating();
        ScoreManager.Instance.AddScore(rating);
        DespawnTile();
        return true;
    }

    /// <summary>Unregisters from TileRegistry and returns to the pool.</summary>
    public void DespawnTile()
    {
        TileRegistry.Unregister(laneIndex, this);
        ObjectPooler.Instance.Despawn(transform);
    }
}
