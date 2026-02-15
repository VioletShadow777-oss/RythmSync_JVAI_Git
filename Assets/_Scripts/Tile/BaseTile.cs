using UnityEngine;

public class BaseTile : MonoBehaviour
{
    protected float tileSpeed;
    public float despawnY = -6f;

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

    public bool TryHit(int pressedLane)
    {
        if (!canBeHit)
            return false;

        if (pressedLane == laneIndex)
        {
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
