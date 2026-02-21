using UnityEngine;

/// <summary>
/// GameInitializer - Bootstraps the game on startup
/// Ensures all managers are initialized in correct order
/// Updated to include all P0 feature managers
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        // Core managers (order matters)
        EnsureManager<ProceduralLevelGenerator>("ProceduralLevelGenerator");
        EnsureManager<LevelManager>("LevelManager");
        EnsureManager<PuzzleGame>("PuzzleGame");
        EnsureManager<MonetizationManager>("MonetizationManager");
        EnsureManager<Analytics>("Analytics");
        EnsureManager<AudioManager>("AudioManager");

        // P0 Feature managers
        EnsureManager<ThemeManager>("ThemeManager");
        EnsureManager<SoundManager>("SoundManager");
        EnsureManager<JuiceManager>("JuiceManager");
        EnsureManager<WorldMapManager>("WorldMapManager");
        EnsureManager<LevelCompleteManager>("LevelCompleteManager");
        EnsureManager<TutorialManager>("TutorialManager");
        EnsureManager<DailyPuzzleManager>("DailyPuzzleManager");
        EnsureManager<StreakManager>("StreakManager");
        EnsureManager<HintManager>("HintManager");
        EnsureManager<PitouManager>("PitouManager");
        EnsureManager<ShareCardGenerator>("ShareCardGenerator");

        if (debugMode)
        {
            Debug.Log("[GameInitializer] All systems initialized successfully (P0 features included)");
        }
    }

    private void EnsureManager<T>(string name) where T : MonoBehaviour
    {
        T manager = FindObjectOfType<T>();
        
        if (manager == null)
        {
            GameObject managerObj = new GameObject(name);
            manager = managerObj.AddComponent<T>();
            if (debugMode)
                Debug.LogWarning($"[GameInitializer] Created missing manager: {name}");
        }
    }

    private void Start()
    {
        // Set quality settings for mobile
        QualitySettings.vSyncCount = 1; // 60 FPS lock
        
        // Initialize audio system and play menu music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }

        // Apply theme
        if (ThemeManager.Instance != null)
        {
            // Theme auto-applies on Start
        }

        // Check daily puzzle availability
        if (DailyPuzzleManager.Instance != null && !DailyPuzzleManager.Instance.IsTodayCompleted())
        {
            if (PitouManager.Instance != null)
                PitouManager.Instance.OnDailyPuzzleAvailable();
        }
        
        // Log device info
        if (debugMode)
        {
            Debug.Log($"Device: {SystemInfo.deviceModel}");
            Debug.Log($"OS: {SystemInfo.operatingSystem}");
            Debug.Log($"RAM: {SystemInfo.systemMemorySize} MB");
            Debug.Log($"GPU: {SystemInfo.graphicsDeviceName}");
        }
    }
}
