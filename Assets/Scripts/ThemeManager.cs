using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Theme Manager - Light (default) and Dark theme system
/// Light theme: cream white, soft purple, bright cyan, gold, soft red
/// Dark theme: unlocked at World 2 completion
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [System.Serializable]
    public class ThemeData
    {
        public string themeName;
        public Color backgroundColor;
        public Color primaryColor;
        public Color accentColor;
        public Color successColor;
        public Color errorColor;
        public Color textColor;
        public Color secondaryTextColor;
        public Color cardColor;
        public Color buttonTextColor;
    }

    public enum Theme
    {
        Light,
        Dark
    }

    [Header("Theme Definitions")]
    [SerializeField] private ThemeData lightTheme = new ThemeData
    {
        themeName = "Light",
        backgroundColor = new Color(1f, 0.973f, 0.941f),       // #FFF8F0
        primaryColor = new Color(0.608f, 0.498f, 1f),           // #9B7FFF
        accentColor = new Color(0f, 0.898f, 1f),                // #00E5FF
        successColor = new Color(1f, 0.843f, 0f),               // #FFD700
        errorColor = new Color(1f, 0.42f, 0.42f),               // #FF6B6B
        textColor = new Color(0.2f, 0.2f, 0.2f),                // Dark text
        secondaryTextColor = new Color(0.5f, 0.5f, 0.5f),       // Gray text
        cardColor = new Color(1f, 1f, 1f, 0.9f),                // White card
        buttonTextColor = Color.white
    };

    [SerializeField] private ThemeData darkTheme = new ThemeData
    {
        themeName = "Dark",
        backgroundColor = new Color(0.12f, 0.1f, 0.2f),        // Dark purple
        primaryColor = new Color(0.608f, 0.498f, 1f),           // #9B7FFF
        accentColor = new Color(0f, 0.898f, 1f),                // #00E5FF
        successColor = new Color(1f, 0.843f, 0f),               // #FFD700
        errorColor = new Color(1f, 0.42f, 0.42f),               // #FF6B6B
        textColor = new Color(0.95f, 0.95f, 0.95f),             // Light text
        secondaryTextColor = new Color(0.7f, 0.7f, 0.7f),       // Light gray
        cardColor = new Color(0.18f, 0.15f, 0.28f, 0.9f),       // Dark card
        buttonTextColor = Color.white
    };

    private const string THEME_PREF_KEY = "SelectedTheme";
    private const string DARK_UNLOCKED_KEY = "DarkThemeUnlocked";

    private Theme currentTheme = Theme.Light;
    private bool isDarkUnlocked = false;

    // Registered UI elements to update
    private List<ThemeTarget> registeredTargets = new List<ThemeTarget>();

    public event System.Action<ThemeData> OnThemeChanged;

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
        isDarkUnlocked = PlayerPrefs.GetInt(DARK_UNLOCKED_KEY, 0) == 1;
        int savedTheme = PlayerPrefs.GetInt(THEME_PREF_KEY, 0);
        currentTheme = (Theme)savedTheme;

        // Force light if dark not unlocked
        if (currentTheme == Theme.Dark && !isDarkUnlocked)
            currentTheme = Theme.Light;

        ApplyTheme();
    }

    /// <summary>
    /// Get current theme data
    /// </summary>
    public ThemeData GetCurrentTheme()
    {
        return currentTheme == Theme.Light ? lightTheme : darkTheme;
    }

    /// <summary>
    /// Get specific theme data
    /// </summary>
    public ThemeData GetThemeData(Theme theme)
    {
        return theme == Theme.Light ? lightTheme : darkTheme;
    }

    /// <summary>
    /// Set the active theme
    /// </summary>
    public void SetTheme(Theme theme)
    {
        if (theme == Theme.Dark && !isDarkUnlocked)
        {
            Debug.Log("[Theme] Dark theme is locked! Complete World 2 to unlock.");
            return;
        }

        currentTheme = theme;
        PlayerPrefs.SetInt(THEME_PREF_KEY, (int)theme);
        PlayerPrefs.Save();

        ApplyTheme();
        Debug.Log($"[Theme] Switched to {theme} theme");
    }

    /// <summary>
    /// Toggle between themes
    /// </summary>
    public void ToggleTheme()
    {
        if (currentTheme == Theme.Light && isDarkUnlocked)
            SetTheme(Theme.Dark);
        else
            SetTheme(Theme.Light);
    }

    /// <summary>
    /// Apply theme to all registered targets
    /// </summary>
    private void ApplyTheme()
    {
        ThemeData theme = GetCurrentTheme();

        // Apply to camera background
        Camera cam = Camera.main;
        if (cam != null)
            cam.backgroundColor = theme.backgroundColor;

        // Notify all registered targets
        foreach (var target in registeredTargets)
        {
            if (target != null)
                target.ApplyTheme(theme);
        }

        OnThemeChanged?.Invoke(theme);
    }

    /// <summary>
    /// Register a UI element to receive theme updates
    /// </summary>
    public void RegisterTarget(ThemeTarget target)
    {
        if (!registeredTargets.Contains(target))
        {
            registeredTargets.Add(target);
            target.ApplyTheme(GetCurrentTheme());
        }
    }

    /// <summary>
    /// Unregister a UI element
    /// </summary>
    public void UnregisterTarget(ThemeTarget target)
    {
        registeredTargets.Remove(target);
    }

    /// <summary>
    /// Clean up null references
    /// </summary>
    private void CleanupTargets()
    {
        registeredTargets.RemoveAll(t => t == null);
    }

    /// <summary>
    /// Unlock dark theme (called when World 2 is completed)
    /// </summary>
    public void UnlockDarkTheme()
    {
        isDarkUnlocked = true;
        PlayerPrefs.SetInt(DARK_UNLOCKED_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("[Theme] Dark theme unlocked! 🌙");
    }

    /// <summary>
    /// Check if dark theme is unlocked
    /// </summary>
    public bool IsDarkThemeUnlocked() => isDarkUnlocked;

    /// <summary>
    /// Get current theme enum
    /// </summary>
    public Theme GetCurrentThemeType() => currentTheme;

    /// <summary>
    /// Convenience color getters
    /// </summary>
    public Color BackgroundColor => GetCurrentTheme().backgroundColor;
    public Color PrimaryColor => GetCurrentTheme().primaryColor;
    public Color AccentColor => GetCurrentTheme().accentColor;
    public Color SuccessColor => GetCurrentTheme().successColor;
    public Color ErrorColor => GetCurrentTheme().errorColor;
    public Color TextColor => GetCurrentTheme().textColor;
}

/// <summary>
/// Component to attach to UI elements for automatic theme application
/// </summary>
public class ThemeTarget : MonoBehaviour
{
    public enum TargetType
    {
        Background,
        Primary,
        Accent,
        Success,
        Error,
        Text,
        SecondaryText,
        Card,
        ButtonText
    }

    [SerializeField] private TargetType targetType = TargetType.Text;
    [SerializeField] private bool applyToImage = false;
    [SerializeField] private bool applyToText = false;

    private Image targetImage;
    private Text targetText;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        targetText = GetComponent<Text>();

        if (targetImage != null) applyToImage = true;
        if (targetText != null) applyToText = true;
    }

    private void OnEnable()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.RegisterTarget(this);
    }

    private void OnDisable()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.UnregisterTarget(this);
    }

    /// <summary>
    /// Apply theme colors to this element
    /// </summary>
    public void ApplyTheme(ThemeManager.ThemeData theme)
    {
        Color color = GetColorForType(theme);

        if (applyToImage && targetImage != null)
            targetImage.color = color;

        if (applyToText && targetText != null)
            targetText.color = color;
    }

    private Color GetColorForType(ThemeManager.ThemeData theme)
    {
        switch (targetType)
        {
            case TargetType.Background: return theme.backgroundColor;
            case TargetType.Primary: return theme.primaryColor;
            case TargetType.Accent: return theme.accentColor;
            case TargetType.Success: return theme.successColor;
            case TargetType.Error: return theme.errorColor;
            case TargetType.Text: return theme.textColor;
            case TargetType.SecondaryText: return theme.secondaryTextColor;
            case TargetType.Card: return theme.cardColor;
            case TargetType.ButtonText: return theme.buttonTextColor;
            default: return Color.white;
        }
    }
}
