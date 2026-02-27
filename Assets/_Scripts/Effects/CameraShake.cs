using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 0.2f;     // How long shake lasts
    public float amplitude = 0.5f;    // How strong the shake is
    public float frequency = 25f;     // How fast it vibrates

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    // Public method you can call from anywhere
    public void Shake()
    {
        // Stop previous shake if running
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Random small offset
            float x = Random.Range(-1f, 1f) * amplitude;
            float y = Random.Range(-1f, 1f) * amplitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            yield return new WaitForSeconds(1f / frequency);
        }

        // Reset position
        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}