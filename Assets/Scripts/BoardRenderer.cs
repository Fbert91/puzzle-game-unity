using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles visual rendering of the game board and tiles
/// </summary>
public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float padding = 0.1f;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Color selectedTileColor = Color.yellow;
    [SerializeField] private Color defaultTileColor = Color.white;

    private Dictionary<string, GameObject> tileGameObjects = new Dictionary<string, GameObject>();
    private Transform boardContainer;

    private void Start()
    {
        if (PuzzleGame.Instance != null)
        {
            PuzzleGame.Instance.OnBoardUpdated += RenderBoard;
            PuzzleGame.Instance.OnTileSelected += OnTileSelected;
            RenderBoard();
        }
    }

    /// <summary>
    /// Render the game board
    /// </summary>
    private void RenderBoard()
    {
        if (!PuzzleGame.Instance) return;

        var board = PuzzleGame.Instance.GetBoard();
        if (board == null) return;

        // Create or update board container
        if (boardContainer == null)
        {
            boardContainer = new GameObject("BoardContainer").transform;
            boardContainer.SetParent(transform);
            boardContainer.localPosition = Vector3.zero;
        }

        // Render each tile
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                PuzzleGame.Tile tile = board[x, y];
                string key = $"tile_{x}_{y}";

                GameObject tileObj;
                if (!tileGameObjects.ContainsKey(key))
                {
                    // Create new tile
                    tileObj = CreateTile(tile, x, y);
                    tileGameObjects[key] = tileObj;
                }
                else
                {
                    tileObj = tileGameObjects[key];
                }

                // Update tile appearance
                UpdateTileAppearance(tileObj, tile);
            }
        }
    }

    private GameObject CreateTile(PuzzleGame.Tile tile, int x, int y)
    {
        GameObject tileObj = new GameObject($"Tile_{x}_{y}");
        tileObj.transform.SetParent(boardContainer);

        // Calculate position
        float posX = x * (tileSize + padding);
        float posY = -y * (tileSize + padding);
        tileObj.transform.localPosition = new Vector3(posX, posY, 0);

        // Add image component
        Image image = tileObj.AddComponent<Image>();
        image.color = defaultTileColor;

        // Add button component
        Button button = tileObj.AddComponent<Button>();
        int tileX = x, tileY = y;
        button.onClick.AddListener(() => OnTileClicked(tileX, tileY));

        // Set size
        RectTransform rect = tileObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(tileSize, tileSize);

        // Add text for value
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(tileObj.transform);
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = tile.value.ToString();
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 20;
        textComponent.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = rect.sizeDelta;

        return tileObj;
    }

    private void UpdateTileAppearance(GameObject tileObj, PuzzleGame.Tile tile)
    {
        Image image = tileObj.GetComponent<Image>();
        if (tile.isSelected)
        {
            image.color = selectedTileColor;
        }
        else
        {
            image.color = defaultTileColor;
        }

        // Update text
        Text textComponent = tileObj.GetComponentInChildren<Text>();
        if (textComponent)
        {
            textComponent.text = tile.value.ToString();
        }
    }

    private void OnTileClicked(int x, int y)
    {
        PuzzleGame.Instance.SelectTile(x, y);
    }

    private void OnTileSelected(PuzzleGame.Tile tile)
    {
        // Play selection feedback
        string key = $"tile_{tile.x}_{tile.y}";
        if (tileGameObjects.ContainsKey(key))
        {
            // Could add particle effect, sound, etc.
            Debug.Log($"Tile selected: {tile.x},{tile.y} = {tile.value}");
        }
    }
}
