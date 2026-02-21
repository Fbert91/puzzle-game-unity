using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Manages level data, progression, and achievements
/// Updated to support 200 levels via ProceduralLevelGenerator
/// 3-Star replay system with star gating per world
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
        public int bestMoves;
        public float bestTime;
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
    /// Initialize all 200 levels from ProceduralLevelGenerator
    /// </summary>
    private void InitializeLevels()
    {
        levels.Clear();

        // If ProceduralLevelGenerator is available, use it
        if (ProceduralLevelGenerator.Instance != null)
        {
            ProceduralLevelGenerator.Instance.LoadLevelData();
            int totalLevels = ProceduralLevelGenerator.Instance.GetTotalLevelCount();

            for (int i = 1; i <= totalLevels; i++)
            {
                var levelData = ProceduralLevelGenerator.Instance.GetLevelData(i);
                if (levelData != null)
                {
                    levels.Add(ProceduralLevelGenerator.Instance.ConvertToPuzzleLevel(levelData));
                }
            }

            Debug.Log($"[LevelManager] Loaded {levels.Count} levels from ProceduralLevelGenerator");
        }
        else
        {
            // Fallback: generate basic 20 levels as before
            GenerateFallbackLevels();
        }
    }

    /// <summary>
    /// Fallback level generation (original 20 levels)
    /// </summary>
    private void GenerateFallbackLevels()
    {
        CreateLevel(1, "SumToTen", 5, 5, new int[] { 1, 2, 3, 4, 5, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 1, 1, 2, 2 }, 10, 1);
        CreateLevel(2, "SumToTen", 5, 5, new int[] { 2, 3, 1, 4, 2, 3, 2, 1, 5, 1, 1, 2, 3, 4, 5, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3 }, 10, 1);
        CreateLevel(3, "ConnectPatterns", 5, 5, new int[] { 1, 1, 2, 2, 3, 1, 2, 3, 1, 2, 2, 3, 1, 3, 2, 1, 2, 3, 1, 1, 2, 2, 1, 3, 3 }, 10, 1);
        CreateLevel(4, "SequenceOrder", 5, 5, new int[] { 5, 4, 3, 2, 1, 1, 2, 3, 4, 5, 2, 3, 1, 5, 4, 3, 4, 2, 1, 5, 1, 1, 2, 3, 4 }, 10, 1);
        CreateLevel(5, "SumToTen", 5, 5, new int[] { 1, 2, 3, 2, 1, 4, 5, 1, 2, 3, 1, 2, 3, 4, 5, 2, 1, 4, 3, 2, 1, 3, 2, 1, 4 }, 10, 1);

        for (int i = 6; i <= 20; i++)
        {
            string rule = (i % 3 == 0) ? "SumToTen" : (i % 3 == 1) ? "ConnectPatterns" : "SequenceOrder";
            int diff = i <= 10 ? 2 : 3;
            int size = diff == 2 ? 6 : 7;
            CreateLevel(i, rule, size, size, GenerateRandomBoard(size * size, 5 + diff * 2), 10 + diff * 5, diff);
        }

        Debug.Log($"[LevelManager] Fallback: initialized {levels.Count} levels");
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
            board[i] = UnityEngine.Random.Range(1, maxValue + 1);
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
    /// Complete a level with star tracking
    /// </summary>
    public void CompleteLevel(int levelId, int score, int stars, int moves = 0, float time = 0f)
    {
        if (!progressData.ContainsKey(levelId))
        {
            progressData[levelId] = new LevelProgress { levelId = levelId };
        }

        LevelProgress progress = progressData[levelId];
        progress.completed = true;
        progress.starsEarned = Mathf.Max(progress.starsEarned, stars);
        progress.bestScore = Mathf.Max(progress.bestScore, score);
        if (moves > 0 && (progress.bestMoves == 0 || moves < progress.bestMoves))
            progress.bestMoves = moves;
        if (time > 0 && (progress.bestTime == 0 || time < progress.bestTime))
            progress.bestTime = time;

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

        // Update world map
        if (WorldMapManager.Instance != null)
            WorldMapManager.Instance.OnLevelCompleted(levelId, stars);

        // Update streak
        if (StreakManager.Instance != null)
            StreakManager.Instance.RecordPlay();

        // Check world unlock for dark theme
        if (levelId == 80 && stars > 0) // World 2 completed (level 80)
        {
            if (ThemeManager.Instance != null)
                ThemeManager.Instance.UnlockDarkTheme();
        }
    }

    /// <summary>
    /// Calculate stars for a level based on performance
    /// </summary>
    public int CalculateStars(int levelId, int moves, float time)
    {
        if (LevelCompleteManager.Instance != null)
            return LevelCompleteManager.Instance.CalculateStars(levelId, moves, time);

        // Fallback
        if (moves <= 20) return 3;
        if (moves <= 35) return 2;
        return 1;
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
    /// Get total stars earned
    /// </summary>
    public int GetTotalStars()
    {
        int total = 0;
        foreach (var p in progressData.Values)
        {
            total += p.starsEarned;
        }
        return total;
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
        progressData.Clear();

        for (int i = 1; i <= levels.Count; i++)
        {
            LevelProgress progress = new LevelProgress
            {
                levelId = i,
                completed = PlayerPrefs.GetInt($"Level_{i}_Completed", 0) == 1,
                starsEarned = PlayerPrefs.GetInt($"Level_{i}_Stars", 0),
                bestScore = PlayerPrefs.GetInt($"Level_{i}_Score", 0),
                bestMoves = PlayerPrefs.GetInt($"Level_{i}_BestMoves", 0),
                bestTime = PlayerPrefs.GetFloat($"Level_{i}_BestTime", 0f),
                unlocked = (i == 1) || (PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1)
            };
            progressData[i] = progress;
        }

        // Ensure level 1 is always unlocked
        if (progressData.ContainsKey(1))
            progressData[1].unlocked = true;
    }

    private void SaveProgressData()
    {
        foreach (var kvp in progressData)
        {
            LevelProgress progress = kvp.Value;
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Completed", progress.completed ? 1 : 0);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Stars", progress.starsEarned);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Score", progress.bestScore);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_BestMoves", progress.bestMoves);
            PlayerPrefs.SetFloat($"Level_{progress.levelId}_BestTime", progress.bestTime);
            PlayerPrefs.SetInt($"Level_{progress.levelId}_Unlocked", progress.unlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public bool IsLevelUnlocked(int levelId)
    {
        return progressData.ContainsKey(levelId) && progressData[levelId].unlocked;
    }
}
