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
        public string[] targetPattern; // For pattern-based puzzles
        public int difficulty; // 1=Easy, 2=Medium, 3=Hard
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
    private float sessionStartTime;

    public event Action OnPuzzleSolved;
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
        // Check if all selected tiles sum to target
        if (selectedTiles.Count == 0) return false;

        int sum = 0;
        foreach (var tile in selectedTiles)
        {
            sum += tile.value;
        }
        return sum == currentLevel.targetSum;
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

        // Calculate score
        int score = CalculateScore(sessionTime);

        // Get stars (1-3 based on moves/time)
        int stars = CalculateStars();

        // Log analytics
        Analytics.Instance.LogLevelComplete(currentLevelId, score, stars, moveCount, sessionTime);

        OnPuzzleSolved?.Invoke();
        Debug.Log($"Puzzle Solved! Score: {score}, Stars: {stars}");
    }

    private int CalculateScore(float sessionTime)
    {
        int baseScore = 1000;
        int moveBonus = Mathf.Max(0, (50 - moveCount) * 10);
        int timeBonus = Mathf.Max(0, (300 - (int)sessionTime) * 2);
        return baseScore + moveBonus + timeBonus;
    }

    private int CalculateStars()
    {
        if (moveCount <= 20) return 3;
        if (moveCount <= 35) return 2;
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
    public int GetHintCount() => hintCount;
    public List<Tile> GetSelectedTiles() => selectedTiles;
}
