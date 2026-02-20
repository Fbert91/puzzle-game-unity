using UnityEngine;

/// <summary>
/// GameInitializer - Bootstraps the game on startup
/// Ensures all managers are initialized in correct order
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        // Ensure managers exist and are initialized
        EnsureManager<LevelManager>("LevelManager");
        EnsureManager<PuzzleGame>("PuzzleGame");
        EnsureManager<MonetizationManager>("MonetizationManager");
        EnsureManager<Analytics>("Analytics");

        if (debugMode)
        {
            Debug.Log("[GameInitializer] All systems initialized successfully");
        }
    }

    private void EnsureManager<T>(string name) where T : MonoBehaviour
    {
        T manager = FindObjectOfType<T>();
        
        if (manager == null)
        {
            GameObject managerObj = new GameObject(name);
            manager = managerObj.AddComponent<T>();
            Debug.LogWarning($"[GameInitializer] Created missing manager: {name}");
        }
    }

    private void Start()
    {
        // Set quality settings for mobile
        QualitySettings.vSyncCount = 1; // 60 FPS lock
        
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
