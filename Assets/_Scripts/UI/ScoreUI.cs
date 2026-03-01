using UnityEngine;
using TMPro;
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

    //Shows Perfect Text
    public IEnumerator ShowPerfectCoroutine()
    {
        perfectUIText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        perfectUIText.SetActive(false);
    }

    //Shows Great Text
    public IEnumerator ShowGreatCoroutine()
    {
        greatUIText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        greatUIText.SetActive(false);
    }

    //Shows Good Text
    public IEnumerator ShowGoodCoroutine()
    {
        goodUIText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        goodUIText.SetActive(false);
    }

    //Shows Miss Text
    //public IEnumerator ShowMissCoroutine()
    //{
    //    missUIText.SetActive(true);
    //    yield return new WaitForSeconds(0.5f);
    //    missUIText.SetActive(false);
    //}
}