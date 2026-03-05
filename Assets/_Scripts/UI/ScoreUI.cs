using UnityEngine;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    public GameObject perfectUIText;
    public GameObject greatUIText;
    public GameObject goodUIText;
    public GameObject missUIText;

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

    /// <summary>Briefly flashes the given rating GameObject for 0.5 seconds.</summary>
    public IEnumerator ShowRatingCoroutine(GameObject ratingObject)
    {
        ratingObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ratingObject.SetActive(false);
    }
}