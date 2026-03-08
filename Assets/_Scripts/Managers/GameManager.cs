using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private BeatDetector beatDetector;
    [Header("Countdown UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    private LaneInputManager laneInputManager;

    private void Awake()
    {
        Application.runInBackground = true;
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        laneInputManager = GetComponent<LaneInputManager>();
    }

    private void Start()
    {
        SetGameplaySystems(false);
        SetState(GameState.Idle);
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>Transitions from Idle to Countdown, then to Playing after a 3-second countdown.</summary>
    public void StartGame()
    {
        //StartCoroutine(CountdownUI.Instance.PlayCountdown();
        StartCoroutine(CountdownCoroutine());
    }

    /// <summary>Pauses the game, freezing time and disabling input and beat detection.</summary>
    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
            return;

        Time.timeScale = 0f;
        SetGameplaySystems(false);
    }

    /// <summary>Resumes the game from a paused state.</summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SetGameplaySystems(true);
        SetState(GameState.Playing);
    }

    /// <summary>Ends the game, disables gameplay systems, and transitions to Results state.</summary>
    public void EndGame()
    {
        SetGameplaySystems(false);
        SetState(GameState.Results);
    }

    /// <summary>Restarts the game from any state, resetting score and running the countdown again.</summary>
    public void RestartGame()
    {
        StopAllCoroutines(); // Stop any ongoing countdowns or coroutines
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ScoreManager.Instance.ResetScore();

        Time.timeScale = 1f; // Ensure time is running in case we were paused
        if (beatDetector != null)
        {
            beatDetector.ResetAudio();
        }

        StartGame();
        StartCoroutine(CountdownCoroutine());
    }

    /// <summary>Returns to the main menu from any state.</summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SetGameplaySystems(false);
        SetState(GameState.Idle);

        
    }

    private void SetGameplaySystems(bool active)
    {
        beatDetector.enabled = active;
        laneInputManager.enabled = active;
    }

    // Countdown coroutine that transitions from Idle to Countdown, then to Playing after a 3-second countdown.
    //private IEnumerator CountdownCoroutine()
    //{
    //    SetState(GameState.Countdown); // Triggers CountdownUI via event
    //    yield return new WaitForSeconds(3.5f); // 3s numbers + 0.5s "GO!"
    //    SetGameplaySystems(true);
    //    beatDetector.StartSong();
    //    SetState(GameState.Playing);
    //}

    private IEnumerator CountdownCoroutine()
    {
        SetState(GameState.Countdown);

        // 3... 2... 1...
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // GO!
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.text = ""; //Clear after GO!

        SetGameplaySystems(true);
        beatDetector.StartSong();
        SetState(GameState.Playing);
    }

    // Call this function to quit the application
    public void DoQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        
    }
    public void AddSongMenu()
    {

    }
}
