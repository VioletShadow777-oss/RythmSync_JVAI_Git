using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Beat Settings")]
    public float sensitivity = 1.5f;      // Higher = fewer beats
    public float minBeatInterval = 0.3f;  // Prevent spam

    private float[] spectrum = new float[1024];
    private float previousEnergy = 0f;
    private float lastBeatTime = 0f;

    void Start()
    {
        audioSource.Play();
    }

    void Update()
    {
        BeatDetection();
    }

    // Beat Detection
    private void BeatDetection()
    {
        if (!audioSource.isPlaying)
            return;

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float currentEnergy = 0f;

        // Check low frequencies (bass area)
        for (int i = 0; i < 20; i++)
        {
            currentEnergy += spectrum[i];
        }

        float songTime = audioSource.time;

        // Beat condition
        if (currentEnergy > previousEnergy * sensitivity)
        {
            if (songTime - lastBeatTime > minBeatInterval)
            {
                Debug.Log("Beat detected at: " + songTime);
                lastBeatTime = songTime;
            }
        }

        previousEnergy = currentEnergy;
    }
}
