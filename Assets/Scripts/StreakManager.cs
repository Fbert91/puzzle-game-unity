using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Streak Manager - 7-day streak system with increasing rewards
/// Visual flame/glow that grows with streak length
/// Streak freeze mechanic, persistent via PlayerPrefs
/// </summary>
public class StreakManager : MonoBehaviour
{
    public static StreakManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Text streakCountText;
    [SerializeField] private Text streakRewardText;
    [SerializeField] private Image streakFlameImage;
    [SerializeField] private GameObject streakFreezeIndicator;
    [SerializeField] private Text streakFreezeCountText;
    [SerializeField] private GameObject streakPanel;

    [Header("Streak Visual")]
    [SerializeField] private Color[] flameColors = new Color[] {
        new Color(1f, 0.6f, 0.2f, 0.3f),  // Day 1 - dim orange
        new Color(1f, 0.5f, 0.1f, 0.5f),  // Day 2
        new Color(1f, 0.4f, 0.0f, 0.6f),  // Day 3
        new Color(1f, 0.3f, 0.0f, 0.7f),  // Day 4
        new Color(1f, 0.2f, 0.0f, 0.8f),  // Day 5
        new Color(1f, 0.1f, 0.0f, 0.9f),  // Day 6
        new Color(1f, 0.0f, 0.0f, 1.0f),  // Day 7 - bright red
    };

    // Rewards per streak day
    private static readonly int[] StreakRewards = { 10, 20, 30, 50, 75, 100, 200 };

    // PlayerPrefs keys
    private const string STREAK_COUNT_KEY = "StreakCount";
    private const string STREAK_LAST_PLAYED_KEY = "StreakLastPlayed";
    private const string STREAK_FREEZE_COUNT_KEY = "StreakFreezeCount";
    private const string STREAK_FREEZE_LAST_EARNED_KEY = "StreakFreezeLastEarned";

    private int currentStreak = 0;
    private int streakFreezes = 0;
    private bool hasPlayedToday = false;

    public event Action<int> OnStreakUpdated;
    public event Action<int> OnStreakRewardClaimed;
    public event Action OnStreakLost;

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
        LoadStreakData();
        CheckStreakStatus();
        UpdateUI();
    }

    /// <summary>
    /// Load streak data from PlayerPrefs
    /// </summary>
    private void LoadStreakData()
    {
        currentStreak = PlayerPrefs.GetInt(STREAK_COUNT_KEY, 0);
        streakFreezes = PlayerPrefs.GetInt(STREAK_FREEZE_COUNT_KEY, 0);
    }

    /// <summary>
    /// Check if streak should reset (midnight local time)
    /// </summary>
    private void CheckStreakStatus()
    {
        string lastPlayed = PlayerPrefs.GetString(STREAK_LAST_PLAYED_KEY, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (string.IsNullOrEmpty(lastPlayed))
        {
            // First time
            currentStreak = 0;
            hasPlayedToday = false;
            return;
        }

        if (lastPlayed == today)
        {
            hasPlayedToday = true;
            return;
        }

        // Check if it was yesterday
        DateTime lastDate = DateTime.ParseExact(lastPlayed, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture);
        DateTime todayDate = DateTime.Now.Date;
        int daysDiff = (int)(todayDate - lastDate).TotalDays;

        if (daysDiff == 1)
        {
            // Streak continues, waiting for today's play
            hasPlayedToday = false;
        }
        else if (daysDiff == 2 && streakFreezes > 0)
        {
            // Use streak freeze
            streakFreezes--;
            PlayerPrefs.SetInt(STREAK_FREEZE_COUNT_KEY, streakFreezes);
            PlayerPrefs.Save();
            hasPlayedToday = false;
            Debug.Log("[Streak] Streak freeze used! Streak preserved.");
        }
        else if (daysDiff > 1)
        {
            // Streak broken
            currentStreak = 0;
            hasPlayedToday = false;
            PlayerPrefs.SetInt(STREAK_COUNT_KEY, 0);
            PlayerPrefs.Save();
            OnStreakLost?.Invoke();
            Debug.Log("[Streak] Streak broken!");
        }
    }

    /// <summary>
    /// Record a play session (call when player completes any level)
    /// </summary>
    public void RecordPlay()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (hasPlayedToday) return;

        hasPlayedToday = true;
        currentStreak = Mathf.Min(currentStreak + 1, 7);

        PlayerPrefs.SetInt(STREAK_COUNT_KEY, currentStreak);
        PlayerPrefs.SetString(STREAK_LAST_PLAYED_KEY, today);
        PlayerPrefs.Save();

        // Grant streak reward
        int reward = GetCurrentReward();
        if (MonetizationManager.Instance != null)
        {
            MonetizationManager.Instance.AddCoins(reward);
        }

        OnStreakUpdated?.Invoke(currentStreak);
        OnStreakRewardClaimed?.Invoke(reward);

        // Check for streak freeze earn (weekly from daily)
        CheckStreakFreezeEarn();

        UpdateUI();
        Debug.Log($"[Streak] Day {currentStreak}! Reward: {reward} coins");
    }

    /// <summary>
    /// Get reward for current streak day
    /// </summary>
    public int GetCurrentReward()
    {
        int index = Mathf.Clamp(currentStreak - 1, 0, StreakRewards.Length - 1);
        return StreakRewards[index];
    }

    /// <summary>
    /// Get reward for a specific streak day
    /// </summary>
    public int GetRewardForDay(int day)
    {
        int index = Mathf.Clamp(day - 1, 0, StreakRewards.Length - 1);
        return StreakRewards[index];
    }

    /// <summary>
    /// Check if player earns a streak freeze (1 per week from completing daily)
    /// </summary>
    private void CheckStreakFreezeEarn()
    {
        string lastEarned = PlayerPrefs.GetString(STREAK_FREEZE_LAST_EARNED_KEY, "");
        DateTime now = DateTime.Now;

        bool shouldEarn = false;
        if (string.IsNullOrEmpty(lastEarned))
        {
            shouldEarn = currentStreak >= 7;
        }
        else
        {
            DateTime lastDate = DateTime.ParseExact(lastEarned, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
            if ((now.Date - lastDate).TotalDays >= 7 && currentStreak >= 7)
            {
                shouldEarn = true;
            }
        }

        if (shouldEarn)
        {
            streakFreezes++;
            PlayerPrefs.SetInt(STREAK_FREEZE_COUNT_KEY, streakFreezes);
            PlayerPrefs.SetString(STREAK_FREEZE_LAST_EARNED_KEY, now.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
            Debug.Log($"[Streak] Earned a streak freeze! Total: {streakFreezes}");
        }
    }

    /// <summary>
    /// Use a streak freeze manually (from rewarded ad)
    /// </summary>
    public bool UseStreakFreeze()
    {
        if (streakFreezes > 0)
        {
            streakFreezes--;
            PlayerPrefs.SetInt(STREAK_FREEZE_COUNT_KEY, streakFreezes);
            PlayerPrefs.Save();
            Debug.Log($"[Streak] Streak freeze used manually. Remaining: {streakFreezes}");
            UpdateUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add streak freeze (e.g., from rewarded ad)
    /// </summary>
    public void AddStreakFreeze()
    {
        streakFreezes++;
        PlayerPrefs.SetInt(STREAK_FREEZE_COUNT_KEY, streakFreezes);
        PlayerPrefs.Save();
        UpdateUI();
        Debug.Log($"[Streak] Streak freeze added! Total: {streakFreezes}");
    }

    /// <summary>
    /// Update UI elements
    /// </summary>
    private void UpdateUI()
    {
        if (streakCountText != null)
            streakCountText.text = $"🔥 {currentStreak}";

        if (streakRewardText != null)
        {
            if (hasPlayedToday)
                streakRewardText.text = "✅ Played Today!";
            else
                streakRewardText.text = $"Play to earn {GetCurrentReward()} coins!";
        }

        // Flame visual
        if (streakFlameImage != null)
        {
            int colorIdx = Mathf.Clamp(currentStreak - 1, 0, flameColors.Length - 1);
            streakFlameImage.color = currentStreak > 0 ? flameColors[colorIdx] : Color.clear;

            // Scale flame with streak
            float scale = 0.5f + (currentStreak / 7f) * 0.5f;
            streakFlameImage.transform.localScale = Vector3.one * scale;
        }

        // Streak freeze indicator
        if (streakFreezeIndicator != null)
            streakFreezeIndicator.SetActive(streakFreezes > 0);

        if (streakFreezeCountText != null)
            streakFreezeCountText.text = $"❄️ {streakFreezes}";
    }

    // Public getters
    public int GetCurrentStreak() => currentStreak;
    public int GetStreakFreezes() => streakFreezes;
    public bool HasPlayedToday() => hasPlayedToday;
}
