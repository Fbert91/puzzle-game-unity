using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Daily Puzzle Manager - One unique puzzle per day (seeded by date)
/// Calendar UI, countdown timer, rewards, separate from main progression
/// </summary>
public class DailyPuzzleManager : MonoBehaviour
{
    public static DailyPuzzleManager Instance { get; private set; }

    [System.Serializable]
    public class DailyPuzzleData
    {
        public int gridWidth;
        public int gridHeight;
        public string puzzleRule;
        public int targetSum;
        public int targetMoves;
        public int timeLimit;
        public int[] initialTiles;
    }

    [Header("UI References")]
    [SerializeField] private GameObject dailyPuzzlePanel;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text rewardText;
    [SerializeField] private Button playDailyButton;
    [SerializeField] private Transform calendarContainer;
    [SerializeField] private GameObject calendarDayPrefab;
    [SerializeField] private Text streakBonusText;

    [Header("Rewards")]
    [SerializeField] private int baseCoinsReward = 50;
    [SerializeField] private int consecutiveDaysForBonus = 7;
    [SerializeField] private string bonusCosmeticId = "daily_7day_badge";

    // Player prefs keys
    private const string DAILY_COMPLETED_PREFIX = "DailyCompleted_";
    private const string DAILY_LAST_COMPLETED = "DailyLastCompleted";
    private const string DAILY_CONSECUTIVE = "DailyConsecutiveDays";

    private string[] puzzleRules = { "SumToTen", "ConnectPatterns", "SequenceOrder" };
    private int consecutiveDays = 0;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadDailyData();

        if (playDailyButton != null)
        {
            playDailyButton.onClick.AddListener(PlayDailyPuzzle);
        }

        UpdateUI();
    }

    /// <summary>
    /// Generate today's daily puzzle (seeded by date)
    /// </summary>
    public DailyPuzzleData GetTodaysPuzzle()
    {
        DateTime today = DateTime.Now.Date;
        int dateSeed = today.Year * 10000 + today.Month * 100 + today.Day;
        System.Random rng = new System.Random(dateSeed);

        // Grid size varies by day of week
        int dayOfWeek = (int)today.DayOfWeek;
        int gw = 5 + (dayOfWeek % 3);
        int gh = 5 + ((dayOfWeek + 1) % 3);

        // Rule varies
        string rule = puzzleRules[dateSeed % puzzleRules.Length];

        // Generate board
        int maxVal = 5 + (dayOfWeek % 4);
        int[] tiles = new int[gw * gh];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = rng.Next(1, maxVal + 1);
        }

        // Target
        int targetSum = 10 + (dateSeed % 15);
        int targetMoves = 20 + (dateSeed % 20);
        int timeLimit = (dayOfWeek >= 5) ? 90 : 0; // Timed on weekends

        return new DailyPuzzleData
        {
            gridWidth = gw,
            gridHeight = gh,
            puzzleRule = rule,
            targetSum = targetSum,
            targetMoves = targetMoves,
            timeLimit = timeLimit,
            initialTiles = tiles
        };
    }

    /// <summary>
    /// Convert daily puzzle to PuzzleLevel for gameplay
    /// </summary>
    public PuzzleGame.PuzzleLevel ConvertToPuzzleLevel(DailyPuzzleData data)
    {
        return new PuzzleGame.PuzzleLevel
        {
            levelId = -1, // Special ID for daily
            gridWidth = data.gridWidth,
            gridHeight = data.gridHeight,
            puzzleRule = data.puzzleRule,
            initialTiles = data.initialTiles,
            targetSum = data.targetSum,
            difficulty = 2
        };
    }

    /// <summary>
    /// Check if today's daily has been completed
    /// </summary>
    public bool IsTodayCompleted()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        return PlayerPrefs.GetInt(DAILY_COMPLETED_PREFIX + today, 0) == 1;
    }

    /// <summary>
    /// Check if a specific date was completed
    /// </summary>
    public bool IsDateCompleted(DateTime date)
    {
        string dateStr = date.ToString("yyyy-MM-dd");
        return PlayerPrefs.GetInt(DAILY_COMPLETED_PREFIX + dateStr, 0) == 1;
    }

    /// <summary>
    /// Complete today's daily puzzle
    /// </summary>
    public void CompleteDailyPuzzle(int score, int moves)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (IsTodayCompleted()) return;

        // Mark as completed
        PlayerPrefs.SetInt(DAILY_COMPLETED_PREFIX + today, 1);

        // Check consecutive days
        string lastCompleted = PlayerPrefs.GetString(DAILY_LAST_COMPLETED, "");
        if (!string.IsNullOrEmpty(lastCompleted))
        {
            DateTime lastDate = DateTime.ParseExact(lastCompleted, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
            DateTime todayDate = DateTime.Now.Date;

            if ((todayDate - lastDate).TotalDays == 1)
            {
                consecutiveDays++;
            }
            else if ((todayDate - lastDate).TotalDays > 1)
            {
                consecutiveDays = 1;
            }
        }
        else
        {
            consecutiveDays = 1;
        }

        PlayerPrefs.SetString(DAILY_LAST_COMPLETED, today);
        PlayerPrefs.SetInt(DAILY_CONSECUTIVE, consecutiveDays);
        PlayerPrefs.Save();

        // Grant rewards
        GrantDailyReward();

        // Analytics
        if (Analytics.Instance != null)
        {
            Analytics.Instance.LogCustomEvent("daily_puzzle_complete", new System.Collections.Generic.Dictionary<string, object>
            {
                { "date", today },
                { "score", score },
                { "moves", moves },
                { "consecutive_days", consecutiveDays }
            });
        }

        Debug.Log($"[DailyPuzzle] Completed! Consecutive days: {consecutiveDays}");
    }

    /// <summary>
    /// Grant daily puzzle reward
    /// </summary>
    private void GrantDailyReward()
    {
        int coins = baseCoinsReward;

        // Bonus for consecutive days
        if (consecutiveDays >= consecutiveDaysForBonus)
        {
            coins *= 2;

            // Grant bonus cosmetic
            if (MonetizationManager.Instance != null)
            {
                // Special reward
                Debug.Log($"[DailyPuzzle] 7-day bonus! Granting cosmetic: {bonusCosmeticId}");
            }
        }

        if (MonetizationManager.Instance != null)
        {
            MonetizationManager.Instance.AddCoins(coins);
        }

        Debug.Log($"[DailyPuzzle] Reward: {coins} coins");
    }

    /// <summary>
    /// Play today's daily puzzle
    /// </summary>
    public void PlayDailyPuzzle()
    {
        if (IsTodayCompleted())
        {
            Debug.Log("[DailyPuzzle] Already completed today!");
            return;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        // Load daily puzzle into game
        DailyPuzzleData data = GetTodaysPuzzle();
        PuzzleGame.PuzzleLevel level = ConvertToPuzzleLevel(data);

        // Signal the game to load this level
        Debug.Log("[DailyPuzzle] Loading daily puzzle...");
    }

    /// <summary>
    /// Get time until next daily puzzle
    /// </summary>
    public TimeSpan GetTimeUntilNextDaily()
    {
        DateTime now = DateTime.Now;
        DateTime tomorrow = now.Date.AddDays(1);
        return tomorrow - now;
    }

    /// <summary>
    /// Get consecutive days count
    /// </summary>
    public int GetConsecutiveDays() => consecutiveDays;

    /// <summary>
    /// Load saved daily data
    /// </summary>
    private void LoadDailyData()
    {
        consecutiveDays = PlayerPrefs.GetInt(DAILY_CONSECUTIVE, 0);
    }

    /// <summary>
    /// Update UI elements
    /// </summary>
    private void UpdateUI()
    {
        // Update countdown
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);
        countdownCoroutine = StartCoroutine(UpdateCountdown());

        // Update play button
        if (playDailyButton != null)
        {
            playDailyButton.interactable = !IsTodayCompleted();
        }

        // Update reward text
        if (rewardText != null)
        {
            if (IsTodayCompleted())
                rewardText.text = "✅ Completed Today!";
            else
                rewardText.text = $"Reward: {baseCoinsReward} coins";
        }

        // Update streak bonus
        if (streakBonusText != null)
        {
            streakBonusText.text = $"Daily Streak: {consecutiveDays}/{consecutiveDaysForBonus}";
        }

        // Build calendar
        BuildCalendar();
    }

    /// <summary>
    /// Update countdown timer to next daily
    /// </summary>
    private IEnumerator UpdateCountdown()
    {
        while (true)
        {
            TimeSpan remaining = GetTimeUntilNextDaily();
            if (countdownText != null)
            {
                if (IsTodayCompleted())
                    countdownText.text = $"Next daily in: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                else
                    countdownText.text = "Today's puzzle available!";
            }
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Build calendar UI showing completed days of current month
    /// </summary>
    private void BuildCalendar()
    {
        if (calendarContainer == null || calendarDayPrefab == null) return;

        // Clear existing
        foreach (Transform child in calendarContainer)
        {
            Destroy(child.gameObject);
        }

        DateTime now = DateTime.Now;
        int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(now.Year, now.Month, day);
            GameObject dayObj = Instantiate(calendarDayPrefab, calendarContainer);

            Text dayText = dayObj.GetComponentInChildren<Text>();
            Image dayImage = dayObj.GetComponent<Image>();

            if (dayText != null)
                dayText.text = day.ToString();

            if (dayImage != null)
            {
                if (IsDateCompleted(date))
                    dayImage.color = new Color(0.4f, 0.9f, 0.4f); // Green completed
                else if (date.Date == now.Date)
                    dayImage.color = new Color(1f, 0.9f, 0.3f); // Yellow today
                else if (date > now)
                    dayImage.color = new Color(0.8f, 0.8f, 0.8f, 0.3f); // Dim future
                else
                    dayImage.color = new Color(0.9f, 0.9f, 0.9f); // Past uncompleted
            }
        }
    }
}
