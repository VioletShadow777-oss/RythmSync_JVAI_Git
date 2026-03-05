using TMPro;
using UnityEngine;

public class ResultsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI perfectCountText;
    [SerializeField] private TextMeshProUGUI greatCountText;
    [SerializeField] private TextMeshProUGUI goodCountText;
    [SerializeField] private TextMeshProUGUI missCountText;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleStateChange;

        // Panel was inactive when Results fired — catch up immediately
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Results)
            Populate();
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Results)
            Populate();
    }

    /// <summary>Reads all counters from ScoreManager and populates the results screen.</summary>
    public void Populate()
    {
        ScoreManager sm = ScoreManager.Instance;

        finalScoreText.text  = $"Score: {sm.Score}";
        highScoreText.text   = $"Best: {sm.HighScore}";
        perfectCountText.text = $"Perfect: {sm.PerfectCount}";
        greatCountText.text   = $"Great: {sm.GreatCount}";
        goodCountText.text    = $"Good: {sm.GoodCount}";
        missCountText.text    = $"Miss: {sm.MissCount}";
        gradeText.text        = CalculateGrade(sm);
    }

    private string CalculateGrade(ScoreManager sm)
    {
        int total = sm.PerfectCount + sm.GreatCount + sm.GoodCount + sm.MissCount;
        if (total == 0) return "D";

        float perfectRatio = (float)sm.PerfectCount / total;

        if (perfectRatio >= 0.95f) return "S";
        if (perfectRatio >= 0.80f) return "A";
        if (perfectRatio >= 0.60f) return "B";
        if (perfectRatio >= 0.40f) return "C";
        return "D";
    }
}
