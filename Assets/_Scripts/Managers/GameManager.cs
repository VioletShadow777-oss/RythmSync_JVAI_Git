using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BaseTile baseTile;

    private void Start()
    {
        baseTile.Initialize(baseTile.tileSpeed);
    }
}
