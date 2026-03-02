using UnityEngine;

public class BaseTile : MonoBehaviour
{
    protected float tileSpeed;
    public float despawnY = -6f;

    public static float HitLineY = 0f;

    protected int laneIndex;
    protected bool canBeHit;

    /// <summary>Initializes tile speed and lane, and registers into TileRegistry.</summary>
    public void Initialize(float fallSpeed, int lane)
    {
        tileSpeed = fallSpeed;
        laneIndex = lane;
        canBeHit = false;

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

        if (distance < 0.35f) return ScoreRatingType.Perfect;
        if (distance < 0.75f) return ScoreRatingType.Great;
        if (distance < 1.25f) return ScoreRatingType.Good;

        return ScoreRatingType.Miss;
    }

    public bool TryHit(int pressedLane)
    {
        if (!canBeHit || pressedLane != laneIndex)
            return false;

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
