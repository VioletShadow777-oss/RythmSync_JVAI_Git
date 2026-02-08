using UnityEngine;

public class SliderTile : BaseTile
{
    public float extraLength = 2f;

    public void SetLength(float length)
    {
        Vector3 scale = transform.localScale;
        scale.y = length;
        transform.localScale = scale;
    }
}
