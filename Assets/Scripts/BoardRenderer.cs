using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles visual rendering of the game board as UI tiles inside a RectTransform.
/// Automatically sizes tiles to fit the available board area.
/// </summary>
public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private float padding = 8f;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Color selectedTileColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color defaultTileColor = new Color(0.22f, 0.22f, 0.30f); // Dark card
    [SerializeField] private Color tileTextColor = Color.white;
    [SerializeField] private int tileFontSize = 32;

    private Dictionary<string, GameObject> tileGameObjects = new Dictionary<string, GameObject>();
    private RectTransform boardContainer;
    private RectTransform myRect;
    private bool needsRender = false;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (myRect == null) myRect = GetComponent<RectTransform>();

        // Subscribe to events
        if (PuzzleGame.Instance != null)
        {
            PuzzleGame.Instance.OnBoardUpdated -= RenderBoard; // prevent double subscribe
            PuzzleGame.Instance.OnBoardUpdated += RenderBoard;
            PuzzleGame.Instance.OnTileSelected -= OnTileSelected;
            PuzzleGame.Instance.OnTileSelected += OnTileSelected;
        }

        // Mark that we need to render (layout may not be ready yet)
        needsRender = true;
    }

    private void Update()
    {
        // Wait for layout to be ready (non-zero size) before rendering
        if (needsRender && myRect != null && myRect.rect.width > 10f)
        {
            needsRender = false;
            // Re-subscribe in case PuzzleGame was created after OnEnable
            if (PuzzleGame.Instance != null)
            {
                PuzzleGame.Instance.OnBoardUpdated -= RenderBoard;
                PuzzleGame.Instance.OnBoardUpdated += RenderBoard;
                PuzzleGame.Instance.OnTileSelected -= OnTileSelected;
                PuzzleGame.Instance.OnTileSelected += OnTileSelected;
            }
            RenderBoard();
        }
    }

    /// <summary>
    /// Call this externally when a new level is loaded to refresh the board.
    /// Also auto-called via OnBoardUpdated event.
    /// </summary>
    public void RenderBoard()
    {
        if (PuzzleGame.Instance == null) return;

        var board = PuzzleGame.Instance.GetBoard();
        if (board == null) return;

        // Create or reuse board container
        if (boardContainer == null)
        {
            var containerObj = new GameObject("BoardContainer");
            containerObj.transform.SetParent(transform, false);
            boardContainer = containerObj.AddComponent<RectTransform>();
            boardContainer.anchorMin = Vector2.zero;
            boardContainer.anchorMax = Vector2.one;
            boardContainer.offsetMin = Vector2.zero;
            boardContainer.offsetMax = Vector2.zero;
        }

        int cols = board.GetLength(0);
        int rows = board.GetLength(1);

        // Calculate tile size based on available space
        float availableWidth = myRect.rect.width - padding * (cols + 1);
        float availableHeight = myRect.rect.height - padding * (rows + 1);
        float tileSize = Mathf.Min(availableWidth / cols, availableHeight / rows);
        tileSize = Mathf.Max(tileSize, 20f); // minimum 20px

        float totalWidth = cols * tileSize + (cols + 1) * padding;
        float totalHeight = rows * tileSize + (rows + 1) * padding;
        float startX = -totalWidth / 2f + padding + tileSize / 2f;
        float startY = totalHeight / 2f - padding - tileSize / 2f;

        // Mark existing tiles for cleanup
        HashSet<string> activeKeys = new HashSet<string>();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                PuzzleGame.Tile tile = board[x, y];
                string key = $"tile_{x}_{y}";
                activeKeys.Add(key);

                GameObject tileObj;
                if (!tileGameObjects.ContainsKey(key))
                {
                    tileObj = CreateTile(x, y, tileSize);
                    tileGameObjects[key] = tileObj;
                }
                else
                {
                    tileObj = tileGameObjects[key];
                    // Resize if needed
                    RectTransform rt = tileObj.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(tileSize, tileSize);
                }

                // Position
                RectTransform rect = tileObj.GetComponent<RectTransform>();
                float posX = startX + x * (tileSize + padding);
                float posY = startY - y * (tileSize + padding);
                rect.anchoredPosition = new Vector2(posX, posY);

                UpdateTileAppearance(tileObj, tile, tileSize);
            }
        }

        // Remove stale tiles
        List<string> toRemove = new List<string>();
        foreach (var kvp in tileGameObjects)
        {
            if (!activeKeys.Contains(kvp.Key))
            {
                Object.Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var k in toRemove) tileGameObjects.Remove(k);
    }

    private GameObject CreateTile(int x, int y, float tileSize)
    {
        GameObject tileObj = new GameObject($"Tile_{x}_{y}");
        tileObj.transform.SetParent(boardContainer, false);

        // RectTransform (added automatically by Image)
        Image image = tileObj.AddComponent<Image>();
        image.color = defaultTileColor;

        RectTransform rect = tileObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(tileSize, tileSize);

        // Rounded look
        // image.sprite = ... // Could assign rounded rect sprite

        // Button for click
        Button button = tileObj.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        button.colors = colors;
        button.targetGraphic = image;

        int tileX = x, tileY = y;
        button.onClick.AddListener(() => OnTileClicked(tileX, tileY));

        // Text showing tile value
        GameObject textObj = new GameObject("Value");
        textObj.transform.SetParent(tileObj.transform, false);
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = "";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = tileFontSize;
        textComponent.fontStyle = FontStyle.Bold;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = tileTextColor;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        textComponent.verticalOverflow = VerticalWrapMode.Overflow;

        // Stretch text to fill tile
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return tileObj;
    }

    private void UpdateTileAppearance(GameObject tileObj, PuzzleGame.Tile tile, float tileSize)
    {
        Image image = tileObj.GetComponent<Image>();

        if (tile.isLocked)
        {
            image.color = new Color(0.3f, 0.3f, 0.3f, 0.4f);
            tileObj.GetComponent<Button>().interactable = false;
        }
        else if (tile.isSelected)
        {
            image.color = selectedTileColor;
            tileObj.GetComponent<Button>().interactable = true;
        }
        else
        {
            image.color = defaultTileColor;
            tileObj.GetComponent<Button>().interactable = true;
        }

        // Update value text
        Text textComponent = tileObj.GetComponentInChildren<Text>();
        if (textComponent != null)
        {
            textComponent.text = tile.value.ToString();
            // Scale font based on tile size
            textComponent.fontSize = Mathf.Max(16, Mathf.RoundToInt(tileSize * 0.4f));
        }
    }

    private void OnTileClicked(int x, int y)
    {
        if (PuzzleGame.Instance != null)
        {
            PuzzleGame.Instance.SelectTile(x, y);

            // Play tile click sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayTilePickup();
        }
    }

    private void OnTileSelected(PuzzleGame.Tile tile)
    {
        string key = $"tile_{tile.x}_{tile.y}";
        if (tileGameObjects.ContainsKey(key))
        {
            // Tile feedback handled via UpdateTileAppearance on next RenderBoard call
        }
    }

    /// <summary>
    /// Clear all tile objects (call when changing levels)
    /// </summary>
    public void ClearBoard()
    {
        foreach (var kvp in tileGameObjects)
        {
            if (kvp.Value != null) Object.Destroy(kvp.Value);
        }
        tileGameObjects.Clear();
    }

    /// <summary>
    /// Temporarily highlight a tile (used by HintManager)
    /// </summary>
    public void HighlightTile(int x, int y, float duration = 2f)
    {
        string key = $"tile_{x}_{y}";
        if (tileGameObjects.ContainsKey(key))
        {
            StartCoroutine(HighlightCoroutine(tileGameObjects[key], duration));
        }
    }

    private IEnumerator HighlightCoroutine(GameObject tileObj, float duration)
    {
        if (tileObj == null) yield break;
        Image img = tileObj.GetComponent<Image>();
        if (img == null) yield break;

        Color original = img.color;
        Color highlight = new Color(0.2f, 0.8f, 1f); // Bright cyan

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 3f, 1f);
            img.color = Color.Lerp(original, highlight, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        img.color = original;
    }

    private void OnDestroy()
    {
        if (PuzzleGame.Instance != null)
        {
            PuzzleGame.Instance.OnBoardUpdated -= RenderBoard;
            PuzzleGame.Instance.OnTileSelected -= OnTileSelected;
        }
    }
}
