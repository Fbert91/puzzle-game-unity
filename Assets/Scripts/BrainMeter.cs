using UnityEngine;

/// <summary>
/// Persistent "Brain Score" / IQ meter that grows as player clears harder worlds.
/// Stored in PlayerPrefs, displayed on main menu.
/// </summary>
public class BrainMeter : MonoBehaviour
{
    public static BrainMeter Instance { get; private set; }

    private const string BRAIN_SCORE_KEY = "BrainScore";
    private const string HIGHEST_WORLD_KEY = "HighestWorld";
    private const string LEVELS_COMPLETED_KEY = "LevelsCompleted";
    private const string PERFECT_CLEARS_KEY = "PerfectClears";

    public int BrainScore { get; private set; }
    public int HighestWorld { get; private set; }
    public int LevelsCompleted { get; private set; }
    public int PerfectClears { get; private set; }

    public string BrainRank
    {
        get
        {
            if (BrainScore >= 5000) return "Genius";
            if (BrainScore >= 3000) return "Brilliant";
            if (BrainScore >= 1500) return "Sharp";
            if (BrainScore >= 500) return "Rising";
            return "Beginner";
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadData();
    }

    private void LoadData()
    {
        BrainScore = PlayerPrefs.GetInt(BRAIN_SCORE_KEY, 0);
        HighestWorld = PlayerPrefs.GetInt(HIGHEST_WORLD_KEY, 0);
        LevelsCompleted = PlayerPrefs.GetInt(LEVELS_COMPLETED_KEY, 0);
        PerfectClears = PlayerPrefs.GetInt(PERFECT_CLEARS_KEY, 0);
    }

    /// <summary>
    /// Called after completing a level. Awards brain points based on performance.
    /// </summary>
    public void OnLevelComplete(int worldIndex, int stars, int score, bool perfectClear)
    {
        // Base brain points per star
        int brainPoints = stars * 10;

        // World multiplier (harder worlds = more brain points)
        brainPoints *= (1 + worldIndex);

        // Perfect clear bonus
        if (perfectClear)
        {
            brainPoints += 25;
            PerfectClears++;
            PlayerPrefs.SetInt(PERFECT_CLEARS_KEY, PerfectClears);
        }

        // Score bonus (every 200 score = +5 brain)
        brainPoints += (score / 200) * 5;

        BrainScore += brainPoints;
        LevelsCompleted++;

        if (worldIndex > HighestWorld)
            HighestWorld = worldIndex;

        PlayerPrefs.SetInt(BRAIN_SCORE_KEY, BrainScore);
        PlayerPrefs.SetInt(HIGHEST_WORLD_KEY, HighestWorld);
        PlayerPrefs.SetInt(LEVELS_COMPLETED_KEY, LevelsCompleted);
        PlayerPrefs.Save();

        Debug.Log($"[BrainMeter] +{brainPoints} brain points! Total: {BrainScore} ({BrainRank})");
    }
}
