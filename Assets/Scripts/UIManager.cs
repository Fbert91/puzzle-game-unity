using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UI Manager - Handles all UI screens and transitions
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // UI Panels
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    // Main Menu Elements
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutButton;

    // Level Select Elements
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Text completionPercentageText;

    // Gameplay HUD Elements
    [SerializeField] private Text levelNameText;
    [SerializeField] private Text moveCounterText;
    [SerializeField] private Text hintButtonText;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button settingsGameplayButton;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text gemsText;

    // Victory Screen Elements
    [SerializeField] private Text victoryScoreText;
    [SerializeField] private Image[] starsImages;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    // Shop Elements
    [SerializeField] private Transform shopItemContainer;
    [SerializeField] private GameObject shopItemPrefab;

    private int currentLevelId = 1;
    private PitouCharacterController characterController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Setup button listeners
        SetupMainMenuListeners();
        SetupGameplayListeners();
        SetupVictoryListeners();

        // Show main menu
        ShowMainMenu();

        // Get character controller
        characterController = FindObjectOfType<PitouCharacterController>();
    }

    #region Navigation

    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel == null)
        {
            Debug.LogError("[UIManager] mainMenuPanel is null! Did you run Build All Scenes?");
            return;
        }
        mainMenuPanel.SetActive(true);
        if (characterController) characterController.PlayIdle();
        Time.timeScale = 1f; // Resume time
        
        // Play menu music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    public void ShowLevelSelect()
    {
        HideAllPanels();
        levelSelectPanel.SetActive(true);
        PopulateLevelButtons();
        UpdateProgressDisplay();
    }

    public void ShowGameplay(int levelId)
    {
        HideAllPanels();
        currentLevelId = levelId;
        gameplayPanel.SetActive(true);
        Time.timeScale = 1f;

        // Load the level
        if (PuzzleGame.Instance)
        {
            PuzzleGame.Instance.LoadLevel(levelId);
            UpdateGameplayHUD();
        }
        
        // Play gameplay music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameplayMusic();

        // Trigger tutorial if needed
        if (TutorialManager.Instance != null && TutorialManager.Instance.ShouldShowTutorial(levelId))
        {
            TutorialManager.Instance.StartTutorial(levelId);
        }
    }

    public void ShowVictory(int score, int stars)
    {
        HideAllPanels();
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // Pause time on victory

        victoryScoreText.text = $"Score: {score}";
        for (int i = 0; i < starsImages.Length; i++)
        {
            starsImages[i].enabled = (i < stars);
        }

        if (characterController)
            characterController.PlayWinAnimation();
        
        // Play victory sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelComplete();
    }

    public void ShowSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void ShowShop()
    {
        HideAllPanels();
        shopPanel.SetActive(true);
        PopulateShopItems();
    }

    public void ShowPause()
    {
        HideAllPanels();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumePause()
    {
        pausePanel.SetActive(false);
        gameplayPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(false);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);
        pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        HideAllPanels();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayInvalidMove();
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        ShowGameplay(currentLevelId);
    }

    #endregion

    #region Main Menu

    private void SetupMainMenuListeners()
    {
        playButton.onClick.AddListener(() => 
        { 
            AudioManager.Instance?.PlayButtonClick();
            ShowLevelSelect();
        });
        settingsButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowSettings();
        });
    }

    #endregion

    #region Level Select

    private void PopulateLevelButtons()
    {
        if (levelButtonContainer == null)
        {
            Debug.LogError("[UIManager] levelButtonContainer is null! Did you run Build All Scenes?");
            return;
        }

        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        if (LevelManager.Instance == null)
        {
            Debug.LogError("[UIManager] LevelManager.Instance is null! GameInitializer may not have run.");
            return;
        }

        List<PuzzleGame.PuzzleLevel> levels = LevelManager.Instance.GetAllLevels();

        foreach (var level in levels)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            int levelId = level.levelId;
            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelId);
            LevelManager.LevelProgress progress = LevelManager.Instance.GetLevelProgress(levelId);

            buttonText.text = $"Level {levelId}";
            button.interactable = isUnlocked;

            // Show stars if completed
            if (progress != null && progress.completed)
            {
                buttonText.text += " " + new string('*', progress.starsEarned);
            }

            button.onClick.AddListener(() =>
            {
                if (isUnlocked)
                {
                    ShowGameplay(levelId);
                }
            });
        }
    }

    private void UpdateProgressDisplay()
    {
        float completionPercent = LevelManager.Instance.GetCompletionPercentage();
        completionPercentageText.text = $"Progress: {completionPercent:F0}%";
    }

    #endregion

    #region Gameplay HUD

    private void SetupGameplayListeners()
    {
        hintButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            UseHint();
        });
        
        pauseButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowPause();
        });
        
        settingsGameplayButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowSettings();
        });

        // Subscribe to puzzle events
        if (PuzzleGame.Instance)
        {
            PuzzleGame.Instance.OnBoardUpdated += UpdateGameplayHUD;
            PuzzleGame.Instance.OnPuzzleSolved += OnPuzzleSolved;
            PuzzleGame.Instance.OnNoValidMoves += ShowGameOver;
            PuzzleGame.Instance.OnHintUsed += OnHintUsed;
            PuzzleGame.Instance.OnTileSelected += (tile) => 
            { 
                AudioManager.Instance?.PlayTilePickup();
            };
        }
    }

    private void UpdateGameplayHUD()
    {
        if (!PuzzleGame.Instance) return;

        var level = PuzzleGame.Instance.GetCurrentLevel();
        levelNameText.text = $"Level {level.levelId}";
        moveCounterText.text = $"Moves: {PuzzleGame.Instance.GetMoveCount()}";
        hintButtonText.text = $"Hint ({MonetizationManager.Instance.GetHints()})";

        coinsText.text = MonetizationManager.Instance.GetCoins().ToString();
        gemsText.text = MonetizationManager.Instance.GetGems().ToString();
    }

    private void UseHint()
    {
        // Try HintManager first (it has its own hint count and visual feedback)
        if (HintManager.Instance != null)
        {
            HintManager.Instance.UseHint();
            UpdateGameplayHUD();
            return;
        }

        // Fallback to MonetizationManager
        if (MonetizationManager.Instance != null && MonetizationManager.Instance.UseHint())
        {
            PuzzleGame.Instance.UseHint();
            UpdateGameplayHUD();

            if (characterController)
                characterController.PlayHintGesture();
        }
        else
        {
            Debug.Log("No hints available.");
        }
    }

    private void OnHintUsed(int hintCount)
    {
        Analytics.Instance.LogHintUsed(currentLevelId, hintCount);
        UpdateGameplayHUD();
    }

    private void OnPuzzleSolved()
    {
        // Delay slightly for visual feedback
        Invoke(nameof(ShowVictoryAfterDelay), 1f);
    }

    private void ShowVictoryAfterDelay()
    {
        int score = 1000 + Random.Range(0, 500); // Placeholder
        int stars = Random.Range(1, 4);
        
        LevelManager.Instance.CompleteLevel(currentLevelId, score, stars);
        ShowVictory(score, stars);
    }

    #endregion

    #region Victory

    private void SetupVictoryListeners()
    {
        nextLevelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            PlayNextLevel();
        });
        
        replayButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowGameplay(currentLevelId);
        });
        
        menuButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButtonClick();
            ShowMainMenu();
        });
    }

    private void PlayNextLevel()
    {
        ShowLevelSelect();
    }

    #endregion

    #region Shop

    private void PopulateShopItems()
    {
        // Clear existing items
        foreach (Transform child in shopItemContainer)
        {
            Destroy(child.gameObject);
        }

        List<MonetizationManager.IAPProduct> products = MonetizationManager.Instance.GetIAPProducts();

        foreach (var product in products)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemContainer);
            Button button = itemObj.GetComponent<Button>();
            Text priceText = itemObj.GetComponentInChildren<Text>();

            priceText.text = $"{product.displayName}\n${product.price:F2}";

            button.onClick.AddListener(() =>
            {
                MonetizationManager.Instance.PurchaseProduct(product.productId);
            });
        }
    }

    #endregion

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
    }
}
