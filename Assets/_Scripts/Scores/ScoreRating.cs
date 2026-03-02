public enum ScoreRatingType
{
    Perfect,
    Great,
    Good,
    Miss
}

public static class ScoreRating
{
    public static int GetScoreRating(ScoreRatingType rating)
    {
        switch (rating)
        {
            case ScoreRatingType.Perfect:
                    ScoreUI.Instance.StartCoroutine(ScoreUI.Instance.ShowPerfectCoroutine());
                return 100;
            case ScoreRatingType.Great:
                    ScoreUI.Instance.StartCoroutine(ScoreUI.Instance.ShowGreatCoroutine());
                return 70;
            case ScoreRatingType.Good:
                    ScoreUI.Instance.StartCoroutine(ScoreUI.Instance.ShowGoodCoroutine());
                return 50;
            case ScoreRatingType.Miss:
                    ScoreUI.Instance.StartCoroutine(ScoreUI.Instance.ShowMissCoroutine());
                return 0;
            default:
                return 0;
        }
    }
}