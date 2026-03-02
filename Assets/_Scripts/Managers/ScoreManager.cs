using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score           { get; private set; }
    public int HighScore       { get; private set; }
    //public int ScoreMultiplier { get; private set; } = 1;

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
        if (rating == ScoreRatingType.Miss)
        {
            //ScoreMultiplier = 1;
            ScoreRating.GetScoreRating(ScoreRatingType.Miss);

        }
        else
        {
            int baseScore = ScoreRating.GetScoreRating(rating);
            Score += baseScore;
            Debug.Log($"Added {baseScore} points for a {rating} rating. Total Score: {Score}");
            //Score += baseScore * ScoreMultiplier;

            //ScoreMultiplier++;

            if (Score > HighScore)
                HighScore = Score;
        }
    }

    public void TriggerResults()
    {
        // Here is trigger for results screen, passing the final score and high score as needed
    }

    public void ResetScore()
    {
        Score = 0;
        //ScoreMultiplier = 1;
    }
}