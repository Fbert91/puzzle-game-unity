using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Procedural Level Generator - Generates 200+ puzzle levels with progressive difficulty
/// 5 worlds (40 levels each): Garden, Ocean, Space, Jungle, Crystal Cave
/// Difficulty curve: Easy (1-40), Medium (41-80), Hard (81-140), Expert (141-200)
/// New mechanics introduced every 15 levels
/// </summary>
public class ProceduralLevelGenerator : MonoBehaviour
{
    public static ProceduralLevelGenerator Instance { get; private set; }

    [System.Serializable]
    public class LevelData
    {
        public int levelId;
        public int worldIndex;
        public string worldName;
        public int levelInWorld;
        public string difficulty;
        public int difficultyNum;
        public int gridWidth;
        public int gridHeight;
        public string puzzleRule;
        public int targetSum;
        public int targetMoves;
        public int timeLimit;
        public StarThresholds starThresholds;
        public string[] activeMechanics;
        public int[] initialTiles;
    }

    [System.Serializable]
    public class StarThresholds
    {
        public int star1;
        public int star2;
        public int star3;
    }

    [System.Serializable]
    public class LevelDataCollection
    {
        public LevelData[] levels;
    }

    public enum WorldTheme
    {
        Garden = 0,
        Ocean = 1,
        Space = 2,
        Jungle = 3,
        CrystalCave = 4
    }

    private LevelDataCollection levelCollection;
    private Dictionary<int, LevelData> levelLookup = new Dictionary<int, LevelData>();
    private bool isLoaded = false;

    // World star requirements to unlock
    public static readonly int[] WorldStarRequirements = { 0, 30, 80, 150, 250 };

    // World color schemes
    public static readonly Color[] WorldPrimaryColors = {
        new Color(0.2f, 0.8f, 0.3f),   // Garden - Green
        new Color(0.2f, 0.5f, 0.9f),   // Ocean - Blue
        new Color(0.6f, 0.3f, 0.9f),   // Space - Purple
        new Color(1.0f, 0.6f, 0.2f),   // Jungle - Orange
        new Color(0.0f, 0.9f, 0.9f)    // Crystal Cave - Cyan
    };

    public static readonly string[] WorldNames = { "Garden", "Ocean", "Space", "Jungle", "Crystal Cave" };

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
        LoadLevelData();
    }

    /// <summary>
    /// Load level data from JSON in Resources
    /// </summary>
    public void LoadLevelData()
    {
        if (isLoaded) return;

        TextAsset jsonFile = Resources.Load<TextAsset>("LevelData/levels");
        if (jsonFile != null)
        {
            levelCollection = JsonUtility.FromJson<LevelDataCollection>(jsonFile.text);

            levelLookup.Clear();
            foreach (var level in levelCollection.levels)
            {
                levelLookup[level.levelId] = level;
            }

            isLoaded = true;
            Debug.Log($"[ProceduralLevelGenerator] Loaded {levelCollection.levels.Length} levels from JSON");
        }
        else
        {
            Debug.LogWarning("[ProceduralLevelGenerator] Level data JSON not found, generating procedurally");
            GenerateLevels();
        }
    }

    /// <summary>
    /// Fallback procedural generation if JSON not found
    /// </summary>
    private void GenerateLevels()
    {
        string[] puzzleRules = { "SumToTen", "ConnectPatterns", "SequenceOrder", "ColorMatch", "MultiStep" };
        string[] mechanics = {
            "basic_tap", "color_matching", "pattern_logic", "timed_elements",
            "multi_step", "chain_reaction", "mirror_symmetry", "rotation",
            "gravity_shift", "cascade", "locked_tiles", "wildcard",
            "teleport", "bomb_tiles"
        };

        List<LevelData> levels = new List<LevelData>();

        for (int i = 1; i <= 200; i++)
        {
            int worldIdx = (i - 1) / 40;
            int levelInWorld = ((i - 1) % 40) + 1;

            int diffNum;
            string difficulty;
            if (i <= 40) { diffNum = 1; difficulty = "Easy"; }
            else if (i <= 80) { diffNum = 2; difficulty = "Medium"; }
            else if (i <= 140) { diffNum = 3; difficulty = "Hard"; }
            else { diffNum = 4; difficulty = "Expert"; }

            int gw, gh;
            switch (diffNum)
            {
                case 1: gw = 5; gh = 5; break;
                case 2: gw = 6; gh = 6; break;
                case 3: gw = levelInWorld % 2 == 0 ? 6 : 7; gh = levelInWorld % 2 == 0 ? 7 : 6; break;
                default: gw = 7; gh = 7; break;
            }

            int mechanicTier = Mathf.Min((i - 1) / 15, mechanics.Length - 1);
            int ruleIdx = (i - 1) % puzzleRules.Length;

            int baseMoves;
            switch (diffNum)
            {
                case 1: baseMoves = 15; break;
                case 2: baseMoves = 25; break;
                case 3: baseMoves = 35; break;
                default: baseMoves = 45; break;
            }
            int targetMoves = baseMoves + (levelInWorld % 10);

            int timeLimit = 0;
            if (i >= 46 && (i % 3 == 0))
                timeLimit = 60 + diffNum * 30;

            int star3 = Mathf.Max(5, targetMoves - 10 - diffNum * 2);
            int star2 = Mathf.Max(star3 + 3, targetMoves - 5);
            int star1 = targetMoves;

            int targetSum = 10 + diffNum * 5 + (levelInWorld % 5);

            // Generate seeded board
            int seedVal = i * 7919 + 42;
            int maxVal = 3 + diffNum * 2;
            int[] board = new int[gw * gh];
            for (int t = 0; t < board.Length; t++)
            {
                seedVal = (int)(((long)seedVal * 1103515245 + 12345) & 0x7fffffff);
                board[t] = (seedVal % maxVal) + 1;
            }

            List<string> activeMechanics = new List<string>();
            for (int m = 0; m <= mechanicTier; m++)
                activeMechanics.Add(mechanics[m]);

            LevelData level = new LevelData
            {
                levelId = i,
                worldIndex = worldIdx,
                worldName = WorldNames[worldIdx],
                levelInWorld = levelInWorld,
                difficulty = difficulty,
                difficultyNum = diffNum,
                gridWidth = gw,
                gridHeight = gh,
                puzzleRule = puzzleRules[ruleIdx],
                targetSum = targetSum,
                targetMoves = targetMoves,
                timeLimit = timeLimit,
                starThresholds = new StarThresholds { star1 = star1, star2 = star2, star3 = star3 },
                activeMechanics = activeMechanics.ToArray(),
                initialTiles = board
            };

            levels.Add(level);
            levelLookup[i] = level;
        }

        levelCollection = new LevelDataCollection { levels = levels.ToArray() };
        isLoaded = true;
        Debug.Log($"[ProceduralLevelGenerator] Generated {levels.Count} levels procedurally");
    }

    /// <summary>
    /// Get level data by ID
    /// </summary>
    public LevelData GetLevelData(int levelId)
    {
        if (!isLoaded) LoadLevelData();
        if (levelLookup.ContainsKey(levelId))
            return levelLookup[levelId];
        return null;
    }

    /// <summary>
    /// Get all levels for a specific world
    /// </summary>
    public List<LevelData> GetWorldLevels(int worldIndex)
    {
        if (!isLoaded) LoadLevelData();

        List<LevelData> worldLevels = new List<LevelData>();
        foreach (var level in levelCollection.levels)
        {
            if (level.worldIndex == worldIndex)
                worldLevels.Add(level);
        }
        return worldLevels;
    }

    /// <summary>
    /// Convert LevelData to PuzzleGame.PuzzleLevel for gameplay
    /// </summary>
    public PuzzleGame.PuzzleLevel ConvertToPuzzleLevel(LevelData data)
    {
        return new PuzzleGame.PuzzleLevel
        {
            levelId = data.levelId,
            gridWidth = data.gridWidth,
            gridHeight = data.gridHeight,
            puzzleRule = data.puzzleRule,
            initialTiles = data.initialTiles,
            targetSum = data.targetSum,
            difficulty = data.difficultyNum
        };
    }

    /// <summary>
    /// Get total level count
    /// </summary>
    public int GetTotalLevelCount()
    {
        if (!isLoaded) LoadLevelData();
        return levelCollection != null ? levelCollection.levels.Length : 0;
    }

    /// <summary>
    /// Get world count
    /// </summary>
    public int GetWorldCount() => 5;

    /// <summary>
    /// Get world color
    /// </summary>
    public Color GetWorldColor(int worldIndex)
    {
        if (worldIndex >= 0 && worldIndex < WorldPrimaryColors.Length)
            return WorldPrimaryColors[worldIndex];
        return Color.white;
    }

    /// <summary>
    /// Get star requirement to unlock a world
    /// </summary>
    public int GetWorldStarRequirement(int worldIndex)
    {
        if (worldIndex >= 0 && worldIndex < WorldStarRequirements.Length)
            return WorldStarRequirements[worldIndex];
        return 999;
    }
}
