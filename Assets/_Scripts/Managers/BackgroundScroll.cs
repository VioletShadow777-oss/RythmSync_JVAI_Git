using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 15f;        
    public float initYPos = 74;  
    public float endYPos = 43;

    private void Start()
    {
            transform.position = new Vector3(transform.position.x, initYPos, transform.position.z);
    }

    void Update()
    {
        ScrollRoad();
    }

    void ResetPosition()
    {
        float overTravel = endYPos - transform.position.y;
        Vector3 newPos = new Vector3(transform.position.x, initYPos - overTravel, transform.position.z);

        transform.position = newPos;
    }

    void ScrollRoad()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y <= endYPos)
        {
            ResetPosition();
        }
    }
}