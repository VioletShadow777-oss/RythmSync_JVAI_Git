using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Slider songProgressBar;
    [SerializeField] private BeatDetector beatDetector;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        Refresh();
    }

    private int cachedScore     = -1;
    private int cachedHighScore = -1;
    private int cachedCombo     = -1;
    private float cachedProgress = -1f;

    /// <summary>Updates HUD elements only when values have changed, avoiding per-frame string allocations.</summary>
    public void Refresh()
    {
        ScoreManager sm = ScoreManager.Instance;

        if (sm.Score != cachedScore)
        {
            cachedScore = sm.Score;
            scoreText.text = $"Score: {cachedScore}";
        }

        if (sm.HighScore != cachedHighScore)
        {
            cachedHighScore = sm.HighScore;
            highScoreText.text = $"Best: {cachedHighScore}";
        }

        if (sm.Combo != cachedCombo)
        {
            cachedCombo = sm.Combo;
            comboText.text = $"x{cachedCombo}";
        }

        float progress = beatDetector.SongProgress;
        if (Mathf.Abs(progress - cachedProgress) > 0.001f)
        {
            cachedProgress = progress;
            songProgressBar.value = progress;
        }
    }
}
