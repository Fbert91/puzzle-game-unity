using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// World Map Manager - Manages 5 themed worlds with 40 levels each
/// Handles world unlocking, progress tracking, and star gating
/// </summary>
public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance { get; private set; }

    [System.Serializable]
    public class WorldData
    {
        public int worldIndex;
        public string worldName;
        public bool isUnlocked;
        public int starsEarned;
        public int starsRequired;
        public int levelsCompleted;
        public int totalLevels;
        public Color themeColor;
    }

    [System.Serializable]
    public class LevelNodeData
    {
        public int levelId;
        public int worldIndex;
        public int levelInWorld;
        public bool isUnlocked;
        public bool isCompleted;
        public int starsEarned;
        public bool isCurrent;
    }

    private List<WorldData> worlds = new List<WorldData>();
    private Dictionary<int, LevelNodeData> levelNodes = new Dictionary<int, LevelNodeData>();

    public event System.Action<int> OnWorldSelected;
    public event System.Action<int> OnLevelSelected;
    public event System.Action OnMapUpdated;

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
        InitializeWorldMap();
    }

    /// <summary>
    /// Initialize world map data from level generator and saved progress
    /// </summary>
    public void InitializeWorldMap()
    {
        worlds.Clear();
        levelNodes.Clear();

        int totalStars = GetTotalStars();

        for (int w = 0; w < 5; w++)
        {
            int starsRequired = ProceduralLevelGenerator.WorldStarRequirements[w];
            bool unlocked = totalStars >= starsRequired;

            WorldData world = new WorldData
            {
                worldIndex = w,
                worldName = ProceduralLevelGenerator.WorldNames[w],
                isUnlocked = unlocked,
                starsRequired = starsRequired,
                starsEarned = 0,
                levelsCompleted = 0,
                totalLevels = 40,
                themeColor = ProceduralLevelGenerator.WorldPrimaryColors[w]
            };

            // Load level nodes for this world
            for (int l = 1; l <= 40; l++)
            {
                int levelId = w * 40 + l;
                int stars = PlayerPrefs.GetInt($"Level_{levelId}_Stars", 0);
                bool completed = PlayerPrefs.GetInt($"Level_{levelId}_Completed", 0) == 1;
                bool levelUnlocked = (levelId == 1) || PlayerPrefs.GetInt($"Level_{levelId}_Unlocked", 0) == 1;

                // First level of unlocked world is always unlocked
                if (l == 1 && unlocked)
                    levelUnlocked = true;

                if (completed) world.levelsCompleted++;
                world.starsEarned += stars;

                LevelNodeData node = new LevelNodeData
                {
                    levelId = levelId,
                    worldIndex = w,
                    levelInWorld = l,
                    isUnlocked = levelUnlocked && unlocked,
                    isCompleted = completed,
                    starsEarned = stars,
                    isCurrent = false
                };
                levelNodes[levelId] = node;
            }

            worlds.Add(world);
        }

        // Mark current level (first uncompleted unlocked level)
        MarkCurrentLevel();

        OnMapUpdated?.Invoke();
        Debug.Log($"[WorldMapManager] Initialized world map. Total stars: {totalStars}");
    }

    /// <summary>
    /// Mark the current (next playable) level
    /// </summary>
    private void MarkCurrentLevel()
    {
        foreach (var node in levelNodes.Values)
        {
            node.isCurrent = false;
        }

        for (int i = 1; i <= 200; i++)
        {
            if (levelNodes.ContainsKey(i) && levelNodes[i].isUnlocked && !levelNodes[i].isCompleted)
            {
                levelNodes[i].isCurrent = true;
                break;
            }
        }
    }

    /// <summary>
    /// Get total stars earned across all levels
    /// </summary>
    public int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= 200; i++)
        {
            total += PlayerPrefs.GetInt($"Level_{i}_Stars", 0);
        }
        return total;
    }

    /// <summary>
    /// Get world data
    /// </summary>
    public WorldData GetWorldData(int worldIndex)
    {
        if (worldIndex >= 0 && worldIndex < worlds.Count)
            return worlds[worldIndex];
        return null;
    }

    /// <summary>
    /// Get all world data
    /// </summary>
    public List<WorldData> GetAllWorlds() => new List<WorldData>(worlds);

    /// <summary>
    /// Get level node data
    /// </summary>
    public LevelNodeData GetLevelNode(int levelId)
    {
        if (levelNodes.ContainsKey(levelId))
            return levelNodes[levelId];
        return null;
    }

    /// <summary>
    /// Get all level nodes for a world
    /// </summary>
    public List<LevelNodeData> GetWorldLevelNodes(int worldIndex)
    {
        List<LevelNodeData> nodes = new List<LevelNodeData>();
        int startId = worldIndex * 40 + 1;
        int endId = startId + 39;

        for (int i = startId; i <= endId; i++)
        {
            if (levelNodes.ContainsKey(i))
                nodes.Add(levelNodes[i]);
        }
        return nodes;
    }

    /// <summary>
    /// Complete a level and update world map
    /// </summary>
    public void OnLevelCompleted(int levelId, int stars)
    {
        if (!levelNodes.ContainsKey(levelId)) return;

        LevelNodeData node = levelNodes[levelId];
        node.isCompleted = true;
        node.starsEarned = Mathf.Max(node.starsEarned, stars);

        // Save progress
        PlayerPrefs.SetInt($"Level_{levelId}_Completed", 1);
        PlayerPrefs.SetInt($"Level_{levelId}_Stars", node.starsEarned);

        // Unlock next level
        int nextLevelId = levelId + 1;
        if (nextLevelId <= 200 && levelNodes.ContainsKey(nextLevelId))
        {
            levelNodes[nextLevelId].isUnlocked = true;
            PlayerPrefs.SetInt($"Level_{nextLevelId}_Unlocked", 1);
        }

        PlayerPrefs.Save();

        // Refresh world data
        InitializeWorldMap();
    }

    /// <summary>
    /// Check if a world is unlocked
    /// </summary>
    public bool IsWorldUnlocked(int worldIndex)
    {
        if (worldIndex >= 0 && worldIndex < worlds.Count)
            return worlds[worldIndex].isUnlocked;
        return false;
    }

    /// <summary>
    /// Select a world to view
    /// </summary>
    public void SelectWorld(int worldIndex)
    {
        OnWorldSelected?.Invoke(worldIndex);
    }

    /// <summary>
    /// Select a level to play
    /// </summary>
    public void SelectLevel(int levelId)
    {
        if (levelNodes.ContainsKey(levelId) && levelNodes[levelId].isUnlocked)
        {
            OnLevelSelected?.Invoke(levelId);
        }
    }

    /// <summary>
    /// Get world completion percentage
    /// </summary>
    public float GetWorldCompletion(int worldIndex)
    {
        WorldData world = GetWorldData(worldIndex);
        if (world == null || world.totalLevels == 0) return 0f;
        return (float)world.levelsCompleted / world.totalLevels * 100f;
    }
}
