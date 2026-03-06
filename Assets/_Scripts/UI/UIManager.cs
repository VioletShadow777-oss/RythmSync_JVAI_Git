using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject countdownPanel;
    public GameObject pausePanel;
    public GameObject resultsPanel;

    [Header("Pause Panel")]
    private bool isPauseActive;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        isPauseActive = false; // Initialize settings panel state
        pausePanel.SetActive(false); // Ensure it's hidden at start

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
            case GameState.Results:
                resultsPanel.SetActive(true);
                break;
        }
    }

    // this method toggles between acive and disable the settings panel

    public void ToggleSettingsPanel()
    {
        if(isPauseActive == false)
        {
            pausePanel.SetActive(true);
            isPauseActive = true;

            //Pause the game when settings panel is active
            GameManager.Instance.PauseGame();
        }
        else
        {
            pausePanel.SetActive(false);
            isPauseActive = false;

            //Resume the game when settings panel is closed
            GameManager.Instance.ResumeGame();
        }
    }

}
