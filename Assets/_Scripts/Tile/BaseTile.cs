using UnityEngine;

public class BaseTile : MonoBehaviour
{
    protected float tileSpeed;
    public float despawnY = -6f;


    public static float HitLineY = 0f; // Y position of the hit line, set by HitLine script

    protected int laneIndex;      // Which lane this tile belongs to
    protected bool canBeHit;      // True when inside hit area

    public void Initialize(float fallSpeed, int lane)
    {
        tileSpeed = fallSpeed;
        laneIndex = lane;
        canBeHit = false;
    }
    private void Update()
    {
        Move(tileSpeed);
    }
    public void Move(float fallSpeed)
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < despawnY)
        {
            DespawnTile();
        }
    }

    public void SetHitState(bool state)
    {
        canBeHit = state;
    }

    private ScoreRatingType GetRating()
    {
        float distance = Mathf.Abs(transform.position.y - HitLineY);
        if (distance < 0.35f) 
        { 
            return ScoreRatingType.Perfect;
        }
        if (distance < 0.75f)
        { 
            return ScoreRatingType.Great;
        }
        if (distance < 1.25f)
        {
            return ScoreRatingType.Good;
        }
        if (distance < 0.1f)
        {
            return ScoreRatingType.Miss;
        }
        return ScoreRatingType.Miss;

    }

    public bool TryHit(int pressedLane)
    {
        if (!canBeHit)
            return false;

        if (pressedLane == laneIndex)
        {
            ScoreRatingType rating = GetRating();
            ScoreManager.Instance.AddScore(rating);
            DespawnTile();
            return true;
        }

        return false;
    }

    public void DespawnTile()
    {
        ObjectPooler.Instance.Despawn(transform);
    }
}