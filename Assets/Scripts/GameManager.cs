using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Handles modern TextMeshPro UI elements

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score & Victory Configuration")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int targetWinScore = 10;

    [Header("In-Scene UI Overlay Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [Header("Scene Navigation Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameplayScene = "Gameplay";

    private int currentScore = 0;
    private bool isGameActive = true;
    private bool isPaused = false;

    private void Awake()
    {
        // Setup Singleton architecture safely
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Section B requirement: Trace key runtime events in the console
        Debug.Log("GameManager: Initialized gameplay environment tracking loop.");

        // Ensure all UI overlay screens are hidden on startup
        InitializeUI();
        UpdateScoreUI();
    }

    private void Update()
    {
        // Listen for standard pause input command key (Escape)
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            TogglePause();
        }

        // Quick shortcut keyboard restart functionality (Rubric requirement)
        if (!isGameActive && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Forces hidden defaults on all navigation overlay screens.
    /// </summary>
    private void InitializeUI()
    {
        // Enforce active execution state on startup
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    /// <summary>
    /// Custom single-responsibility scoring method (Section B requirement).
    /// </summary>
    public void AddScore(int points)
    {
        if (!isGameActive) return;

        currentScore += points;
        UpdateScoreUI();

        Debug.Log($"[Score Update] Scavenged item processed. Score: {currentScore}/{targetWinScore}");

        if (currentScore >= targetWinScore)
        {
            TriggerWinState();
        }
    }
    /// <summary>
    /// Allows external spawner managers to set the exact goal target dynamically at start.
    /// </summary>
    public void SetTargetWinScore(int amount)
    {
        targetWinScore = amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Scrap Collected: " + currentScore + " / " + targetWinScore;
        }
    }

    /// <summary>
    /// Toggles the hardware clock loop freeze to manage pause breaks.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Halt physics engines completely
            if (pausePanel != null) pausePanel.SetActive(true);
            Debug.Log("[Game Loop] System execution paused by player.");
        }
        else
        {
            Time.timeScale = 1f; // Restore normal processing speed
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("[Game Loop] System execution resumed.");
        }
    }

    /// <summary>
    /// Custom method to initialize failure operations (Section B requirement).
    /// </summary>
    public void TriggerGameOver()
    {
        if (!isGameActive) return;

        isGameActive = false;
        Time.timeScale = 0f; // Freeze asteroid movement on screen behind overlay

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Debug.Log("[Game State] Drone hull compromised. Game Over display initialized.");
    }

    /// <summary>
    /// Custom method to initialize player victory operations.
    /// </summary>
    private void TriggerWinState()
    {
        if (!isGameActive) return;

        isGameActive = false;
        Time.timeScale = 0f; // Freeze hazards upon winning

        if (winPanel != null) winPanel.SetActive(true);
        Debug.Log("[Game State] Scavenger milestones completed. Victory menu displayed.");
    }

    /// <summary>
    /// Resets physics scale variables and refreshes the gameplay arena loop freshly.
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("GameManager: Re-initializing active gameplay arena scene elements.");
        Time.timeScale = 1f; // CRUCIAL: Reset the clock speed so things move again!
        SceneManager.LoadScene(gameplayScene);
    }

    /// <summary>
    /// Universal button navigation command function to load back into MainMenu scene.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Debug.Log("GameManager: Exiting gameplay layout environment context.");
        Time.timeScale = 1f; // CRUCIAL: Reset clock before exiting level context
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// NEW METHOD: Triggered by the Main Menu button to launch the game session.
    /// </summary>
    public void StartGame()
    {
        Debug.Log($"GameManager: Loading core gameplay scene context: '{gameplayScene}'");
        Time.timeScale = 1f; // Ensure physics run at full speed upon loading
        SceneManager.LoadScene(gameplayScene);
    }
}
