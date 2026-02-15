using System.Collections.Generic;
using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    [Header("Beat Settings")]
    public float sensitivity = 1.5f;
    public float minBeatInterval = 0.3f;

    [Header("Spawn Timing")]
    [Tooltip("How early before the beat the tile should spawn")]
    public float spawnEarlyTime = 0.8f;

    private float[] spectrum = new float[1024];
    private float previousEnergy = 0f;
    private float lastBeatTime = 0f;

    // Store upcoming beats
    private List<float> beatQueue = new List<float>();

    public AudioSource audioSource;
    public TileSpawner tileSpawner;

    void Start()
    {
        audioSource.Play();
    }

    void Update()
    {
        DetectBeat();
        HandleSpawning();
    }

    // Detect beat and store beat time
    private void DetectBeat()
    {
        if (!audioSource.isPlaying)
            return;

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float currentEnergy = 0f;

        for (int i = 0; i < 20; i++)
        {
            currentEnergy += spectrum[i];
        }

        float songTime = audioSource.time;

        if (currentEnergy > previousEnergy * sensitivity)
        {
            if (songTime - lastBeatTime > minBeatInterval)
            {
                // Store beat time instead of spawning instantly
                beatQueue.Add(songTime);

                Debug.Log("Beat detected at: " + songTime);
                lastBeatTime = songTime;
            }
        }

        previousEnergy = currentEnergy;
    }

    // Spawn tiles slightly before beat
    private void HandleSpawning()
    {
        float currentTime = audioSource.time;

        for (int i = beatQueue.Count - 1; i >= 0; i--)
        {
            float beatTime = beatQueue[i];

            // Spawn early
            if (currentTime >= beatTime - spawnEarlyTime)
            {
                tileSpawner.SpawnTiles();
                beatQueue.RemoveAt(i);
            }
        }
    }
}
