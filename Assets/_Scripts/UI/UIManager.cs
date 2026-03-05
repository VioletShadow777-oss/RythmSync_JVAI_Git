using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject resultsPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleStateChange;
    }

    /// <summary>Activates the correct panels for the given game state.</summary>
    private void HandleStateChange(GameState state)
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(false);
        countdownPanel.SetActive(false);
        pausePanel.SetActive(false);
        resultsPanel.SetActive(false);

        switch (state)
        {
            case GameState.Idle:
                mainMenuPanel.SetActive(true);
                break;
            case GameState.Countdown:
                countdownPanel.SetActive(true);
                hudPanel.SetActive(true);
                break;
            case GameState.Playing:
                hudPanel.SetActive(true);
                break;
            case GameState.Paused:
                hudPanel.SetActive(true);
                pausePanel.SetActive(true);
                break;
            case GameState.Results:
                resultsPanel.SetActive(true);
                break;
        }
    }
}
