using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Hint Manager - 3 free hints per day, rewarded ad integration
/// Highlights correct tile placement with glow + Pitou animation
/// </summary>
public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Text hintCountText;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button buyHintAdButton;
    [SerializeField] private Button buyHintIAPButton;
    [SerializeField] private GameObject hintOverlay;
    [SerializeField] private Image hintGlowEffect;

    [Header("Settings")]
    [SerializeField] private int freeHintsPerDay = 3;
    [SerializeField] private int hintsFromAd = 1;
    [SerializeField] private int hintsFromIAP = 10;
    [SerializeField] private float hintGlowDuration = 2f;

    // Pitou hint messages
    private static readonly string[] HintMessages = {
        "Try this one! 🐾",
        "Look here! 👀",
        "Psst... this might help! 😸",
        "I found something! 🌟",
        "How about this? 🐱"
    };

    // PlayerPrefs keys
    private const string HINTS_REMAINING_KEY = "HintsRemaining";
    private const string HINTS_LAST_RESET_KEY = "HintsLastReset";
    private const string HINTS_PURCHASED_KEY = "HintsPurchased";

    private int freeHintsRemaining;
    private int purchasedHints;
    private bool isHintActive = false;

    public event Action<int, int> OnHintUsed; // tileX, tileY
    public event Action OnHintsExhausted;

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
        LoadHintData();
        CheckDailyReset();
        SetupButtons();
        UpdateUI();
    }

    /// <summary>
    /// Load hint data from PlayerPrefs
    /// </summary>
    private void LoadHintData()
    {
        freeHintsRemaining = PlayerPrefs.GetInt(HINTS_REMAINING_KEY, freeHintsPerDay);
        purchasedHints = PlayerPrefs.GetInt(HINTS_PURCHASED_KEY, 0);
    }

    /// <summary>
    /// Check if free hints should reset (midnight)
    /// </summary>
    private void CheckDailyReset()
    {
        string lastReset = PlayerPrefs.GetString(HINTS_LAST_RESET_KEY, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (lastReset != today)
        {
            freeHintsRemaining = freeHintsPerDay;
            PlayerPrefs.SetInt(HINTS_REMAINING_KEY, freeHintsRemaining);
            PlayerPrefs.SetString(HINTS_LAST_RESET_KEY, today);
            PlayerPrefs.Save();
            Debug.Log("[Hints] Daily hints reset!");
        }
    }

    /// <summary>
    /// Setup button listeners
    /// </summary>
    private void SetupButtons()
    {
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(UseHint);
        }

        if (buyHintAdButton != null)
        {
            buyHintAdButton.onClick.AddListener(BuyHintWithAd);
        }

        if (buyHintIAPButton != null)
        {
            buyHintIAPButton.onClick.AddListener(BuyHintWithIAP);
        }
    }

    /// <summary>
    /// Get total available hints (free + purchased)
    /// </summary>
    public int GetTotalHints()
    {
        return freeHintsRemaining + purchasedHints;
    }

    /// <summary>
    /// Use a hint in the current puzzle
    /// </summary>
    public void UseHint()
    {
        if (isHintActive) return;

        int totalHints = GetTotalHints();
        if (totalHints <= 0)
        {
            OnHintsExhausted?.Invoke();
            Debug.Log("[Hints] No hints available!");
            return;
        }

        // Consume hint (free first, then purchased)
        if (freeHintsRemaining > 0)
        {
            freeHintsRemaining--;
            PlayerPrefs.SetInt(HINTS_REMAINING_KEY, freeHintsRemaining);
        }
        else if (purchasedHints > 0)
        {
            purchasedHints--;
            PlayerPrefs.SetInt(HINTS_PURCHASED_KEY, purchasedHints);
        }
        PlayerPrefs.Save();

        // Play hint sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("tap");

        // Find hint tile (basic: find a correct tile to suggest)
        int hintX, hintY;
        FindHintTile(out hintX, out hintY);

        // Show hint visual
        StartCoroutine(ShowHintVisual(hintX, hintY));

        // Pitou hint message
        if (PitouManager.Instance != null)
        {
            string msg = HintMessages[UnityEngine.Random.Range(0, HintMessages.Length)];
            PitouManager.Instance.ShowSpeechBubble(msg);
            PitouManager.Instance.TriggerReaction(PitouManager.ReactionType.Thinking);
        }

        // Log analytics
        if (Analytics.Instance != null)
        {
            int currentLevel = PuzzleGame.Instance != null ? PuzzleGame.Instance.GetCurrentLevel().levelId : 0;
            Analytics.Instance.LogHintUsed(currentLevel, GetTotalHints());
        }

        OnHintUsed?.Invoke(hintX, hintY);
        UpdateUI();

        Debug.Log($"[Hints] Hint used! Remaining: {GetTotalHints()} (Free: {freeHintsRemaining}, Bought: {purchasedHints})");
    }

    /// <summary>
    /// Find a tile to suggest as hint
    /// </summary>
    private void FindHintTile(out int hintX, out int hintY)
    {
        hintX = -1;
        hintY = -1;

        if (PuzzleGame.Instance == null) return;

        PuzzleGame.PuzzleLevel level = PuzzleGame.Instance.GetCurrentLevel();
        PuzzleGame.Tile[,] board = PuzzleGame.Instance.GetBoard();

        if (level == null || board == null) return;

        // Simple hint: find a non-selected tile with high value
        int bestValue = -1;
        for (int x = 0; x < level.gridWidth; x++)
        {
            for (int y = 0; y < level.gridHeight; y++)
            {
                PuzzleGame.Tile tile = board[x, y];
                if (!tile.isSelected && !tile.isLocked && tile.value > bestValue)
                {
                    bestValue = tile.value;
                    hintX = x;
                    hintY = y;
                }
            }
        }
    }

    /// <summary>
    /// Show hint visual effect (glow on tile)
    /// </summary>
    private IEnumerator ShowHintVisual(int x, int y)
    {
        isHintActive = true;

        if (hintGlowEffect != null)
        {
            hintGlowEffect.gameObject.SetActive(true);
        }

        // Glow effect through JuiceManager if available
        if (JuiceManager.Instance != null && hintGlowEffect != null)
        {
            JuiceManager.Instance.Glow(hintGlowEffect.transform);
            JuiceManager.Instance.Pulse(hintGlowEffect.transform, 1.2f, hintGlowDuration);
        }

        yield return new WaitForSeconds(hintGlowDuration);

        if (hintGlowEffect != null)
        {
            hintGlowEffect.gameObject.SetActive(false);
        }

        isHintActive = false;
    }

    /// <summary>
    /// Buy hint with rewarded ad
    /// </summary>
    public void BuyHintWithAd()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        // Use the new ad system
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd(reward =>
            {
                purchasedHints += hintsFromAd;
                PlayerPrefs.SetInt(HINTS_PURCHASED_KEY, purchasedHints);
                PlayerPrefs.Save();
                UpdateUI();
                Debug.Log($"[Hints] Ad watched! +{hintsFromAd} hint. Total: {GetTotalHints()}");
            });
        }
    }

    /// <summary>
    /// Buy hints with IAP
    /// </summary>
    public void BuyHintWithIAP()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (MonetizationManager.Instance != null)
        {
            MonetizationManager.Instance.PurchaseProduct("hint_pack_5");
            MonetizationManager.Instance.OnPurchaseSuccess += OnHintPurchaseSuccess;
        }
    }

    private void OnHintPurchaseSuccess(string productId, int quantity)
    {
        if (productId.Contains("hint"))
        {
            purchasedHints += hintsFromIAP;
            PlayerPrefs.SetInt(HINTS_PURCHASED_KEY, purchasedHints);
            PlayerPrefs.Save();
            UpdateUI();
            Debug.Log($"[Hints] IAP purchased! +{hintsFromIAP} hints. Total: {GetTotalHints()}");
        }

        if (MonetizationManager.Instance != null)
        {
            MonetizationManager.Instance.OnPurchaseSuccess -= OnHintPurchaseSuccess;
        }
    }

    /// <summary>
    /// Update UI
    /// </summary>
    private void UpdateUI()
    {
        int total = GetTotalHints();

        if (hintCountText != null)
            hintCountText.text = $"💡 {total}";

        if (hintButton != null)
            hintButton.interactable = total > 0 && !isHintActive;
    }

    // Public getters
    public int GetFreeHintsRemaining() => freeHintsRemaining;
    public int GetPurchasedHints() => purchasedHints;
    public bool IsHintActive() => isHintActive;
}
