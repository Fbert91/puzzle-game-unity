using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Manages level data, progression, and achievements
/// </summary>
public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelProgress
    {
        public int levelId;
        public bool completed;
        public int starsEarned; // 0-3
        public int bestScore;
        public bool unlocked;
    }

    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<PuzzleGame.PuzzleLevel> levels = new List<PuzzleGame.PuzzleLevel>();
    private Dictionary<int, LevelProgress> progressData = new Dictionary<int, LevelProgress>();

    public event Action<int> OnLevelUnlocked;
    public event Action<int, int> OnLevelCompleted; // levelId, stars

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
        InitializeLevels();
        LoadProgressData();
    }

    /// <summary>
    /// Initialize all 20+ levels with progressive difficulty
    /// </summary>
    private void InitializeLevels()
    {
        levels.Clear();

        // Tutorial levels (1-5): Easy, teach mechanics
        CreateLevel(1, "SumToTen", 5, 5, new int[] { 1, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 1, 1, 2, 2 }, 10, 1);
        CreateLevel(2, "SumToTen", 5, 5, new int[] { 2, 3, 1, 4, 2, 3, 2, 1, 5, 1, 1, 2, 3, 4, 5, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3 }, 10, 1);
        CreateLevel(3, "ConnectPatterns", 5, 5, new int[] { 1, 1, 2, 2, 3, 1, 2, 3, 1, 2, 2, 3, 1, 3, 2, 1, 2, 3, 1, 1, 2, 2, 1, 3, 3 }, 10, 1);
        CreateLevel(4, "SequenceOrder", 5, 5, new int[] { 5, 4, 3, 2, 1, 1, 2, 3, 4, 5, 2, 3, 1, 5, 4, 3, 4, 2, 1, 5, 1, 1, 2, 3, 4 }, 10, 1);
        CreateLevel(5, "SumToTen", 5, 5, new int[] { 1, 2, 3, 2, 1, 4, 5, 1, 2, 3, 1, 2, 3, 4, 5, 2, 1, 4, 3, 2, 1, 3, 2, 1, 4 }, 10, 1);

        // Standard levels (6-15): Medium, fun variety
        CreateLevel(6, "SumToTen", 6, 6, GenerateRandomBoard(36, 9), 15, 2);
        CreateLevel(7, "ConnectPatterns", 6, 6, GenerateRandomBoard(36, 3), 15, 2);
        CreateLevel(8, "SumToTen", 6, 6, GenerateRandomBoard(36, 9), 20, 2);
        CreateLevel(9, "SequenceOrder", 6, 6, GenerateRandomBoard(36, 9), 15, 2);
        CreateLevel(10, "SumToTen", 5, 5, GenerateRandomBoard(25, 9), 15, 2);
        CreateLevel(11, "ConnectPatterns", 6, 6, GenerateRandomBoard(36, 3), 15, 2);
        CreateLevel(12, "SumToTen", 6, 6, GenerateRandomBoard(36, 9), 18, 2);
        CreateLevel(13, "SequenceOrder", 5, 5, GenerateRandomBoard(25, 9), 15, 2);
        CreateLevel(14, "SumToTen", 6, 6, GenerateRandomBoard(36, 10), 25, 2);
        CreateLevel(15, "ConnectPatterns", 6, 6, GenerateRandomBoard(36, 5), 15, 2);

        // Challenging levels (16-20): Hard, requires strategy
        CreateLevel(16, "SumToTen", 6, 6, GenerateRandomBoard(36, 12), 30, 3);
        CreateLevel(17, "ConnectPatterns", 6, 6, GenerateRandomBoard(36, 4), 20, 3);
        CreateLevel(18, "SequenceOrder", 6, 6, GenerateRandomBoard(36, 12), 20, 3);
        CreateLevel(19, "SumToTen", 6, 6, GenerateRandomBoard(36, 15), 35, 3);
        CreateLevel(20, "ConnectPatterns", 6, 6, GenerateRandomBoard(36, 6), 20, 3);

        Debug.Log($"Initialized {levels.Count} levels");
    }

    private void CreateLevel(int id, string rule, int width, int height, int[] tiles, int targetSum, int difficulty)
    {
        var level = new PuzzleGame.PuzzleLevel
        {
            levelId = id,
            gridWidth = width,
            gridHeight = height,
            puzzleRule = rule,
            initialTiles = tiles,
            targetSum = targetSum,
            difficulty = difficulty
        };
        levels.Add(level);
    }

    private int[] GenerateRandomBoard(int tileCount, int maxValue)
    {
        int[] board = new int[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            board[i] = Random.Range(1, maxValue + 1);
        }
        return board;
    }

    /// <summary>
    /// Get a level by ID
    /// </summary>
    public PuzzleGame.PuzzleLevel GetLevel(int levelId)
    {
        return levels.FirstOrDefault(l => l.levelId == levelId);
    }

    /// <summary>
    /// Complete a level
    /// </summary>
    public void CompleteLevel(int levelId, int score, int stars)
    {
        if (!progressData.ContainsKey(levelId))
        {
            progressData[levelId] = new LevelProgress { levelId = levelId };
        }

        LevelProgress progress = progressData[levelId];
        progress.completed = true;
        progress.starsEarned = Mathf.Max(progress.starsEarned, stars);
        progress.bestScore = Mathf.Max(progress.bestScore, score);

        // Unlock next level
        int nextLevelId = levelId + 1;
        if (GetLevel(nextLevelId) != null)
        {
            if (!progressData.ContainsKey(nextLevelId))
            {
                progressData[nextLevelId] = new LevelProgress
                {
                    levelId = nextLevelId,
                    unlocked = true,
                    completed = false
                };
            }
            else
            {
                progressData[nextLevelId].unlocked = true;
            }
            OnLevelUnlocked?.Invoke(nextLevelId);
        }

        SaveProgressData();
        OnLevelCompleted?.Invoke(levelId, stars);
    }

    /// <summary>
    /// Get progress for a level
    /// </summary>
    public LevelProgress GetLevelProgress(int levelId)
    {
        if (progressData.ContainsKey(levelId))
            return progressData[levelId];
        return null;
    }

    /// <summary>
    /// Get overall progress
    /// </summary>
    public int GetCompletedLevelCount()
    {
        return progressData.Count(p => p.Value.completed);
    }

    public int GetTotalLevelCount() => levels.Count;

    public float GetCompletionPercentage()
    {
        int completed = GetCompletedLevelCount();
        int total = GetTotalLevelCount();
        return total > 0 ? (completed / (float)total) * 100f : 0f;
    }

    /// <summary>
    /// Get all levels (for level select screen)
    /// </summary>
    public List<PuzzleGame.PuzzleLevel> GetAllLevels() => new List<PuzzleGame.PuzzleLevel>(levels);

    private void LoadProgressData()
    {
        // Load from PlayerPrefs or JSON
        string json = PlayerPrefs.GetString("LevelProgress", "");
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                // Simple load from PlayerPrefs
                for (int i = 1; i <= levels.Count; i++)
                {
                    LevelProgress progress = new LevelProgress
                    {
                        levelId = i,
                        completed = PlayerPrefs.GetInt($"Level_{i}_Completed", 0) == 1,
                        starsEarned = PlayerPrefs.GetInt($"Level_{i}_Stars", 0),
                        bestScore = PlayerPrefs.GetInt($"Level_{i}_Score", 0),
                        unlocked = (i == 1) || (i > 1 && PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1)
                    };
                    progressData[i] = progress;
                }
            }
            catch
            {
                Debug.LogWarning("Failed to load progress data");
            }
        }

        // Ensure level 1 is always unlocked
        if (!progressData.ContainsKey(1))
        {
            progressData[1] = new LevelProgress { levelId = 1, unlocked = true };
        }
    }

    private void SaveProgressData()
    {
        foreach (var kvp in progressData)
        {
            LevelProgress progress = kvp.Value;
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Completed", progress.completed ? 1 : 0);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Stars", progress.starsEarned);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Score", progress.bestScore);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Unlocked", progress.unlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public bool IsLevelUnlocked(int levelId)
    {
        return progressData.ContainsKey(levelId) && progressData[levelId].unlocked;
    }
}
