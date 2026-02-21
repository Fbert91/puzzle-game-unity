using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// World Map UI - Visual representation of the 5-world map system
/// Scrollable world selection + level node grid per world
/// </summary>
public class WorldMapUI : MonoBehaviour
{
    [Header("World Selection")]
    [SerializeField] private Transform worldButtonContainer;
    [SerializeField] private GameObject worldButtonPrefab;
    [SerializeField] private ScrollRect worldScrollRect;

    [Header("Level Grid")]
    [SerializeField] private Transform levelNodeContainer;
    [SerializeField] private GameObject levelNodePrefab;
    [SerializeField] private Text worldTitleText;
    [SerializeField] private Text worldProgressText;
    [SerializeField] private Text totalStarsText;
    [SerializeField] private Image worldBackground;

    [Header("Level Node Visuals")]
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.4f, 0.9f, 0.4f);
    [SerializeField] private Color currentColor = new Color(1f, 0.9f, 0.3f);
    [SerializeField] private Color star3Color = new Color(1f, 0.84f, 0f);

    [Header("Lock Overlay")]
    [SerializeField] private GameObject worldLockedOverlay;
    [SerializeField] private Text lockRequirementText;

    private int currentWorldIndex = 0;
    private List<GameObject> worldButtons = new List<GameObject>();
    private List<GameObject> levelNodeObjects = new List<GameObject>();

    private void Start()
    {
        if (WorldMapManager.Instance != null)
        {
            WorldMapManager.Instance.OnMapUpdated += RefreshUI;
        }

        BuildWorldButtons();
        ShowWorld(0);
        UpdateTotalStars();
    }

    private void OnDestroy()
    {
        if (WorldMapManager.Instance != null)
        {
            WorldMapManager.Instance.OnMapUpdated -= RefreshUI;
        }
    }

    /// <summary>
    /// Build the world selection buttons
    /// </summary>
    private void BuildWorldButtons()
    {
        // Clear existing
        foreach (var btn in worldButtons)
        {
            if (btn != null) Destroy(btn);
        }
        worldButtons.Clear();

        if (worldButtonContainer == null || worldButtonPrefab == null) return;

        for (int w = 0; w < 5; w++)
        {
            int worldIdx = w;
            WorldMapManager.WorldData worldData = WorldMapManager.Instance?.GetWorldData(w);
            if (worldData == null) continue;

            GameObject btnObj = Instantiate(worldButtonPrefab, worldButtonContainer);
            worldButtons.Add(btnObj);

            // Setup button
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();
            Image btnImage = btnObj.GetComponent<Image>();

            if (btnText != null)
            {
                string lockIcon = worldData.isUnlocked ? "" : " 🔒";
                btnText.text = $"{worldData.worldName}{lockIcon}";
            }

            if (btnImage != null)
            {
                btnImage.color = worldData.isUnlocked ? worldData.themeColor : lockedColor;
            }

            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                    ShowWorld(worldIdx);
                });
            }
        }
    }

    /// <summary>
    /// Show a specific world's level nodes
    /// </summary>
    public void ShowWorld(int worldIndex)
    {
        currentWorldIndex = worldIndex;
        WorldMapManager.WorldData worldData = WorldMapManager.Instance?.GetWorldData(worldIndex);
        if (worldData == null) return;

        // Update world title and progress
        if (worldTitleText != null)
            worldTitleText.text = worldData.worldName;

        if (worldProgressText != null)
            worldProgressText.text = $"{worldData.levelsCompleted}/{worldData.totalLevels} Levels • {worldData.starsEarned} ⭐";

        // World background color
        if (worldBackground != null)
        {
            Color bgColor = worldData.themeColor;
            bgColor.a = 0.15f;
            worldBackground.color = bgColor;
        }

        // Handle locked worlds
        if (worldLockedOverlay != null)
        {
            worldLockedOverlay.SetActive(!worldData.isUnlocked);
        }
        if (lockRequirementText != null && !worldData.isUnlocked)
        {
            lockRequirementText.text = $"Requires {worldData.starsRequired} ⭐ to unlock\n(You have {WorldMapManager.Instance.GetTotalStars()})";
        }

        // Build level nodes
        BuildLevelNodes(worldIndex);
    }

    /// <summary>
    /// Build level node buttons for a world
    /// </summary>
    private void BuildLevelNodes(int worldIndex)
    {
        // Clear existing nodes
        foreach (var nodeObj in levelNodeObjects)
        {
            if (nodeObj != null) Destroy(nodeObj);
        }
        levelNodeObjects.Clear();

        if (levelNodeContainer == null || levelNodePrefab == null) return;

        List<WorldMapManager.LevelNodeData> nodes = WorldMapManager.Instance?.GetWorldLevelNodes(worldIndex);
        if (nodes == null) return;

        foreach (var node in nodes)
        {
            GameObject nodeObj = Instantiate(levelNodePrefab, levelNodeContainer);
            levelNodeObjects.Add(nodeObj);

            Button btn = nodeObj.GetComponent<Button>();
            Text nodeText = nodeObj.GetComponentInChildren<Text>();
            Image nodeImage = nodeObj.GetComponent<Image>();

            // Level number text
            if (nodeText != null)
            {
                string starDisplay = "";
                if (node.isCompleted)
                {
                    for (int s = 0; s < node.starsEarned; s++) starDisplay += "★";
                    for (int s = node.starsEarned; s < 3; s++) starDisplay += "☆";
                }
                nodeText.text = $"{node.levelInWorld}\n{starDisplay}";
            }

            // Node color based on state
            if (nodeImage != null)
            {
                if (node.isCurrent)
                    nodeImage.color = currentColor;
                else if (node.starsEarned == 3)
                    nodeImage.color = star3Color;
                else if (node.isCompleted)
                    nodeImage.color = completedColor;
                else if (node.isUnlocked)
                    nodeImage.color = unlockedColor;
                else
                    nodeImage.color = lockedColor;
            }

            // Button interaction
            if (btn != null)
            {
                int levelId = node.levelId;
                bool isUnlocked = node.isUnlocked;
                btn.interactable = isUnlocked;

                btn.onClick.AddListener(() =>
                {
                    if (isUnlocked)
                    {
                        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                        WorldMapManager.Instance?.SelectLevel(levelId);
                    }
                });
            }
        }
    }

    /// <summary>
    /// Update total stars display
    /// </summary>
    private void UpdateTotalStars()
    {
        if (totalStarsText != null && WorldMapManager.Instance != null)
        {
            totalStarsText.text = $"Total Stars: {WorldMapManager.Instance.GetTotalStars()} ⭐";
        }
    }

    /// <summary>
    /// Refresh the entire UI
    /// </summary>
    public void RefreshUI()
    {
        BuildWorldButtons();
        ShowWorld(currentWorldIndex);
        UpdateTotalStars();
    }

    /// <summary>
    /// Navigate to next world
    /// </summary>
    public void NextWorld()
    {
        int nextWorld = Mathf.Min(currentWorldIndex + 1, 4);
        ShowWorld(nextWorld);
    }

    /// <summary>
    /// Navigate to previous world
    /// </summary>
    public void PreviousWorld()
    {
        int prevWorld = Mathf.Max(currentWorldIndex - 1, 0);
        ShowWorld(prevWorld);
    }
}
