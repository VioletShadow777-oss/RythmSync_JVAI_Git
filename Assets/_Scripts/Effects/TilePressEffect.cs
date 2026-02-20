using UnityEngine;

public class TilePressEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] tilePressParticals;

    // This Script will be called from laneInputManager Script
    public void PlayTilePressPartical(int index)
    {
        tilePressParticals[index].Play();
    }
}
