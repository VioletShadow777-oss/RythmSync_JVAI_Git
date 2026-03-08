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

    private const int SpectrumSize = 512;
    private const int FrequencyBands = 20;
    private const int DetectionFrameInterval = 2;

    private float[] spectrum = new float[SpectrumSize];
    private float previousEnergy = 0f;
    private float lastBeatTime = 0f;
    private int frameCounter = 0;
    private bool songStarted = false;

    private List<float> beatQueue = new List<float>();

    public AudioSource audioSource;
    public TileSpawner tileSpawner;

    public void SetSong(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[BeatDetector] SetSong received null clip.");
            return;
        }

        audioSource.clip = clip;
        songStarted = false;
        beatQueue.Clear();
        lastBeatTime = -minBeatInterval;
        previousEnergy = 0f;
        frameCounter = 0;

        audioSource.Stop();
        audioSource.time = 0f;
    }

    /// <summary>Returns song playback progress as a normalized value between 0 and 1.</summary>
    public float SongProgress
    {
        get
        {
            if (audioSource.clip == null || audioSource.clip.length <= 0f)
                return 0f;

            return audioSource.time / audioSource.clip.length;
        }
    }

    void OnEnable()
    {
        songStarted = false;

        if (audioSource.clip != null && audioSource.time > 0f)
            audioSource.UnPause();
        else
        {
            beatQueue.Clear();
            audioSource.Play();
        }
    }

    void OnDisable()
    {
        audioSource.Pause();
    }

    /// <summary>Stops the audio and clears the beat queue so the next enable starts fresh from the beginning.</summary>
    public void ResetAudio()
    {
        audioSource.Stop();
        beatQueue.Clear();
    }

    void Update()
    {
        if (!songStarted && audioSource.isPlaying)
            songStarted = true;

        if (songStarted && !audioSource.isPlaying && GameManager.Instance.CurrentState == GameState.Playing)
        {
            Debug.Log("Song ended — triggering results.");
            ScoreManager.Instance.TriggerResults();
            return;
        }

        frameCounter++;
        if (frameCounter % DetectionFrameInterval == 0)
            DetectBeat();

        HandleSpawning();
    }

    private void DetectBeat()
    {
        if (!audioSource.isPlaying)
            return;

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float currentEnergy = 0f;
        for (int i = 0; i < FrequencyBands; i++)
            currentEnergy += spectrum[i];

        float songTime = audioSource.time;

        if (currentEnergy > previousEnergy * sensitivity && songTime - lastBeatTime > minBeatInterval)
        {
            beatQueue.Add(songTime);
            lastBeatTime = songTime;
            Debug.Log($"Beat detected at: {songTime:F2}s");
        }

        previousEnergy = currentEnergy;
    }

    private void HandleSpawning()
    {
        float currentTime = audioSource.time;

        for (int i = beatQueue.Count - 1; i >= 0; i--)
        {
            if (currentTime >= beatQueue[i] - spawnEarlyTime)
            {
                tileSpawner.SpawnTiles();
                beatQueue.RemoveAt(i);
            }
        }
    }
}
