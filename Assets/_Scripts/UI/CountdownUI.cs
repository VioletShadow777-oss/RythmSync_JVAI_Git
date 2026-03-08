using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    public static CountdownUI Instance { get; private set; }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Countdown)
            StartCoroutine(PlayCountdown());
        else
            countdownText.gameObject.SetActive(false); // Hide on any other state
    }

    public IEnumerator PlayCountdown()
    {
        countdownText.gameObject.SetActive(true);

        // 3... 2... 1...
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // GO!
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
    }
}