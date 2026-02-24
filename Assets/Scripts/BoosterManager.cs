using UnityEngine;
using System;

/// <summary>
/// Booster system — 3 types of power-ups players can use during gameplay.
/// Free boosters given early; purchasable later via IAP or rewarded ads.
/// </summary>
public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance { get; private set; }

    public enum BoosterType
    {
        RevealCombo,  // Highlights a valid combination on the board
        AddTime,      // Adds 15 seconds to the timer
        Shuffle       // Reshuffles all remaining tiles
    }

    private const string REVEAL_KEY = "Booster_Reveal";
    private const string TIME_KEY = "Booster_Time";
    private const string SHUFFLE_KEY = "Booster_Shuffle";

    public int RevealCount { get; private set; }
    public int TimeCount { get; private set; }
    public int ShuffleCount { get; private set; }

    public event Action<BoosterType> OnBoosterUsed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Load saved counts (start with 3 of each for new players)
        RevealCount = PlayerPrefs.GetInt(REVEAL_KEY, 3);
        TimeCount = PlayerPrefs.GetInt(TIME_KEY, 3);
        ShuffleCount = PlayerPrefs.GetInt(SHUFFLE_KEY, 3);
    }

    public int GetCount(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.RevealCombo: return RevealCount;
            case BoosterType.AddTime: return TimeCount;
            case BoosterType.Shuffle: return ShuffleCount;
            default: return 0;
        }
    }

    public bool UseBooster(BoosterType type)
    {
        int count = GetCount(type);
        if (count <= 0) return false;

        switch (type)
        {
            case BoosterType.RevealCombo:
                RevealCount--;
                PlayerPrefs.SetInt(REVEAL_KEY, RevealCount);
                DoRevealCombo();
                break;
            case BoosterType.AddTime:
                TimeCount--;
                PlayerPrefs.SetInt(TIME_KEY, TimeCount);
                DoAddTime();
                break;
            case BoosterType.Shuffle:
                ShuffleCount--;
                PlayerPrefs.SetInt(SHUFFLE_KEY, ShuffleCount);
                DoShuffle();
                break;
        }

        PlayerPrefs.Save();
        OnBoosterUsed?.Invoke(type);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        Debug.Log($"[Booster] Used {type}. Remaining: {GetCount(type)}");
        return true;
    }

    public void AddBooster(BoosterType type, int amount)
    {
        switch (type)
        {
            case BoosterType.RevealCombo:
                RevealCount += amount;
                PlayerPrefs.SetInt(REVEAL_KEY, RevealCount);
                break;
            case BoosterType.AddTime:
                TimeCount += amount;
                PlayerPrefs.SetInt(TIME_KEY, TimeCount);
                break;
            case BoosterType.Shuffle:
                ShuffleCount += amount;
                PlayerPrefs.SetInt(SHUFFLE_KEY, ShuffleCount);
                break;
        }
        PlayerPrefs.Save();
    }

    private void DoRevealCombo()
    {
        // Find a valid combo and highlight it via BoardRenderer
        if (PuzzleGame.Instance == null) return;

        var board = PuzzleGame.Instance.GetBoard();
        var level = PuzzleGame.Instance.GetCurrentLevel();
        if (board == null || level == null) return;

        int target = level.targetSum;
        var boardRenderer = FindObjectOfType<BoardRenderer>();

        // Find first valid pair
        for (int y1 = 0; y1 < level.gridHeight; y1++)
            for (int x1 = 0; x1 < level.gridWidth; x1++)
            {
                if (board[x1, y1].isLocked) continue;
                for (int y2 = 0; y2 < level.gridHeight; y2++)
                    for (int x2 = 0; x2 < level.gridWidth; x2++)
                    {
                        if (x1 == x2 && y1 == y2) continue;
                        if (board[x2, y2].isLocked) continue;
                        if (board[x1, y1].value + board[x2, y2].value == target)
                        {
                            if (boardRenderer != null)
                            {
                                boardRenderer.HighlightTile(x1, y1, 3f);
                                boardRenderer.HighlightTile(x2, y2, 3f);
                            }
                            return;
                        }
                    }
            }
    }

    private void DoAddTime()
    {
        // Add 15 seconds — GameplayUI listens for this
        var gpUI = FindObjectOfType<GameplayUI>();
        if (gpUI != null)
            gpUI.AddBonusTime(15f);
    }

    private void DoShuffle()
    {
        // Reshuffle remaining tile values
        if (PuzzleGame.Instance == null) return;

        var board = PuzzleGame.Instance.GetBoard();
        var level = PuzzleGame.Instance.GetCurrentLevel();
        if (board == null || level == null) return;

        // Collect remaining values
        var values = new System.Collections.Generic.List<int>();
        var positions = new System.Collections.Generic.List<Vector2Int>();

        for (int y = 0; y < level.gridHeight; y++)
            for (int x = 0; x < level.gridWidth; x++)
                if (!board[x, y].isLocked && board[x, y].value > 0)
                {
                    values.Add(board[x, y].value);
                    positions.Add(new Vector2Int(x, y));
                }

        // Shuffle values
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = values[i];
            values[i] = values[j];
            values[j] = temp;
        }

        // Reassign
        for (int i = 0; i < positions.Count; i++)
        {
            board[positions[i].x, positions[i].y].value = values[i];
            board[positions[i].x, positions[i].y].isSelected = false;
        }

        PuzzleGame.Instance.ClearSelection();
        // BoardRenderer will update on next OnBoardUpdated
    }
}
