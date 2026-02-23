using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// BrainBlast Scene Builder v4 — Single-scene architecture.
/// Scene 1: SplashScreen (logo + fade -> loads Gameplay)
/// Scene 2: Gameplay (ALL panels managed by UIManager)
/// Menu: BrainBlast > Build All Scenes
/// </summary>
public class SceneBuilder
{
    // ── Color Palette ──
    private static readonly Color DarkBg       = HexColor("#1A1A2E");
    private static readonly Color Purple       = HexColor("#7C3AED");
    private static readonly Color LightPurple  = HexColor("#8B5CF6");
    private static readonly Color YellowGold   = HexColor("#F1C40F");
    private static readonly Color Green        = HexColor("#2ECC71");
    private static readonly Color Blue         = HexColor("#3498DB");
    private static readonly Color Red          = HexColor("#E94560");
    private static readonly Color DarkCard     = HexColor("#2D2D4E");
    private static readonly Color TextWhite    = Color.white;
    private static readonly Color TextSecondary = HexColor("#AAAACC");
    private static readonly Color DimBlack     = new Color(0, 0, 0, 0.7f);

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }

    private const float REF_W = 1080f;
    private const float REF_H = 1920f;

    [MenuItem("BrainBlast/Build All Scenes")]
    public static void BuildAllScenes()
    {
        if (!EditorUtility.DisplayDialog("BrainBlast Scene Builder v4",
            "This will rebuild SplashScreen and Gameplay scenes. Continue?", "Build", "Cancel"))
            return;

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        System.IO.Directory.CreateDirectory("Assets/Prefabs");

        CreatePrefabs();
        BuildSplashScreen();
        BuildGameplay();

        // Update build settings
        var scenes = new List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/SplashScreen.unity", true));
        scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Gameplay.unity", true));
        EditorBuildSettings.scenes = scenes.ToArray();

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "All scenes built successfully!\nBuild settings updated: SplashScreen + Gameplay", "OK");
    }

    // ════════════════════════════════════════════════════════════════
    // PREFABS
    // ════════════════════════════════════════════════════════════════
    private static void CreatePrefabs()
    {
        // Level Button Prefab
        {
            var obj = new GameObject("LevelButton");
            var img = obj.AddComponent<Image>();
            img.color = DarkCard;
            var btn = obj.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
            btn.colors = colors;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var text = textObj.AddComponent<Text>();
            text.text = "Level";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = TextWhite;
            text.alignment = TextAnchor.MiddleCenter;
            StretchFull(textObj.GetComponent<RectTransform>());

            var rt = obj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(160, 160);

            var le = obj.AddComponent<LayoutElement>();
            le.preferredWidth = 160;
            le.preferredHeight = 160;

            PrefabUtility.SaveAsPrefabAsset(obj, "Assets/Prefabs/LevelButton.prefab");
            Object.DestroyImmediate(obj);
        }

        // Shop Item Prefab
        {
            var obj = new GameObject("ShopItem");
            var img = obj.AddComponent<Image>();
            img.color = DarkCard;
            var btn = obj.AddComponent<Button>();

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var text = textObj.AddComponent<Text>();
            text.text = "Item";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.color = TextWhite;
            text.alignment = TextAnchor.MiddleCenter;
            StretchFull(textObj.GetComponent<RectTransform>());

            var rt = obj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 120);

            var le = obj.AddComponent<LayoutElement>();
            le.preferredWidth = 300;
            le.preferredHeight = 120;

            PrefabUtility.SaveAsPrefabAsset(obj, "Assets/Prefabs/ShopItem.prefab");
            Object.DestroyImmediate(obj);
        }
    }

    // ════════════════════════════════════════════════════════════════
    // SPLASH SCREEN
    // ════════════════════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Splash Screen")]
    public static void BuildSplashScreen()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(DarkBg);
        CreateEventSystem();
        var canvas = CreateCanvas("SplashCanvas");

        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        var center = CreateEmpty(canvas.transform, "CenterGroup");
        StretchFull(center.GetComponent<RectTransform>());
        var cg = center.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        var logo = CreateImage(center.transform, "Logo", Purple);
        SetAnchors(logo.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        logo.rectTransform.sizeDelta = new Vector2(300, 300);

        var title = CreateTMP(center.transform, "TitleText", "BrainBlast", 64, TextWhite, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        title.rectTransform.sizeDelta = new Vector2(800, 100);
        title.rectTransform.anchoredPosition = new Vector2(0, -220);

        var splashComp = center.AddComponent<SplashScreen>();
        var so = new SerializedObject(splashComp);
        so.FindProperty("logoImage").objectReferenceValue = logo;
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.ApplyModifiedProperties();

        SaveScene(scene, "Assets/Scenes/SplashScreen.unity");
    }

    // ════════════════════════════════════════════════════════════════
    // GAMEPLAY (THE MAIN SCENE — EVERYTHING)
    // ════════════════════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Gameplay")]
    public static void BuildGameplay()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(DarkBg);
        CreateEventSystem();
        var canvas = CreateCanvas("MainCanvas");

        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        // Load prefabs
        var levelBtnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LevelButton.prefab");
        var shopItemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ShopItem.prefab");

        // ════════════════════════════════════════════════════════
        // PANEL: MAIN MENU
        // ════════════════════════════════════════════════════════
        var mainMenuPanel = CreatePanel(canvas.transform, "MainMenuPanel");

        // Currency bar (top)
        var currencyBar = CreateImage(mainMenuPanel.transform, "CurrencyBar", DarkCard);
        SetAnchors(currencyBar.rectTransform, 0f, 0.94f, 1f, 1f);
        currencyBar.rectTransform.offsetMin = Vector2.zero;
        currencyBar.rectTransform.offsetMax = Vector2.zero;

        var coinsText = CreateLegacyText(currencyBar.transform, "CoinsText", "Coins: 0", 24, YellowGold, TextAnchor.MiddleLeft);
        SetAnchors(coinsText.rectTransform, 0.05f, 0f, 0.45f, 1f);
        coinsText.rectTransform.offsetMin = Vector2.zero;
        coinsText.rectTransform.offsetMax = Vector2.zero;

        var gemsText = CreateLegacyText(currencyBar.transform, "GemsText", "Gems: 0", 24, Blue, TextAnchor.MiddleRight);
        SetAnchors(gemsText.rectTransform, 0.55f, 0f, 0.95f, 1f);
        gemsText.rectTransform.offsetMin = Vector2.zero;
        gemsText.rectTransform.offsetMax = Vector2.zero;

        // Settings gear (top-right)
        var settingsBtn = CreateButtonLegacy(mainMenuPanel.transform, "SettingsButton", "Settings", 22, DarkCard, TextWhite);
        SetAnchors(settingsBtn.GetComponent<RectTransform>(), 0.82f, 0.88f, 0.98f, 0.93f);
        settingsBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        settingsBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Title
        var titleText = CreateTMP(mainMenuPanel.transform, "TitleText", "BrainBlast", 80, TextWhite, TextAlignmentOptions.Center);
        titleText.fontStyle = FontStyles.Bold;
        SetAnchors(titleText.rectTransform, 0.1f, 0.76f, 0.9f, 0.87f);
        titleText.rectTransform.offsetMin = Vector2.zero;
        titleText.rectTransform.offsetMax = Vector2.zero;

        var subtitle = CreateTMP(mainMenuPanel.transform, "SubtitleText", "Train Your Brain!", 32, TextSecondary, TextAlignmentOptions.Center);
        SetAnchors(subtitle.rectTransform, 0.1f, 0.72f, 0.9f, 0.76f);
        subtitle.rectTransform.offsetMin = Vector2.zero;
        subtitle.rectTransform.offsetMax = Vector2.zero;

        // Pitou mascot area
        var pitouImage = CreateImage(mainMenuPanel.transform, "PitouImage", LightPurple);
        SetAnchors(pitouImage.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        pitouImage.rectTransform.sizeDelta = new Vector2(180, 180);
        pitouImage.rectTransform.anchoredPosition = new Vector2(0, 200);

        // Speech bubble for Pitou
        var speechBubbleObj = CreateImage(mainMenuPanel.transform, "SpeechBubble", DarkCard);
        SetAnchors(speechBubbleObj.rectTransform, 0.15f, 0.62f, 0.85f, 0.70f);
        speechBubbleObj.rectTransform.offsetMin = Vector2.zero;
        speechBubbleObj.rectTransform.offsetMax = Vector2.zero;

        var speechBubbleText = CreateLegacyText(speechBubbleObj.transform, "SpeechBubbleText", "Welcome!", 22, TextWhite, TextAnchor.MiddleCenter);
        StretchFull(speechBubbleText.rectTransform);
        speechBubbleText.rectTransform.offsetMin = new Vector2(10, 5);
        speechBubbleText.rectTransform.offsetMax = new Vector2(-10, -5);

        speechBubbleObj.gameObject.SetActive(false);

        // Streak section
        var streakPanel = CreateImage(mainMenuPanel.transform, "StreakPanel", DarkCard);
        SetAnchors(streakPanel.rectTransform, 0.1f, 0.52f, 0.9f, 0.61f);
        streakPanel.rectTransform.offsetMin = Vector2.zero;
        streakPanel.rectTransform.offsetMax = Vector2.zero;

        var streakFlameImage = CreateImage(streakPanel.transform, "StreakFlame", Red);
        SetAnchors(streakFlameImage.rectTransform, 0.02f, 0.1f, 0.15f, 0.9f);
        streakFlameImage.rectTransform.offsetMin = Vector2.zero;
        streakFlameImage.rectTransform.offsetMax = Vector2.zero;

        var streakCountText = CreateLegacyText(streakPanel.transform, "StreakCountText", "0", 36, YellowGold, TextAnchor.MiddleCenter);
        SetAnchors(streakCountText.rectTransform, 0.15f, 0f, 0.4f, 1f);
        streakCountText.rectTransform.offsetMin = Vector2.zero;
        streakCountText.rectTransform.offsetMax = Vector2.zero;

        var streakRewardText = CreateLegacyText(streakPanel.transform, "StreakRewardText", "Play to earn coins!", 20, TextSecondary, TextAnchor.MiddleLeft);
        SetAnchors(streakRewardText.rectTransform, 0.42f, 0f, 0.98f, 1f);
        streakRewardText.rectTransform.offsetMin = Vector2.zero;
        streakRewardText.rectTransform.offsetMax = Vector2.zero;

        // Streak freeze indicator (hidden by default)
        var streakFreezeIndicator = CreateImage(streakPanel.transform, "StreakFreezeIndicator", Blue);
        SetAnchors(streakFreezeIndicator.rectTransform, 0.85f, 0.6f, 0.98f, 0.95f);
        streakFreezeIndicator.rectTransform.offsetMin = Vector2.zero;
        streakFreezeIndicator.rectTransform.offsetMax = Vector2.zero;
        streakFreezeIndicator.gameObject.SetActive(false);

        var streakFreezeCountText = CreateLegacyText(streakFreezeIndicator.transform, "StreakFreezeCountText", "0", 16, TextWhite, TextAnchor.MiddleCenter);
        StretchFull(streakFreezeCountText.rectTransform);

        // PLAY button
        var playBtn = CreateButtonLegacy(mainMenuPanel.transform, "PlayButton", "PLAY", 48, Purple, TextWhite);
        SetAnchors(playBtn.GetComponent<RectTransform>(), 0.2f, 0.42f, 0.8f, 0.51f);
        playBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        playBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Tabs: Worlds / Daily / Events
        var tabsContainer = CreateEmpty(mainMenuPanel.transform, "TabsContainer");
        SetAnchors(tabsContainer.GetComponent<RectTransform>(), 0.05f, 0.34f, 0.95f, 0.41f);
        tabsContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        tabsContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var tabsHlg = tabsContainer.AddComponent<HorizontalLayoutGroup>();
        tabsHlg.spacing = 15;
        tabsHlg.childAlignment = TextAnchor.MiddleCenter;
        tabsHlg.childControlWidth = true;
        tabsHlg.childControlHeight = true;
        tabsHlg.childForceExpandWidth = true;
        tabsHlg.childForceExpandHeight = true;

        var worldsBtn = CreateButtonLegacy(tabsContainer.transform, "WorldsButton", "Worlds", 26, DarkCard, TextWhite);
        var dailyBtn = CreateButtonLegacy(tabsContainer.transform, "DailyButton", "Daily", 26, DarkCard, TextWhite);
        var eventsBtn = CreateButtonLegacy(tabsContainer.transform, "EventsButton", "Events", 26, DarkCard, TextWhite);

        // Bottom nav: Home, Shop, Pitou, Stats
        var bottomNav = CreateImage(mainMenuPanel.transform, "BottomNav", DarkCard);
        SetAnchors(bottomNav.rectTransform, 0f, 0f, 1f, 0.07f);
        bottomNav.rectTransform.offsetMin = Vector2.zero;
        bottomNav.rectTransform.offsetMax = Vector2.zero;
        var navHlg = bottomNav.gameObject.AddComponent<HorizontalLayoutGroup>();
        navHlg.spacing = 10;
        navHlg.childAlignment = TextAnchor.MiddleCenter;
        navHlg.childControlWidth = true;
        navHlg.childControlHeight = true;
        navHlg.childForceExpandWidth = true;
        navHlg.childForceExpandHeight = true;
        navHlg.padding = new RectOffset(20, 20, 5, 5);

        var homeBtn = CreateButtonLegacy(bottomNav.transform, "HomeButton", "Home", 20, Purple, TextWhite);
        var shopNavBtn = CreateButtonLegacy(bottomNav.transform, "ShopNavButton", "Shop", 20, DarkCard, TextWhite);
        var pitouNavBtn = CreateButtonLegacy(bottomNav.transform, "PitouNavButton", "Pitou", 20, DarkCard, TextWhite);
        var statsBtn = CreateButtonLegacy(bottomNav.transform, "StatsButton", "Stats", 20, DarkCard, TextWhite);

        // About button (hidden, for UIManager ref)
        var aboutBtn = CreateButtonLegacy(mainMenuPanel.transform, "AboutButton", "About", 18, DarkCard, TextWhite);
        aboutBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        aboutBtn.gameObject.SetActive(false);

        // Daily puzzle panel (overlay within main menu)
        var dailyPuzzlePanel = CreatePanel(mainMenuPanel.transform, "DailyPuzzlePanel");
        dailyPuzzlePanel.SetActive(false);

        var dpCard = CreateImage(dailyPuzzlePanel.transform, "DPCard", DarkCard);
        SetAnchors(dpCard.rectTransform, 0.05f, 0.25f, 0.95f, 0.75f);
        dpCard.rectTransform.offsetMin = Vector2.zero;
        dpCard.rectTransform.offsetMax = Vector2.zero;

        var dpTitle = CreateTMP(dpCard.transform, "DPTitle", "Daily Puzzle", 40, YellowGold, TextAlignmentOptions.Center);
        SetAnchors(dpTitle.rectTransform, 0.05f, 0.82f, 0.95f, 0.97f);
        dpTitle.rectTransform.offsetMin = Vector2.zero;
        dpTitle.rectTransform.offsetMax = Vector2.zero;

        var countdownText = CreateLegacyText(dpCard.transform, "CountdownText", "Today's puzzle available!", 24, TextWhite, TextAnchor.MiddleCenter);
        SetAnchors(countdownText.rectTransform, 0.05f, 0.7f, 0.95f, 0.82f);
        countdownText.rectTransform.offsetMin = Vector2.zero;
        countdownText.rectTransform.offsetMax = Vector2.zero;

        var dpRewardText = CreateLegacyText(dpCard.transform, "RewardText", "Reward: 50 coins", 22, YellowGold, TextAnchor.MiddleCenter);
        SetAnchors(dpRewardText.rectTransform, 0.05f, 0.6f, 0.95f, 0.7f);
        dpRewardText.rectTransform.offsetMin = Vector2.zero;
        dpRewardText.rectTransform.offsetMax = Vector2.zero;

        var calendarContainer = CreateEmpty(dpCard.transform, "CalendarContainer");
        SetAnchors(calendarContainer.GetComponent<RectTransform>(), 0.05f, 0.2f, 0.95f, 0.58f);
        calendarContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        calendarContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var calGrid = calendarContainer.AddComponent<GridLayoutGroup>();
        calGrid.cellSize = new Vector2(80, 60);
        calGrid.spacing = new Vector2(5, 5);

        var dpStreakBonusText = CreateLegacyText(dpCard.transform, "StreakBonusText", "Daily Streak: 0/7", 20, TextSecondary, TextAnchor.MiddleCenter);
        SetAnchors(dpStreakBonusText.rectTransform, 0.05f, 0.1f, 0.95f, 0.2f);
        dpStreakBonusText.rectTransform.offsetMin = Vector2.zero;
        dpStreakBonusText.rectTransform.offsetMax = Vector2.zero;

        var playDailyBtn = CreateButtonLegacy(dpCard.transform, "PlayDailyButton", "Play Daily", 30, Green, TextWhite);
        SetAnchors(playDailyBtn.GetComponent<RectTransform>(), 0.2f, 0.02f, 0.8f, 0.1f);
        playDailyBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        playDailyBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // ════════════════════════════════════════════════════════
        // PANEL: LEVEL SELECT
        // ════════════════════════════════════════════════════════
        var levelSelectPanel = CreatePanel(canvas.transform, "LevelSelectPanel");
        levelSelectPanel.SetActive(false);

        var lsTitle = CreateTMP(levelSelectPanel.transform, "LSTitle", "Select Level", 48, TextWhite, TextAlignmentOptions.Center);
        lsTitle.fontStyle = FontStyles.Bold;
        SetAnchors(lsTitle.rectTransform, 0.1f, 0.88f, 0.9f, 0.95f);
        lsTitle.rectTransform.offsetMin = Vector2.zero;
        lsTitle.rectTransform.offsetMax = Vector2.zero;

        var completionPercentageText = CreateLegacyText(levelSelectPanel.transform, "CompletionText", "Progress: 0%", 24, TextSecondary, TextAnchor.MiddleCenter);
        SetAnchors(completionPercentageText.rectTransform, 0.1f, 0.83f, 0.9f, 0.88f);
        completionPercentageText.rectTransform.offsetMin = Vector2.zero;
        completionPercentageText.rectTransform.offsetMax = Vector2.zero;

        // Level grid with scroll
        var lsScrollArea = CreateEmpty(levelSelectPanel.transform, "LevelScrollArea");
        SetAnchors(lsScrollArea.GetComponent<RectTransform>(), 0.05f, 0.1f, 0.95f, 0.82f);
        lsScrollArea.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        lsScrollArea.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        lsScrollArea.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f); // for mask
        lsScrollArea.AddComponent<Mask>().showMaskGraphic = false;

        var levelButtonContainer = CreateEmpty(lsScrollArea.transform, "LevelButtonContainer");
        StretchFull(levelButtonContainer.GetComponent<RectTransform>());
        var lsGrid = levelButtonContainer.AddComponent<GridLayoutGroup>();
        lsGrid.cellSize = new Vector2(160, 160);
        lsGrid.spacing = new Vector2(20, 20);
        lsGrid.childAlignment = TextAnchor.UpperCenter;
        var lsCSF = levelButtonContainer.AddComponent<ContentSizeFitter>();
        lsCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var lsScroll = lsScrollArea.AddComponent<ScrollRect>();
        lsScroll.content = levelButtonContainer.GetComponent<RectTransform>();
        lsScroll.horizontal = false;
        lsScroll.vertical = true;

        var lsBackBtn = CreateButtonLegacy(levelSelectPanel.transform, "LSBackButton", "Back", 32, Purple, TextWhite);
        SetAnchors(lsBackBtn.GetComponent<RectTransform>(), 0.2f, 0.02f, 0.8f, 0.08f);
        lsBackBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        lsBackBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // ════════════════════════════════════════════════════════
        // PANEL: GAMEPLAY
        // ════════════════════════════════════════════════════════
        var gameplayPanel = CreatePanel(canvas.transform, "GameplayPanel");
        gameplayPanel.SetActive(false);

        // Top HUD
        var gpTopBar = CreateImage(gameplayPanel.transform, "GPTopBar", DarkCard);
        SetAnchors(gpTopBar.rectTransform, 0f, 0.93f, 1f, 1f);
        gpTopBar.rectTransform.offsetMin = Vector2.zero;
        gpTopBar.rectTransform.offsetMax = Vector2.zero;

        var gpBackBtn = CreateButtonLegacy(gpTopBar.transform, "GPBackButton", "<", 32, Red, TextWhite);
        SetAnchors(gpBackBtn.GetComponent<RectTransform>(), 0f, 0f, 0f, 1f);
        gpBackBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 0);
        gpBackBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(45, 0);

        var levelNameText = CreateLegacyText(gpTopBar.transform, "LevelNameText", "Level 1", 28, TextWhite, TextAnchor.MiddleCenter);
        SetAnchors(levelNameText.rectTransform, 0.1f, 0f, 0.35f, 1f);
        levelNameText.rectTransform.offsetMin = Vector2.zero;
        levelNameText.rectTransform.offsetMax = Vector2.zero;

        var moveCounterText = CreateLegacyText(gpTopBar.transform, "MoveCounterText", "Moves: 0", 24, TextWhite, TextAnchor.MiddleCenter);
        SetAnchors(moveCounterText.rectTransform, 0.35f, 0f, 0.55f, 1f);
        moveCounterText.rectTransform.offsetMin = Vector2.zero;
        moveCounterText.rectTransform.offsetMax = Vector2.zero;

        var gpCoinsText = CreateLegacyText(gpTopBar.transform, "GPCoinsText", "0", 22, YellowGold, TextAnchor.MiddleCenter);
        SetAnchors(gpCoinsText.rectTransform, 0.6f, 0f, 0.78f, 1f);
        gpCoinsText.rectTransform.offsetMin = Vector2.zero;
        gpCoinsText.rectTransform.offsetMax = Vector2.zero;

        var gpGemsText = CreateLegacyText(gpTopBar.transform, "GPGemsText", "0", 22, Blue, TextAnchor.MiddleCenter);
        SetAnchors(gpGemsText.rectTransform, 0.8f, 0f, 0.98f, 1f);
        gpGemsText.rectTransform.offsetMin = Vector2.zero;
        gpGemsText.rectTransform.offsetMax = Vector2.zero;

        // Rule text
        var ruleText = CreateTMP(gameplayPanel.transform, "RuleText", "Select tiles that sum to 10!", 28, YellowGold, TextAlignmentOptions.Center);
        ruleText.fontStyle = FontStyles.Bold;
        SetAnchors(ruleText.rectTransform, 0.05f, 0.88f, 0.95f, 0.93f);
        ruleText.rectTransform.offsetMin = Vector2.zero;
        ruleText.rectTransform.offsetMax = Vector2.zero;

        // GameplayUI TMP stats
        var gpTimerText = CreateTMP(gameplayPanel.transform, "GPTimerText", "0:00", 24, TextSecondary, TextAlignmentOptions.Center);
        SetAnchors(gpTimerText.rectTransform, 0.35f, 0.84f, 0.65f, 0.88f);
        gpTimerText.rectTransform.offsetMin = Vector2.zero;
        gpTimerText.rectTransform.offsetMax = Vector2.zero;

        var gpScoreText = CreateTMP(gameplayPanel.transform, "GPScoreText", "Score: 0", 24, Green, TextAlignmentOptions.Center);
        SetAnchors(gpScoreText.rectTransform, 0.65f, 0.84f, 0.95f, 0.88f);
        gpScoreText.rectTransform.offsetMin = Vector2.zero;
        gpScoreText.rectTransform.offsetMax = Vector2.zero;

        var gpLevelText = CreateTMP(gameplayPanel.transform, "GPLevelText", "Level 1", 24, TextWhite, TextAlignmentOptions.Center);
        SetAnchors(gpLevelText.rectTransform, 0.05f, 0.84f, 0.35f, 0.88f);
        gpLevelText.rectTransform.offsetMin = Vector2.zero;
        gpLevelText.rectTransform.offsetMax = Vector2.zero;

        var gpMovesText = CreateTMP(gameplayPanel.transform, "GPMovesText", "Moves: 0", 22, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(gpMovesText.rectTransform, 0.05f, 0.80f, 0.5f, 0.84f);
        gpMovesText.rectTransform.offsetMin = Vector2.zero;
        gpMovesText.rectTransform.offsetMax = Vector2.zero;

        var gpComboText = CreateTMP(gameplayPanel.transform, "GPComboText", "", 22, YellowGold, TextAlignmentOptions.Right);
        SetAnchors(gpComboText.rectTransform, 0.5f, 0.80f, 0.95f, 0.84f);
        gpComboText.rectTransform.offsetMin = Vector2.zero;
        gpComboText.rectTransform.offsetMax = Vector2.zero;

        // Board area (for BoardRenderer)
        var boardArea = CreateEmpty(gameplayPanel.transform, "BoardArea");
        SetAnchors(boardArea.GetComponent<RectTransform>(), 0.05f, 0.18f, 0.95f, 0.80f);
        boardArea.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        boardArea.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Bottom action buttons
        var gpActionBar = CreateEmpty(gameplayPanel.transform, "GPActionBar");
        SetAnchors(gpActionBar.GetComponent<RectTransform>(), 0.05f, 0.08f, 0.95f, 0.16f);
        gpActionBar.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        gpActionBar.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var gpHlg = gpActionBar.AddComponent<HorizontalLayoutGroup>();
        gpHlg.spacing = 30;
        gpHlg.childAlignment = TextAnchor.MiddleCenter;
        gpHlg.childControlWidth = true;
        gpHlg.childControlHeight = true;
        gpHlg.childForceExpandWidth = true;
        gpHlg.childForceExpandHeight = true;

        var hintBtn = CreateButtonLegacy(gpActionBar.transform, "HintButton", "Hint (3)", 26, Blue, TextWhite);
        var gpUndoBtn = CreateButtonLegacy(gpActionBar.transform, "UndoButton", "Undo", 26, DarkCard, TextWhite);
        var pauseBtn = CreateButtonLegacy(gpActionBar.transform, "PauseButton", "Pause", 26, DarkCard, TextWhite);
        var settingsGpBtn = CreateButtonLegacy(gpActionBar.transform, "SettingsGPButton", "Settings", 22, DarkCard, TextWhite);

        // Hint button text ref (legacy Text inside hintBtn)
        var hintBtnText = hintBtn.GetComponentInChildren<Text>();

        // Tutorial overlay (inside gameplay panel)
        var tutorialOverlay = CreateEmpty(gameplayPanel.transform, "TutorialOverlay");
        StretchFull(tutorialOverlay.GetComponent<RectTransform>());

        var dimBg = CreateImage(tutorialOverlay.transform, "DimBackground", DimBlack);
        StretchFull(dimBg.rectTransform);

        var tutSpeechBubble = CreateImage(tutorialOverlay.transform, "TutSpeechBubble", DarkCard);
        SetAnchors(tutSpeechBubble.rectTransform, 0.1f, 0.55f, 0.9f, 0.72f);
        tutSpeechBubble.rectTransform.offsetMin = Vector2.zero;
        tutSpeechBubble.rectTransform.offsetMax = Vector2.zero;

        var tutSpeechText = CreateLegacyText(tutSpeechBubble.transform, "TutSpeechText", "", 26, TextWhite, TextAnchor.MiddleCenter);
        StretchFull(tutSpeechText.rectTransform);
        tutSpeechText.rectTransform.offsetMin = new Vector2(15, 10);
        tutSpeechText.rectTransform.offsetMax = new Vector2(-15, -10);

        var highlightFrame = CreateImage(tutorialOverlay.transform, "HighlightFrame", new Color(1, 1, 0, 0.3f));
        SetAnchors(highlightFrame.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        highlightFrame.rectTransform.sizeDelta = new Vector2(150, 150);
        highlightFrame.gameObject.SetActive(false);

        var skipBtn = CreateButtonLegacy(tutorialOverlay.transform, "SkipButton", "Skip", 22, Red, TextWhite);
        SetAnchors(skipBtn.GetComponent<RectTransform>(), 1f, 1f, 1f, 1f);
        skipBtn.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
        skipBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 55);
        skipBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-15, -15);
        var skipBtnText = skipBtn.GetComponentInChildren<Text>();

        var tapToContinueBtn = CreateButtonLegacy(tutorialOverlay.transform, "TapToContinueButton", "Tap to continue", 22, new Color(0, 0, 0, 0.01f), TextSecondary);
        SetAnchors(tapToContinueBtn.GetComponent<RectTransform>(), 0f, 0f, 1f, 0.12f);
        tapToContinueBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        tapToContinueBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        tutorialOverlay.SetActive(false);

        // ════════════════════════════════════════════════════════
        // PANEL: VICTORY
        // ════════════════════════════════════════════════════════
        var victoryPanel = CreatePanel(canvas.transform, "VictoryPanel");
        victoryPanel.SetActive(false);

        var victDim = CreateImage(victoryPanel.transform, "VictoryDim", new Color(0, 0, 0, 0.6f));
        StretchFull(victDim.rectTransform);

        var victCard = CreateImage(victoryPanel.transform, "VictoryCard", DarkCard);
        SetAnchors(victCard.rectTransform, 0.08f, 0.2f, 0.92f, 0.8f);
        victCard.rectTransform.offsetMin = Vector2.zero;
        victCard.rectTransform.offsetMax = Vector2.zero;

        var victTitle = CreateTMP(victCard.transform, "VictoryTitle", "Level Complete!", 44, YellowGold, TextAlignmentOptions.Center);
        SetAnchors(victTitle.rectTransform, 0.05f, 0.85f, 0.95f, 0.97f);
        victTitle.rectTransform.offsetMin = Vector2.zero;
        victTitle.rectTransform.offsetMax = Vector2.zero;

        // Stars (3 Image components)
        var starsContainer = CreateEmpty(victCard.transform, "StarsContainer");
        SetAnchors(starsContainer.GetComponent<RectTransform>(), 0.15f, 0.7f, 0.85f, 0.85f);
        starsContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        starsContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var starsHlg = starsContainer.AddComponent<HorizontalLayoutGroup>();
        starsHlg.spacing = 20;
        starsHlg.childAlignment = TextAnchor.MiddleCenter;
        starsHlg.childControlWidth = true;
        starsHlg.childControlHeight = true;
        starsHlg.childForceExpandWidth = true;
        starsHlg.childForceExpandHeight = true;

        var star1 = CreateImage(starsContainer.transform, "Star1", YellowGold);
        var star2 = CreateImage(starsContainer.transform, "Star2", YellowGold);
        var star3 = CreateImage(starsContainer.transform, "Star3", YellowGold);

        // UIManager uses starsImages (Image[])
        // LevelCompleteManager uses starImages (Image[])

        var victoryScoreText = CreateLegacyText(victCard.transform, "VictoryScoreText", "Score: 0", 32, TextWhite, TextAnchor.MiddleCenter);
        SetAnchors(victoryScoreText.rectTransform, 0.05f, 0.58f, 0.95f, 0.7f);
        victoryScoreText.rectTransform.offsetMin = Vector2.zero;
        victoryScoreText.rectTransform.offsetMax = Vector2.zero;

        // LevelCompleteManager additional texts
        var victMovesText = CreateLegacyText(victCard.transform, "VictMovesText", "Moves: 0", 24, TextSecondary, TextAnchor.MiddleCenter);
        SetAnchors(victMovesText.rectTransform, 0.05f, 0.5f, 0.5f, 0.58f);
        victMovesText.rectTransform.offsetMin = Vector2.zero;
        victMovesText.rectTransform.offsetMax = Vector2.zero;

        var victTimeText = CreateLegacyText(victCard.transform, "VictTimeText", "", 24, TextSecondary, TextAnchor.MiddleCenter);
        SetAnchors(victTimeText.rectTransform, 0.5f, 0.5f, 0.95f, 0.58f);
        victTimeText.rectTransform.offsetMin = Vector2.zero;
        victTimeText.rectTransform.offsetMax = Vector2.zero;

        var pitouReactionText = CreateLegacyText(victCard.transform, "PitouReactionText", "", 28, YellowGold, TextAnchor.MiddleCenter);
        SetAnchors(pitouReactionText.rectTransform, 0.05f, 0.42f, 0.95f, 0.5f);
        pitouReactionText.rectTransform.offsetMin = Vector2.zero;
        pitouReactionText.rectTransform.offsetMax = Vector2.zero;

        // Victory buttons
        var victBtnContainer = CreateEmpty(victCard.transform, "VictoryButtons");
        SetAnchors(victBtnContainer.GetComponent<RectTransform>(), 0.05f, 0.03f, 0.95f, 0.4f);
        victBtnContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        victBtnContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var victVlg = victBtnContainer.AddComponent<VerticalLayoutGroup>();
        victVlg.spacing = 12;
        victVlg.childAlignment = TextAnchor.MiddleCenter;
        victVlg.childControlWidth = true;
        victVlg.childControlHeight = true;
        victVlg.childForceExpandWidth = true;
        victVlg.childForceExpandHeight = true;

        var continueBtn = CreateButtonLegacy(victBtnContainer.transform, "ContinueButton", "Continue", 30, Green, TextWhite);
        var victReplayBtn = CreateButtonLegacy(victBtnContainer.transform, "VictReplayButton", "Replay", 28, DarkCard, TextWhite);
        var victHomeBtn = CreateButtonLegacy(victBtnContainer.transform, "VictHomeButton", "Home", 28, DarkCard, TextWhite);
        var shareBtn = CreateButtonLegacy(victBtnContainer.transform, "ShareButton", "Share", 28, Blue, TextWhite);

        // GameplayUI level complete panel (reuse victory for GameplayUI ref)
        var gpLevelCompletePanel = victoryPanel;

        // ════════════════════════════════════════════════════════
        // PANEL: SETTINGS
        // ════════════════════════════════════════════════════════
        var settingsPanel = CreatePanel(canvas.transform, "SettingsPanel");
        settingsPanel.SetActive(false);

        var setTitle = CreateTMP(settingsPanel.transform, "SettingsTitle", "Settings", 48, TextWhite, TextAlignmentOptions.Center);
        setTitle.fontStyle = FontStyles.Bold;
        SetAnchors(setTitle.rectTransform, 0.1f, 0.85f, 0.9f, 0.93f);
        setTitle.rectTransform.offsetMin = Vector2.zero;
        setTitle.rectTransform.offsetMax = Vector2.zero;

        var setCard = CreateImage(settingsPanel.transform, "SettingsCard", DarkCard);
        SetAnchors(setCard.rectTransform, 0.08f, 0.4f, 0.92f, 0.82f);
        setCard.rectTransform.offsetMin = Vector2.zero;
        setCard.rectTransform.offsetMax = Vector2.zero;

        var sfxLabel = CreateTMP(setCard.transform, "SFXLabel", "SFX Volume", 28, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(sfxLabel.rectTransform, 0.05f, 0.78f, 0.95f, 0.92f);
        sfxLabel.rectTransform.offsetMin = Vector2.zero;
        sfxLabel.rectTransform.offsetMax = Vector2.zero;

        var sfxSlider = CreateSlider(setCard.transform, "SFXVolumeSlider", 0.7f);
        SetAnchors(sfxSlider.GetComponent<RectTransform>(), 0.05f, 0.64f, 0.95f, 0.78f);
        sfxSlider.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        sfxSlider.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var musicLabel = CreateTMP(setCard.transform, "MusicLabel", "Music Volume", 28, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(musicLabel.rectTransform, 0.05f, 0.48f, 0.95f, 0.62f);
        musicLabel.rectTransform.offsetMin = Vector2.zero;
        musicLabel.rectTransform.offsetMax = Vector2.zero;

        var musicSlider = CreateSlider(setCard.transform, "MusicVolumeSlider", 0.5f);
        SetAnchors(musicSlider.GetComponent<RectTransform>(), 0.05f, 0.34f, 0.95f, 0.48f);
        musicSlider.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        musicSlider.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var muteContainer = CreateEmpty(setCard.transform, "MuteContainer");
        SetAnchors(muteContainer.GetComponent<RectTransform>(), 0.05f, 0.15f, 0.95f, 0.32f);
        muteContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        muteContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var muteLabel = CreateTMP(muteContainer.transform, "MuteLabel", "Mute All", 28, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(muteLabel.rectTransform, 0f, 0f, 0.7f, 1f);
        muteLabel.rectTransform.offsetMin = Vector2.zero;
        muteLabel.rectTransform.offsetMax = Vector2.zero;

        var muteToggle = CreateToggle(muteContainer.transform, "MuteToggle");
        SetAnchors(muteToggle.GetComponent<RectTransform>(), 0.75f, 0.15f, 0.95f, 0.85f);
        muteToggle.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        muteToggle.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var setBackBtn = CreateButtonLegacy(settingsPanel.transform, "SettingsBackButton", "Back", 32, Purple, TextWhite);
        SetAnchors(setBackBtn.GetComponent<RectTransform>(), 0.2f, 0.25f, 0.8f, 0.35f);
        setBackBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        setBackBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // ════════════════════════════════════════════════════════
        // PANEL: SHOP
        // ════════════════════════════════════════════════════════
        var shopPanel = CreatePanel(canvas.transform, "ShopPanel");
        shopPanel.SetActive(false);

        var shopTitle = CreateTMP(shopPanel.transform, "ShopTitle", "Shop", 48, YellowGold, TextAlignmentOptions.Center);
        shopTitle.fontStyle = FontStyles.Bold;
        SetAnchors(shopTitle.rectTransform, 0.1f, 0.88f, 0.9f, 0.95f);
        shopTitle.rectTransform.offsetMin = Vector2.zero;
        shopTitle.rectTransform.offsetMax = Vector2.zero;

        var shopScrollArea = CreateEmpty(shopPanel.transform, "ShopScrollArea");
        SetAnchors(shopScrollArea.GetComponent<RectTransform>(), 0.05f, 0.1f, 0.95f, 0.86f);
        shopScrollArea.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        shopScrollArea.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        shopScrollArea.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
        shopScrollArea.AddComponent<Mask>().showMaskGraphic = false;

        var shopItemContainer = CreateEmpty(shopScrollArea.transform, "ShopItemContainer");
        StretchFull(shopItemContainer.GetComponent<RectTransform>());
        var shopVlg = shopItemContainer.AddComponent<VerticalLayoutGroup>();
        shopVlg.spacing = 15;
        shopVlg.childAlignment = TextAnchor.UpperCenter;
        shopVlg.childControlWidth = true;
        shopVlg.childControlHeight = false;
        shopVlg.childForceExpandWidth = true;
        shopVlg.padding = new RectOffset(10, 10, 10, 10);
        var shopCSF = shopItemContainer.AddComponent<ContentSizeFitter>();
        shopCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var shopScroll = shopScrollArea.AddComponent<ScrollRect>();
        shopScroll.content = shopItemContainer.GetComponent<RectTransform>();
        shopScroll.horizontal = false;
        shopScroll.vertical = true;

        var shopBackBtn = CreateButtonLegacy(shopPanel.transform, "ShopBackButton", "Back", 32, Purple, TextWhite);
        SetAnchors(shopBackBtn.GetComponent<RectTransform>(), 0.2f, 0.02f, 0.8f, 0.08f);
        shopBackBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        shopBackBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // ════════════════════════════════════════════════════════
        // PANEL: PAUSE
        // ════════════════════════════════════════════════════════
        var pausePanel = CreatePanel(canvas.transform, "PausePanel");
        pausePanel.SetActive(false);

        var pauseDim = CreateImage(pausePanel.transform, "PauseDim", new Color(0, 0, 0, 0.7f));
        StretchFull(pauseDim.rectTransform);

        var pauseCard = CreateImage(pausePanel.transform, "PauseCard", DarkCard);
        SetAnchors(pauseCard.rectTransform, 0.15f, 0.3f, 0.85f, 0.7f);
        pauseCard.rectTransform.offsetMin = Vector2.zero;
        pauseCard.rectTransform.offsetMax = Vector2.zero;

        var pauseTitle = CreateTMP(pauseCard.transform, "PauseTitle", "Paused", 44, TextWhite, TextAlignmentOptions.Center);
        SetAnchors(pauseTitle.rectTransform, 0.05f, 0.8f, 0.95f, 0.97f);
        pauseTitle.rectTransform.offsetMin = Vector2.zero;
        pauseTitle.rectTransform.offsetMax = Vector2.zero;

        var pauseBtnContainer = CreateEmpty(pauseCard.transform, "PauseButtons");
        SetAnchors(pauseBtnContainer.GetComponent<RectTransform>(), 0.1f, 0.05f, 0.9f, 0.75f);
        pauseBtnContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        pauseBtnContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var pauseVlg = pauseBtnContainer.AddComponent<VerticalLayoutGroup>();
        pauseVlg.spacing = 20;
        pauseVlg.childAlignment = TextAnchor.MiddleCenter;
        pauseVlg.childControlWidth = true;
        pauseVlg.childControlHeight = true;
        pauseVlg.childForceExpandWidth = true;
        pauseVlg.childForceExpandHeight = true;

        var resumeBtn = CreateButtonLegacy(pauseBtnContainer.transform, "ResumeButton", "Resume", 32, Green, TextWhite);
        var pauseSettingsBtn = CreateButtonLegacy(pauseBtnContainer.transform, "PauseSettingsButton", "Settings", 28, DarkCard, TextWhite);
        var pauseHomeBtn = CreateButtonLegacy(pauseBtnContainer.transform, "PauseHomeButton", "Home", 28, DarkCard, TextWhite);

        // ════════════════════════════════════════════════════════
        // MANAGERS (on canvas GameObject)
        // ════════════════════════════════════════════════════════

        // --- BoardRenderer on boardArea ---
        var boardRenderer = boardArea.AddComponent<BoardRenderer>();

        // --- UIManager ---
        var uiMgr = canvas.gameObject.AddComponent<UIManager>();
        {
            var so = new SerializedObject(uiMgr);
            so.FindProperty("mainCanvas").objectReferenceValue = canvas;
            so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
            so.FindProperty("levelSelectPanel").objectReferenceValue = levelSelectPanel;
            so.FindProperty("gameplayPanel").objectReferenceValue = gameplayPanel;
            so.FindProperty("victoryPanel").objectReferenceValue = victoryPanel;
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("shopPanel").objectReferenceValue = shopPanel;
            so.FindProperty("pausePanel").objectReferenceValue = pausePanel;

            so.FindProperty("playButton").objectReferenceValue = playBtn;
            so.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
            so.FindProperty("aboutButton").objectReferenceValue = aboutBtn;

            so.FindProperty("levelButtonContainer").objectReferenceValue = levelButtonContainer.transform;
            so.FindProperty("levelButtonPrefab").objectReferenceValue = levelBtnPrefab;
            so.FindProperty("completionPercentageText").objectReferenceValue = completionPercentageText;

            so.FindProperty("levelNameText").objectReferenceValue = levelNameText;
            so.FindProperty("moveCounterText").objectReferenceValue = moveCounterText;
            so.FindProperty("hintButtonText").objectReferenceValue = hintBtnText;
            so.FindProperty("hintButton").objectReferenceValue = hintBtn;
            so.FindProperty("pauseButton").objectReferenceValue = pauseBtn;
            so.FindProperty("settingsGameplayButton").objectReferenceValue = settingsGpBtn;
            so.FindProperty("coinsText").objectReferenceValue = coinsText;
            so.FindProperty("gemsText").objectReferenceValue = gemsText;

            so.FindProperty("victoryScoreText").objectReferenceValue = victoryScoreText;

            // starsImages array (Image[])
            var starsProp = so.FindProperty("starsImages");
            starsProp.arraySize = 3;
            starsProp.GetArrayElementAtIndex(0).objectReferenceValue = star1;
            starsProp.GetArrayElementAtIndex(1).objectReferenceValue = star2;
            starsProp.GetArrayElementAtIndex(2).objectReferenceValue = star3;

            so.FindProperty("nextLevelButton").objectReferenceValue = continueBtn;
            so.FindProperty("replayButton").objectReferenceValue = victReplayBtn;
            so.FindProperty("menuButton").objectReferenceValue = victHomeBtn;

            so.FindProperty("shopItemContainer").objectReferenceValue = shopItemContainer.transform;
            so.FindProperty("shopItemPrefab").objectReferenceValue = shopItemPrefab;

            so.ApplyModifiedProperties();
        }

        // --- LevelCompleteManager ---
        var lcMgr = canvas.gameObject.AddComponent<LevelCompleteManager>();
        {
            var so = new SerializedObject(lcMgr);
            so.FindProperty("celebrationPanel").objectReferenceValue = victoryPanel;
            
            var starsProp = so.FindProperty("starImages");
            starsProp.arraySize = 3;
            starsProp.GetArrayElementAtIndex(0).objectReferenceValue = star1;
            starsProp.GetArrayElementAtIndex(1).objectReferenceValue = star2;
            starsProp.GetArrayElementAtIndex(2).objectReferenceValue = star3;

            so.FindProperty("scoreText").objectReferenceValue = victoryScoreText;
            so.FindProperty("movesText").objectReferenceValue = victMovesText;
            so.FindProperty("timeText").objectReferenceValue = victTimeText;
            so.FindProperty("pitouReactionText").objectReferenceValue = pitouReactionText;
            so.FindProperty("continueButton").objectReferenceValue = continueBtn;
            so.FindProperty("replayButton").objectReferenceValue = victReplayBtn;
            so.FindProperty("homeButton").objectReferenceValue = victHomeBtn;
            so.FindProperty("shareButton").objectReferenceValue = shareBtn;
            so.ApplyModifiedProperties();
        }

        // --- GameplayUI ---
        var gpUI = canvas.gameObject.AddComponent<GameplayUI>();
        {
            var so = new SerializedObject(gpUI);
            so.FindProperty("timerText").objectReferenceValue = gpTimerText;
            so.FindProperty("scoreText").objectReferenceValue = gpScoreText;
            so.FindProperty("levelText").objectReferenceValue = gpLevelText;
            so.FindProperty("backButton").objectReferenceValue = gpBackBtn;
            so.FindProperty("movesText").objectReferenceValue = gpMovesText;
            so.FindProperty("comboText").objectReferenceValue = gpComboText;
            so.FindProperty("hintButton").objectReferenceValue = hintBtn;
            so.FindProperty("undoButton").objectReferenceValue = gpUndoBtn;
            so.FindProperty("levelCompletePanel").objectReferenceValue = victoryPanel;
            so.FindProperty("nextLevelButton").objectReferenceValue = continueBtn;
            so.FindProperty("menuButton").objectReferenceValue = victHomeBtn;
            so.ApplyModifiedProperties();
        }

        // --- TutorialManager ---
        var tutMgr = canvas.gameObject.AddComponent<TutorialManager>();
        {
            var so = new SerializedObject(tutMgr);
            so.FindProperty("tutorialOverlay").objectReferenceValue = tutorialOverlay;
            so.FindProperty("dimBackground").objectReferenceValue = dimBg;
            so.FindProperty("speechBubble").objectReferenceValue = tutSpeechBubble.gameObject;
            so.FindProperty("speechBubbleText").objectReferenceValue = tutSpeechText;
            so.FindProperty("highlightFrame").objectReferenceValue = highlightFrame.gameObject;
            so.FindProperty("skipButton").objectReferenceValue = skipBtn;
            so.FindProperty("skipButtonText").objectReferenceValue = skipBtnText;
            so.FindProperty("tapToContinueButton").objectReferenceValue = tapToContinueBtn;
            so.ApplyModifiedProperties();
        }

        // --- PitouManager ---
        var pitouMgr = canvas.gameObject.AddComponent<PitouManager>();
        {
            var so = new SerializedObject(pitouMgr);
            so.FindProperty("speechBubbleObject").objectReferenceValue = speechBubbleObj.gameObject;
            so.FindProperty("speechBubbleText").objectReferenceValue = speechBubbleText;
            so.FindProperty("pitouImage").objectReferenceValue = pitouImage;
            so.ApplyModifiedProperties();
        }

        // --- StreakManager ---
        var streakMgr = canvas.gameObject.AddComponent<StreakManager>();
        {
            var so = new SerializedObject(streakMgr);
            so.FindProperty("streakCountText").objectReferenceValue = streakCountText;
            so.FindProperty("streakRewardText").objectReferenceValue = streakRewardText;
            so.FindProperty("streakFlameImage").objectReferenceValue = streakFlameImage;
            so.FindProperty("streakFreezeIndicator").objectReferenceValue = streakFreezeIndicator.gameObject;
            so.FindProperty("streakFreezeCountText").objectReferenceValue = streakFreezeCountText;
            so.FindProperty("streakPanel").objectReferenceValue = streakPanel.gameObject;
            so.ApplyModifiedProperties();
        }

        // --- DailyPuzzleManager ---
        var dailyMgr = canvas.gameObject.AddComponent<DailyPuzzleManager>();
        {
            var so = new SerializedObject(dailyMgr);
            so.FindProperty("dailyPuzzlePanel").objectReferenceValue = dailyPuzzlePanel;
            so.FindProperty("countdownText").objectReferenceValue = countdownText;
            so.FindProperty("rewardText").objectReferenceValue = dpRewardText;
            so.FindProperty("playDailyButton").objectReferenceValue = playDailyBtn;
            so.FindProperty("calendarContainer").objectReferenceValue = calendarContainer.transform;
            so.FindProperty("streakBonusText").objectReferenceValue = dpStreakBonusText;
            so.ApplyModifiedProperties();
        }

        // --- AudioSettingsUI ---
        var audioUI = canvas.gameObject.AddComponent<AudioSettingsUI>();
        {
            var so = new SerializedObject(audioUI);
            so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
            so.FindProperty("muteToggle").objectReferenceValue = muteToggle;
            so.ApplyModifiedProperties();
        }

        // --- MonetizationManager ---
        canvas.gameObject.AddComponent<MonetizationManager>();

        // ════════════════════════════════════════════════════════
        // WIRE BUTTON onClick EVENTS
        // ════════════════════════════════════════════════════════
        // Note: UIManager.Start() sets up most listeners via code.
        // We wire the navigation buttons that UIManager doesn't handle:

        // LevelSelect back -> ShowMainMenu
        WireButtonToUIManager(lsBackBtn, "ShowMainMenu");

        // Settings back -> ShowMainMenu 
        WireButtonToUIManager(setBackBtn, "BackToMainMenu");

        // Shop back -> ShowMainMenu
        WireButtonToUIManager(shopBackBtn, "BackToMainMenu");

        // Bottom nav
        WireButtonToUIManager(homeBtn, "ShowMainMenu");
        WireButtonToUIManager(shopNavBtn, "ShowShop");

        // Worlds tab -> ShowLevelSelect
        WireButtonToUIManager(worldsBtn, "ShowLevelSelect");

        // Daily button -> show daily panel
        WireButtonToUIManager(dailyBtn, "ShowDaily");

        // Pause buttons
        WireButtonToUIManager(resumeBtn, "ResumePause");
        WireButtonToUIManager(pauseSettingsBtn, "ShowSettings");
        WireButtonToUIManager(pauseHomeBtn, "BackToMainMenu");

        SaveScene(scene, "Assets/Scenes/Gameplay.unity");
    }

    /// <summary>
    /// Wire a button's onClick to call a method on UIManager.Instance via a small helper
    /// Since we can't wire to singleton instance at edit time, UIManager.Start() handles
    /// most button wiring. But we'll add UnityEvent persistent listeners where possible.
    /// For now, we add a ButtonAction helper component.
    /// </summary>
    private static void WireButtonToUIManager(Button btn, string action)
    {
        var helper = btn.gameObject.AddComponent<UIButtonAction>();
        var so = new SerializedObject(helper);
        so.FindProperty("action").stringValue = action;
        so.ApplyModifiedProperties();
    }

    // ════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ════════════════════════════════════════════════════════════════

    private static GameObject CreatePanel(Transform parent, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        StretchFull(rt);
        return obj;
    }

    private static Camera CreateCamera(Color bgColor)
    {
        var camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.orthographic = true;
        cam.orthographicSize = 5;
        return cam;
    }

    private static void CreateEventSystem()
    {
        var esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
        esObj.AddComponent<StandaloneInputModule>();
    }

    private static Canvas CreateCanvas(string name)
    {
        var obj = new GameObject(name);
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        var scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(REF_W, REF_H);
        scaler.matchWidthOrHeight = 0.5f;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        obj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static Image CreateImage(Transform parent, string name, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = color;
        return img;
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text, int fontSize, Color color, TextAlignmentOptions alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    private static Text CreateLegacyText(Transform parent, string name, string text, int fontSize, Color color, TextAnchor alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var t = obj.AddComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = alignment;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        return t;
    }

    /// <summary>
    /// Create a button with legacy Text child (for scripts using UnityEngine.UI.Text)
    /// </summary>
    private static Button CreateButtonLegacy(Transform parent, string name, string label, int fontSize, Color bgColor, Color textColor)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var img = obj.AddComponent<Image>();
        img.color = bgColor;

        var btn = obj.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        btn.colors = colors;

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        var t = textObj.AddComponent<Text>();
        t.text = label;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.color = textColor;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontStyle = FontStyle.Bold;
        StretchFull(textObj.GetComponent<RectTransform>());

        return btn;
    }

    private static Slider CreateSlider(Transform parent, string name, float defaultValue)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(obj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.15f);
        StretchFull(bgObj.GetComponent<RectTransform>());

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(obj.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        StretchFull(fillAreaRect);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = Purple;
        StretchFull(fill.GetComponent<RectTransform>());

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(obj.transform, false);
        StretchFull(handleArea.AddComponent<RectTransform>());

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = TextWhite;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(40, 0);

        var slider = obj.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = defaultValue;

        return slider;
    }

    private static Toggle CreateToggle(Transform parent, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(obj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.2f);
        StretchFull(bgObj.GetComponent<RectTransform>());

        var checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(bgObj.transform, false);
        var checkImg = checkObj.AddComponent<Image>();
        checkImg.color = Purple;
        var checkRect = checkObj.GetComponent<RectTransform>();
        StretchFull(checkRect);
        checkRect.offsetMin = new Vector2(6, 6);
        checkRect.offsetMax = new Vector2(-6, -6);

        var toggle = obj.AddComponent<Toggle>();
        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;
        toggle.isOn = false;

        return toggle;
    }

    private static GameObject CreateEmpty(Transform parent, string name)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void SetAnchors(RectTransform rt, float xMin, float yMin, float xMax, float yMax)
    {
        rt.anchorMin = new Vector2(xMin, yMin);
        rt.anchorMax = new Vector2(xMax, yMax);
    }

    private static void SaveScene(Scene scene, string path)
    {
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"[SceneBuilder] Saved: {path}");
    }
}
