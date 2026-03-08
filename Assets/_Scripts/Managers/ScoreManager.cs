using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score     { get; private set; }
    public int HighScore { get; private set; }

    public int PerfectCount { get; private set; }
    public int GreatCount   { get; private set; }
    public int GoodCount    { get; private set; }
    public int MissCount    { get; private set; }
    public int Combo        { get; private set; }
    public int MaxCombo     { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(ScoreRatingType rating)
    {
        int baseScore = ScoreRating.GetScoreRating(rating);
        Score += baseScore;

        if (rating == ScoreRatingType.Miss)
        {
            MissCount++;
            Combo = 0;
            Debug.Log($"Miss! Combo reset. Total Score: {Score}");
            return;
        }

        switch (rating)
        {
            case ScoreRatingType.Perfect: PerfectCount++; break;
            case ScoreRatingType.Great:   GreatCount++;   break;
            case ScoreRatingType.Good:    GoodCount++;    break;
        }

        Combo++;
        if (Combo > MaxCombo) MaxCombo = Combo;
        if (Score > HighScore) HighScore = Score;

        Debug.Log($"Added {baseScore} points for a {rating} rating. Total Score: {Score} | Combo: {Combo}");
    }

    /// <summary>Signals the end of the song and hands control over to GameManager to transition to Results state.</summary>
    public void TriggerResults()
    {
        if (WebSocketClientManager.Instance != null)
        {
            WebSocketClientManager.Instance.SendFinalResult();
        }

        GameManager.Instance.EndGame();
    }

    public void ResetScore()
    {
        Score        = 0;
        PerfectCount = 0;
        GreatCount   = 0;
        GoodCount    = 0;
        MissCount    = 0;
        Combo        = 0;
        MaxCombo     = 0;
    }
}