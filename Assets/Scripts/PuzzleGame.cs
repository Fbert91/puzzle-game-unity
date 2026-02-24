using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Core puzzle game logic - Logic + Tile hybrid mechanic
/// Handles board state, tile manipulation, and puzzle solving
/// </summary>
public class PuzzleGame : MonoBehaviour
{
    [System.Serializable]
    public class Tile
    {
        public int value;
        public int x, y;
        public bool isSelected;
        public bool isLocked; // Can't be moved
    }

    [System.Serializable]
    public class PuzzleLevel
    {
        public int levelId;
        public int gridWidth = 5;
        public int gridHeight = 5;
        public string puzzleRule; // "SumToTen", "ConnectPatterns", "SequenceOrder"
        public int[] initialTiles;
        public int targetSum; // For sum-based puzzles
        public int requiredTiles; // 0 = any 2+, 3 = must pick exactly 3, etc.
        public string[] targetPattern; // For pattern-based puzzles
        public int difficulty; // 1=Easy, 2=Medium, 3=Hard
        public int timeLimit = 120;
        public int[] starThresholds; // [1star, 2star, 3star] score thresholds
    }

    public static PuzzleGame Instance { get; private set; }

    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float tilePadding = 0.1f;
    [SerializeField] private int currentLevelId = 1;

    private Tile[,] board;
    private PuzzleLevel currentLevel;
    private List<Tile> selectedTiles = new List<Tile>();
    private bool isPuzzleSolved = false;
    private int hintCount = 0;
    private int moveCount = 0;
    private int score = 0;
    private float sessionStartTime;

    public event Action OnPuzzleSolved;
    public event Action OnNoValidMoves;
    public event Action<Tile> OnTileSelected;
    public event Action OnBoardUpdated;
    public event Action<int> OnHintUsed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        sessionStartTime = Time.time;
        LoadLevel(currentLevelId);
    }

    /// <summary>
    /// Load a level and initialize the board
    /// </summary>
    public void LoadLevel(int levelId)
    {
        currentLevelId = levelId;
        currentLevel = LevelManager.Instance.GetLevel(levelId);
        
        if (currentLevel == null)
        {
            Debug.LogError($"Level {levelId} not found!");
            return;
        }

        InitializeBoard();
        isPuzzleSolved = false;
        selectedTiles.Clear();
        moveCount = 0;
        score = 0;
        hintCount = 0;

        OnBoardUpdated?.Invoke();
    }

    /// <summary>
    /// Initialize the board with tiles from the level data
    /// </summary>
    private void InitializeBoard()
    {
        int w = currentLevel.gridWidth;
        int h = currentLevel.gridHeight;
        board = new Tile[w, h];

        int tileIndex = 0;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                board[x, y] = new Tile
                {
                    value = currentLevel.initialTiles[tileIndex],
                    x = x,
                    y = y,
                    isSelected = false,
                    isLocked = false
                };
                tileIndex++;
            }
        }
    }

    /// <summary>
    /// Player selects/deselects a tile
    /// </summary>
    public void SelectTile(int x, int y)
    {
        if (isPuzzleSolved || x < 0 || x >= currentLevel.gridWidth || y < 0 || y >= currentLevel.gridHeight)
            return;

        Tile tile = board[x, y];
        
        if (tile.isLocked)
        {
            Debug.Log("Tile is locked!");
            return;
        }

        tile.isSelected = !tile.isSelected;

        if (tile.isSelected)
        {
            selectedTiles.Add(tile);
        }
        else
        {
            selectedTiles.Remove(tile);
        }

        OnTileSelected?.Invoke(tile);
        OnBoardUpdated?.Invoke();

        // Notify tutorial system
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialAction("tap");

        // Check if puzzle is solved after selection
        CheckPuzzleState();
    }

    /// <summary>
    /// Swap two adjacent tiles
    /// </summary>
    public void SwapTiles(int x1, int y1, int x2, int y2)
    {
        if (isPuzzleSolved) return;

        Tile tile1 = board[x1, y1];
        Tile tile2 = board[x2, y2];

        if (tile1.isLocked || tile2.isLocked)
        {
            Debug.Log("Cannot swap locked tiles!");
            return;
        }

        // Swap values
        int tempValue = tile1.value;
        tile1.value = tile2.value;
        tile2.value = tempValue;

        moveCount++;
        OnBoardUpdated?.Invoke();
        CheckPuzzleState();
    }

    /// <summary>
    /// Check if the current puzzle state matches the win condition
    /// </summary>
    private void CheckPuzzleState()
    {
        if (isPuzzleSolved) return;

        bool solved = false;

        switch (currentLevel.puzzleRule)
        {
            case "SumToTen":
                solved = CheckSumToTen();
                break;
            case "ConnectPatterns":
                solved = CheckConnectPatterns();
                break;
            case "SequenceOrder":
                solved = CheckSequenceOrder();
                break;
        }

        if (solved)
        {
            SolvePuzzle();
        }
    }

    private bool CheckSumToTen()
    {
        int required = currentLevel.requiredTiles;
        int minTiles = required > 0 ? required : 2;

        if (selectedTiles.Count < minTiles) return false;

        // If required tiles is set, only check when exact count is reached
        if (required > 0 && selectedTiles.Count != required) return false;

        int sum = 0;
        foreach (var tile in selectedTiles)
        {
            sum += tile.value;
        }

        if (sum == currentLevel.targetSum)
        {
            int tilesCleared = selectedTiles.Count;

            // Lock/clear the matched tiles
            foreach (var tile in selectedTiles)
            {
                tile.isLocked = true;
                tile.isSelected = false;
                tile.value = 0;
            }
            selectedTiles.Clear();
            moveCount++;

            // Scoring: base + combo bonus for extra tiles
            score += 100;
            if (tilesCleared > minTiles)
                score += (tilesCleared - minTiles) * 50; // combo bonus

            // Play success sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayTilePickup();

            OnBoardUpdated?.Invoke();

            // Count remaining tiles
            int remainingCount = CountRemainingTiles();

            if (remainingCount == 0)
            {
                // Perfect clear bonus
                score += 200;
                return true;
            }

            // Check if any valid moves remain
            if (!HasValidMoves())
            {
                // Penalty for remaining tiles
                score -= remainingCount * 50;
                if (score < 0) score = 0;
                OnNoValidMoves?.Invoke();
            }

            return false;
        }
        else if (required > 0 && selectedTiles.Count == required && sum != currentLevel.targetSum)
        {
            // Wrong sum with exact required tiles — deselect all
            foreach (var tile in selectedTiles)
                tile.isSelected = false;
            selectedTiles.Clear();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayInvalidMove();

            OnBoardUpdated?.Invoke();
        }
        else if (required == 0 && sum > currentLevel.targetSum)
        {
            // Over target — deselect all
            foreach (var tile in selectedTiles)
                tile.isSelected = false;
            selectedTiles.Clear();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayInvalidMove();

            OnBoardUpdated?.Invoke();
        }

        return false;
    }

    public int CountRemainingTiles()
    {
        int count = 0;
        for (int y = 0; y < currentLevel.gridHeight; y++)
            for (int x = 0; x < currentLevel.gridWidth; x++)
                if (!board[x, y].isLocked && board[x, y].value > 0)
                    count++;
        return count;
    }

    private bool CheckConnectPatterns()
    {
        // Check if selected tiles form a connected pattern
        if (selectedTiles.Count < 2) return false;

        // Simple check: are all selected tiles adjacent?
        foreach (var tile in selectedTiles)
        {
            bool hasAdjacent = false;
            foreach (var other in selectedTiles)
            {
                if (tile == other) continue;
                if (AreAdjacent(tile, other))
                {
                    hasAdjacent = true;
                    break;
                }
            }
            if (!hasAdjacent && selectedTiles.Count > 1) return false;
        }
        return true;
    }

    private bool CheckSequenceOrder()
    {
        // Check if selected tiles are in ascending order
        if (selectedTiles.Count < 2) return false;

        for (int i = 0; i < selectedTiles.Count - 1; i++)
        {
            if (selectedTiles[i].value >= selectedTiles[i + 1].value)
                return false;
        }
        return true;
    }

    private bool AreAdjacent(Tile t1, Tile t2)
    {
        int dx = Mathf.Abs(t1.x - t2.x);
        int dy = Mathf.Abs(t1.y - t2.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    /// <summary>
    /// Called when puzzle is solved
    /// </summary>
    private void SolvePuzzle()
    {
        isPuzzleSolved = true;
        float sessionTime = Time.time - sessionStartTime;

        // Time bonus
        int timeBonus = Mathf.Max(0, (int)(currentLevel.timeLimit - sessionTime) * 10);
        score += timeBonus;

        int stars = CalculateStars();

        // Log analytics
        Analytics.Instance.LogLevelComplete(currentLevelId, score, stars, moveCount, sessionTime);

        // Notify UI
        if (UIManager.Instance != null)
            UIManager.Instance.ShowVictory(score, stars);

        OnPuzzleSolved?.Invoke();
        Debug.Log($"Puzzle Solved! Score: {score}, Stars: {stars}, Time: {sessionTime:F1}s");
    }

    private int CalculateStars()
    {
        if (currentLevel.starThresholds != null && currentLevel.starThresholds.Length >= 3)
        {
            if (score >= currentLevel.starThresholds[2]) return 3;
            if (score >= currentLevel.starThresholds[1]) return 2;
            if (score >= currentLevel.starThresholds[0]) return 1;
            return 0;
        }
        // Fallback
        if (score >= 800) return 3;
        if (score >= 500) return 2;
        return 1;
    }

    /// <summary>
    /// Provide a hint to the player
    /// </summary>
    public void UseHint()
    {
        hintCount++;
        OnHintUsed?.Invoke(hintCount);
        Debug.Log($"Hint used. Total hints: {hintCount}");
        // TODO: Implement hint logic (show next move, etc.)
    }

    /// <summary>
    /// Get current board state
    /// </summary>
    public Tile[,] GetBoard() => board;

    /// <summary>
    /// Get current level data
    /// </summary>
    public PuzzleLevel GetCurrentLevel() => currentLevel;

    public bool IsPuzzleSolved() => isPuzzleSolved;
    public int GetMoveCount() => moveCount;
    public int GetScore() => score;

    /// <summary>
    /// Check if any pair of remaining tiles sums to 10
    /// </summary>
    public bool HasValidMoves()
    {
        List<Tile> remaining = new List<Tile>();
        for (int y = 0; y < currentLevel.gridHeight; y++)
            for (int x = 0; x < currentLevel.gridWidth; x++)
                if (!board[x, y].isLocked && board[x, y].value > 0)
                    remaining.Add(board[x, y]);

        int target = currentLevel.targetSum;
        int required = currentLevel.requiredTiles;

        if (required > 0)
        {
            // Must use exactly 'required' tiles summing to target
            return HasSubsetSum(remaining, 0, required, target);
        }
        else
        {
            // Any 2+ tiles summing to target
            // Check pairs
            for (int i = 0; i < remaining.Count; i++)
                for (int j = i + 1; j < remaining.Count; j++)
                    if (remaining[i].value + remaining[j].value == target)
                        return true;
            // Check triplets
            for (int i = 0; i < remaining.Count; i++)
                for (int j = i + 1; j < remaining.Count; j++)
                    for (int k = j + 1; k < remaining.Count; k++)
                        if (remaining[i].value + remaining[j].value + remaining[k].value == target)
                            return true;
            // Check quads
            for (int i = 0; i < remaining.Count; i++)
                for (int j = i + 1; j < remaining.Count; j++)
                    for (int k = j + 1; k < remaining.Count; k++)
                        for (int l = k + 1; l < remaining.Count; l++)
                            if (remaining[i].value + remaining[j].value + remaining[k].value + remaining[l].value == target)
                                return true;
        }

        return false;
    }

    private bool HasSubsetSum(List<Tile> tiles, int idx, int count, int target)
    {
        if (count == 0) return target == 0;
        if (idx >= tiles.Count) return false;
        if (tiles.Count - idx < count) return false; // not enough tiles left

        // Include current tile or skip
        return HasSubsetSum(tiles, idx + 1, count - 1, target - tiles[idx].value)
            || HasSubsetSum(tiles, idx + 1, count, target);
    }

    public int GetHintCount() => hintCount;
    public List<Tile> GetSelectedTiles() => selectedTiles;
}
