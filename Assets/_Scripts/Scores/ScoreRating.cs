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
        ScoreUI ui = ScoreUI.Instance;

        switch (rating)
        {
            case ScoreRatingType.Perfect:
                ui.StartCoroutine(ui.ShowRatingCoroutine(ui.perfectUIText));
                return 100;
            case ScoreRatingType.Great:
                ui.StartCoroutine(ui.ShowRatingCoroutine(ui.greatUIText));
                return 70;
            case ScoreRatingType.Good:
                ui.StartCoroutine(ui.ShowRatingCoroutine(ui.goodUIText));
                return 50;
            case ScoreRatingType.Miss:
                ui.StartCoroutine(ui.ShowRatingCoroutine(ui.missUIText));
                return 0;
            default:
                return 0;
        }
    }
}