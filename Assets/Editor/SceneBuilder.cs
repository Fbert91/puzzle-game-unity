using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// BrainBlast Scene Builder v2 — builds all scenes matching the v2 UI mockup design.
/// Purple gradient theme, Pitou mascot, bottom nav, daily streak, world progress.
/// Menu: BrainBlast > Build All Scenes
/// </summary>
public class SceneBuilder
{
    // ── Color Palette ──
    private static readonly Color BgDarkNavy    = new Color(0.102f, 0.102f, 0.180f, 1f); // #1a1a2e
    private static readonly Color BgMidNavy     = new Color(0.086f, 0.129f, 0.243f, 1f); // #16213e
    private static readonly Color BgDeepBlue    = new Color(0.059f, 0.204f, 0.376f, 1f); // #0f3460
    private static readonly Color PurpleAccent  = new Color(0.486f, 0.227f, 0.929f, 1f); // #7c3aed
    private static readonly Color PurpleLight   = new Color(0.545f, 0.361f, 0.965f, 1f); // #8b5cf6
    private static readonly Color YellowGold    = new Color(0.945f, 0.769f, 0.059f, 1f); // #f1c40f
    private static readonly Color YellowDark    = new Color(0.902f, 0.722f, 0f, 1f);     // #e6b800
    private static readonly Color Orange        = new Color(0.953f, 0.612f, 0.071f, 1f); // #f39c12
    private static readonly Color GreenBright   = new Color(0.180f, 0.800f, 0.443f, 1f); // #2ecc71
    private static readonly Color GreenDark     = new Color(0.153f, 0.682f, 0.376f, 1f); // #27ae60
    private static readonly Color BlueBright    = new Color(0.204f, 0.596f, 0.859f, 1f); // #3498db
    private static readonly Color RedAccent     = new Color(0.914f, 0.271f, 0.376f, 1f); // #e94560
    private static readonly Color White         = Color.white;
    private static readonly Color WhiteSec      = new Color(1f, 1f, 1f, 0.6f);           // secondary text
    private static readonly Color SemiBlack     = new Color(0f, 0f, 0f, 0.4f);
    private static readonly Color CardBg        = new Color(1f, 1f, 1f, 0.08f);
    private static readonly Color DarkOverlay   = new Color(0f, 0f, 0f, 0.7f);

    // Gameplay background
    private static readonly Color GameBgDark    = new Color(0.059f, 0.047f, 0.161f, 1f); // #0f0c29

    [MenuItem("BrainBlast/Build All Scenes")]
    public static void BuildAllScenes()
    {
        if (!EditorUtility.DisplayDialog("BrainBlast Scene Builder v2",
            "This will overwrite all scene files in Assets/Scenes/. Continue?", "Build", "Cancel"))
            return;

        System.IO.Directory.CreateDirectory("Assets/Scenes");

        BuildSplashScreen();
        BuildMainMenu();
        BuildGameplay();
        BuildSettings();
        FixBuildSettings();

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "All 4 scenes built (v2 mockup design)!\nBuild settings updated.", "OK");
    }

    // ═══════════════════════════════════════════════════
    // SPLASH SCREEN
    // ═══════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Scenes/1 - SplashScreen")]
    public static void BuildSplashScreen()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(BgDarkNavy);
        var canvas = CreateCanvas("SplashCanvas");
        var cg = canvas.AddComponent<CanvasGroup>();

        // Background panel (full screen dark gradient approximation)
        var bg = CreateStretchPanel(canvas.transform, "Background", BgDarkNavy);

        // Logo placeholder
        var logo = CreateImage(canvas.transform, "LogoImage", new Vector2(0, 100), new Vector2(300, 300));
        logo.GetComponent<Image>().color = new Color(0.49f, 0.23f, 0.93f, 0.5f);

        // Studio name
        CreateTMPText(canvas.transform, "StudioText", "Pito Games", 40, WhiteSec, new Vector2(0, -80));

        // Title
        var title = CreateTMPText(canvas.transform, "TitleText", "BrainBlast", 72, White, new Vector2(0, -160));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Subtitle
        CreateTMPText(canvas.transform, "SubtitleText", "PUZZLE ADVENTURE", 24, PurpleLight, new Vector2(0, -220));

        // SplashScreen script
        var splash = canvas.AddComponent<SplashScreen>();
        var so = new SerializedObject(splash);
        so.FindProperty("logoImage").objectReferenceValue = logo.GetComponent<Image>();
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SplashScreen.unity");
        Debug.Log("[SceneBuilder] SplashScreen built (v2).");
    }

    // ═══════════════════════════════════════════════════
    // MAIN MENU  (v2 mockup)
    // ═══════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Scenes/2 - MainMenu")]
    public static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(BgDarkNavy);
        var canvas = CreateCanvas("MainMenuCanvas");

        // ── Full-screen background ──
        CreateStretchPanel(canvas.transform, "Background", BgDarkNavy);

        // ── Top Bar: Currency + Icons ──
        var topBar = CreateAnchoredPanel(canvas.transform, "TopBar",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            new Vector2(0, 0), new Vector2(0, 100));
        topBar.GetComponent<Image>().color = new Color(0, 0, 0, 0f); // transparent

        // Gems display
        var gemsPanel = CreateImage(topBar.transform, "GemsPanel", new Vector2(-360, -50), new Vector2(200, 50));
        gemsPanel.GetComponent<Image>().color = CardBg;
        var gemsIcon = CreateTMPText(gemsPanel.transform, "GemsIcon", "💎", 22, White, new Vector2(-70, 0));
        var gemsText = CreateTMPText(gemsPanel.transform, "GemsText", "2,450", 22, White, new Vector2(10, 0));
        var addGemsBtn = CreateIconButton(gemsPanel.transform, "AddGemsBtn", "+", new Vector2(85, 0), new Vector2(36, 36), GreenBright);

        // Right icons
        var bellBtn = CreateIconButton(topBar.transform, "NotificationBtn", "🔔", new Vector2(360, -50), new Vector2(50, 50), new Color(1, 1, 1, 0f));
        var settingsBtn = CreateIconButton(topBar.transform, "SettingsButton", "⚙", new Vector2(430, -50), new Vector2(50, 50), new Color(1, 1, 1, 0f));

        // ── Pitou Mascot + Speech Bubble ──
        var mascot = CreateImage(canvas.transform, "PitouMascot", new Vector2(0, 550), new Vector2(220, 220));
        mascot.GetComponent<Image>().color = new Color(0.55f, 0.36f, 0.97f, 0.6f); // purple placeholder

        var speechBubble = CreateImage(canvas.transform, "SpeechBubble", new Vector2(180, 620), new Vector2(320, 70));
        speechBubble.GetComponent<Image>().color = CardBg;
        CreateTMPText(speechBubble.transform, "SpeechText", "Ready to blast some puzzles?", 16, White, Vector2.zero);

        // ── Title ──
        var titleGo = CreateTMPText(canvas.transform, "TitleText", "BrainBlast", 80, White, new Vector2(0, 380));
        titleGo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText(canvas.transform, "SubtitleText", "PUZZLE ADVENTURE", 22, PurpleLight, new Vector2(0, 320));

        // ── Daily Streak Card ──
        var streakCard = CreateImage(canvas.transform, "DailyStreakCard", new Vector2(0, 200), new Vector2(900, 120));
        streakCard.GetComponent<Image>().color = CardBg;

        CreateTMPText(streakCard.transform, "StreakIcon", "🔥", 36, White, new Vector2(-350, 10));
        CreateTMPText(streakCard.transform, "StreakTitle", "Daily Streak", 22, WhiteSec, new Vector2(-200, 20));
        var streakDays = CreateTMPText(streakCard.transform, "StreakDays", "5 Days!", 32, YellowGold, new Vector2(-200, -15));
        streakDays.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var claimBtn = CreateStyledButton(streakCard.transform, "ClaimButton", "CLAIM", new Vector2(320, 0), new Vector2(160, 56), GreenBright, White);

        // Progress dots
        for (int i = 0; i < 7; i++)
        {
            var dot = CreateImage(streakCard.transform, $"Dot{i}", new Vector2(-100 + i * 40, -45), new Vector2(24, 24));
            dot.GetComponent<Image>().color = i < 5 ? YellowGold : new Color(1, 1, 1, 0.2f);
        }

        // ── PLAY Button ──
        var playBtn = CreateStyledButton(canvas.transform, "PlayButton", "▶  PLAY", new Vector2(0, 20), new Vector2(800, 100), PurpleAccent, White);
        var playLabel = playBtn.GetComponentInChildren<TextMeshProUGUI>();
        playLabel.fontSize = 42;
        playLabel.fontStyle = FontStyles.Bold;

        CreateTMPText(canvas.transform, "PlaySubtitle", "Level 47 — Ocean World", 18, WhiteSec, new Vector2(0, -45));

        // ── Tabs: Worlds / Daily / Events ──
        float tabY = -110;
        var worldsTab = CreateStyledButton(canvas.transform, "WorldsTab", "🌍  Worlds", new Vector2(-300, tabY), new Vector2(260, 56), PurpleLight, White);
        var dailyTab = CreateStyledButton(canvas.transform, "DailyTab", "📅  Daily", new Vector2(0, tabY), new Vector2(260, 56), CardBg, WhiteSec);
        var eventsTab = CreateStyledButton(canvas.transform, "EventsTab", "🎉  Events", new Vector2(300, tabY), new Vector2(260, 56), CardBg, WhiteSec);

        // ── World Progress Card ──
        var worldCard = CreateImage(canvas.transform, "WorldProgressCard", new Vector2(0, -220), new Vector2(900, 130));
        worldCard.GetComponent<Image>().color = CardBg;

        CreateTMPText(worldCard.transform, "WorldIcon", "🌊", 36, White, new Vector2(-370, 0));
        CreateTMPText(worldCard.transform, "WorldName", "Ocean World", 26, White, new Vector2(-180, 15));
        CreateTMPText(worldCard.transform, "WorldStars", "⭐ 127 / 150", 18, YellowGold, new Vector2(-180, -15));

        // Progress bar background
        var progBg = CreateImage(worldCard.transform, "ProgressBg", new Vector2(150, -10), new Vector2(350, 16));
        progBg.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        // Progress bar fill
        var progFill = CreateImage(progBg.transform, "ProgressFill", Vector2.zero, Vector2.zero);
        var progFillRT = progFill.GetComponent<RectTransform>();
        progFillRT.anchorMin = Vector2.zero;
        progFillRT.anchorMax = new Vector2(0.85f, 1f);
        progFillRT.offsetMin = Vector2.zero;
        progFillRT.offsetMax = Vector2.zero;
        progFill.GetComponent<Image>().color = PurpleAccent;

        // ── Bottom Nav Bar ──
        var navBar = CreateAnchoredPanel(canvas.transform, "BottomNavBar",
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0),
            new Vector2(0, 0), new Vector2(0, 130));
        navBar.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.12f, 0.95f);

        string[] navIcons = { "🏠", "🛒", "🐱", "📊" };
        string[] navLabels = { "Home", "Shop", "Pitou", "Stats" };
        float navSpacing = 220f;
        float navStartX = -navSpacing * 1.5f;
        for (int i = 0; i < 4; i++)
        {
            float x = navStartX + i * navSpacing;
            CreateTMPText(navBar.transform, $"NavIcon{i}", navIcons[i], 32, i == 0 ? PurpleLight : WhiteSec, new Vector2(x, 20));
            CreateTMPText(navBar.transform, $"NavLabel{i}", navLabels[i], 16, i == 0 ? PurpleLight : WhiteSec, new Vector2(x, -15));
        }

        // ── Wire MainMenuUI (expects playButton, settingsButton, levelsButton) ──
        var menuUI = canvas.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.FindProperty("levelsButton").objectReferenceValue = worldsTab.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("[SceneBuilder] MainMenu built (v2).");
    }

    // ═══════════════════════════════════════════════════
    // GAMEPLAY  (v2 mockup)
    // ═══════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Scenes/3 - Gameplay")]
    public static void BuildGameplay()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(GameBgDark);
        var canvas = CreateCanvas("GameplayCanvas");

        // ── Background ──
        CreateStretchPanel(canvas.transform, "Background", GameBgDark);

        // ── Top HUD Bar ──
        var topBar = CreateAnchoredPanel(canvas.transform, "TopBar",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            Vector2.zero, new Vector2(0, 100));
        topBar.GetComponent<Image>().color = SemiBlack;

        var backBtn = CreateStyledButton(topBar.transform, "BackButton", "←", new Vector2(-440, -50), new Vector2(70, 60), new Color(1, 1, 1, 0.15f), White);
        backBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;

        var levelTxt = CreateTMPText(topBar.transform, "LevelText", "Level 3", 30, White, new Vector2(0, -50));
        levelTxt.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var scoreTxt = CreateTMPText(topBar.transform, "ScoreText", "Score: 450", 26, YellowGold, new Vector2(360, -50));

        // ── Timer (below top bar) ──
        var timerTxt = CreateTMPText(canvas.transform, "TimerText", "0:00", 22, WhiteSec, new Vector2(0, 820));

        // ── Board Area (5x5 grid) ──
        var boardPanel = CreateImage(canvas.transform, "BoardPanel", new Vector2(0, 150), new Vector2(940, 940));
        boardPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.03f);
        var boardRenderer = boardPanel.AddComponent<BoardRenderer>();

        // Create 5x5 tile placeholders
        Color[] tileColors = {
            RedAccent, BlueBright, GreenBright, YellowGold, new Color(0.608f, 0.349f, 0.714f) // purple
        };
        float cellSize = 160f;
        float gap = 12f;
        float gridStart = -(cellSize + gap) * 2f;
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                float x = gridStart + col * (cellSize + gap);
                float y = -gridStart - row * (cellSize + gap);
                int colorIdx = (row + col) % tileColors.Length;
                // Some empty slots
                bool isEmpty = (row == 2 && col == 2) || (row == 4 && col == 0);
                var tile = CreateImage(boardPanel.transform, $"Tile_{col}_{row}", new Vector2(x, y), new Vector2(cellSize, cellSize));
                if (isEmpty)
                    tile.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
                else
                    tile.GetComponent<Image>().color = tileColors[colorIdx];
            }
        }

        // ── Bottom HUD ──
        var bottomBar = CreateAnchoredPanel(canvas.transform, "BottomBar",
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0),
            Vector2.zero, new Vector2(0, 200));
        bottomBar.GetComponent<Image>().color = SemiBlack;

        // Stats row
        var movesTxt = CreateTMPText(bottomBar.transform, "MovesText", "Moves: 0", 22, White, new Vector2(-340, 130));
        var timerStatTxt = CreateTMPText(bottomBar.transform, "TimerStatText", "Time: 0:00", 22, White, new Vector2(0, 130));
        var comboTxt = CreateTMPText(bottomBar.transform, "ComboText", "", 22, YellowGold, new Vector2(340, 130));

        // Hint button (yellow/orange gradient approximation)
        var hintBtn = CreateStyledButton(bottomBar.transform, "HintButton", "💡 HINT", new Vector2(-180, 50), new Vector2(300, 70), YellowGold, BgDarkNavy);
        hintBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Undo button (translucent)
        var undoBtn = CreateStyledButton(bottomBar.transform, "UndoButton", "↩ UNDO", new Vector2(180, 50), new Vector2(300, 70), new Color(1, 1, 1, 0.15f), White);

        // ── Level Complete Panel (hidden by default) ──
        var lcPanel = CreateStretchPanel(canvas.transform, "LevelCompletePanel", DarkOverlay);

        var lcTitle = CreateTMPText(lcPanel.transform, "CompleteTitle", "🎉  Level Complete!", 52, YellowGold, new Vector2(0, 300));
        lcTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateTMPText(lcPanel.transform, "CompleteScore", "Score: 1000", 30, White, new Vector2(0, 200));

        // Stars row
        for (int i = 0; i < 3; i++)
        {
            CreateTMPText(lcPanel.transform, $"Star{i}", "⭐", 60, YellowGold, new Vector2(-80 + i * 80, 100));
        }

        var nextBtn = CreateStyledButton(lcPanel.transform, "NextLevelButton", "Next Level  →", new Vector2(0, -50), new Vector2(500, 80), PurpleAccent, White);
        var menuBtn = CreateStyledButton(lcPanel.transform, "MenuButton", "Main Menu", new Vector2(0, -160), new Vector2(500, 80), CardBg, White);

        lcPanel.SetActive(false);

        // ── Wire GameplayUI ──
        var gpUI = canvas.AddComponent<GameplayUI>();
        var so = new SerializedObject(gpUI);
        so.FindProperty("timerText").objectReferenceValue = timerTxt.GetComponent<TextMeshProUGUI>();
        so.FindProperty("scoreText").objectReferenceValue = scoreTxt.GetComponent<TextMeshProUGUI>();
        so.FindProperty("levelText").objectReferenceValue = levelTxt.GetComponent<TextMeshProUGUI>();
        so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        so.FindProperty("movesText").objectReferenceValue = movesTxt.GetComponent<TextMeshProUGUI>();
        so.FindProperty("comboText").objectReferenceValue = comboTxt.GetComponent<TextMeshProUGUI>();
        so.FindProperty("hintButton").objectReferenceValue = hintBtn.GetComponent<Button>();
        so.FindProperty("undoButton").objectReferenceValue = undoBtn.GetComponent<Button>();
        so.FindProperty("levelCompletePanel").objectReferenceValue = lcPanel;
        so.FindProperty("nextLevelButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // ── Managers ──
        var initGo = new GameObject("GameInitializer");
        initGo.AddComponent<GameInitializer>();

        var puzzleGo = new GameObject("PuzzleGame");
        puzzleGo.AddComponent<PuzzleGame>();

        var uiMgrGo = new GameObject("UIManager");
        uiMgrGo.AddComponent<UIManager>();

        var lcMgrGo = new GameObject("LevelCompleteManager");
        var lcMgr = lcMgrGo.AddComponent<LevelCompleteManager>();
        var lcSo = new SerializedObject(lcMgr);
        lcSo.FindProperty("celebrationPanel").objectReferenceValue = lcPanel;
        lcSo.FindProperty("continueButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        lcSo.FindProperty("homeButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        lcSo.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Gameplay.unity");
        Debug.Log("[SceneBuilder] Gameplay built (v2).");
    }

    // ═══════════════════════════════════════════════════
    // SETTINGS  (v2 mockup)
    // ═══════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Scenes/4 - Settings")]
    public static void BuildSettings()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(BgDarkNavy);
        var canvas = CreateCanvas("SettingsCanvas");

        // Background
        CreateStretchPanel(canvas.transform, "Background", BgDarkNavy);

        // ── Top: Back arrow + title ──
        var topBar = CreateAnchoredPanel(canvas.transform, "TopBar",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1),
            Vector2.zero, new Vector2(0, 120));
        topBar.GetComponent<Image>().color = new Color(0, 0, 0, 0f);

        var backBtn = CreateStyledButton(topBar.transform, "BackButton", "←", new Vector2(-430, -60), new Vector2(70, 60), new Color(1, 1, 1, 0.15f), White);
        backBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;

        var titleTxt = CreateTMPText(topBar.transform, "SettingsTitle", "Settings", 42, White, new Vector2(0, -60));
        titleTxt.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // ── Wire Back button to SceneLoader.LoadMainMenu ──
        var sceneLoader = canvas.AddComponent<SceneLoader>();
        var backBtnComp = backBtn.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtnComp.onClick, sceneLoader.LoadMainMenu);

        // Content area
        float startY = 680;
        float rowH = 140;
        int row = 0;

        // ── Music Volume ──
        CreateTMPText(canvas.transform, "MusicLabel", "🎵  Music Volume", 28, White, new Vector2(-150, startY - rowH * row));
        var musicSlider = CreateThemedSlider(canvas.transform, "MusicVolumeSlider", new Vector2(0, startY - rowH * row - 50), BlueBright);
        row++;

        // ── SFX Volume ──
        CreateTMPText(canvas.transform, "SFXLabel", "🔊  SFX Volume", 28, White, new Vector2(-170, startY - rowH * row));
        var sfxSlider = CreateThemedSlider(canvas.transform, "SFXVolumeSlider", new Vector2(0, startY - rowH * row - 50), GreenBright);
        row++;

        // ── Mute All Toggle ──
        var muteToggle = CreateThemedToggle(canvas.transform, "MuteToggle", "🔇  Mute All", new Vector2(0, startY - rowH * row - 20), RedAccent);
        row++;

        // ── Particles Toggle ──
        CreateThemedToggle(canvas.transform, "ParticlesToggle", "✨  Particles", new Vector2(0, startY - rowH * row - 20), GreenBright);
        row++;

        // ── Vibration Toggle ──
        CreateThemedToggle(canvas.transform, "VibrationToggle", "📳  Vibration", new Vector2(0, startY - rowH * row - 20), GreenBright);
        row++;

        // ── Divider ──
        var divider = CreateImage(canvas.transform, "Divider", new Vector2(0, startY - rowH * row - 10), new Vector2(850, 2));
        divider.GetComponent<Image>().color = new Color(1, 1, 1, 0.15f);
        row++;

        // ── About Section ──
        CreateTMPText(canvas.transform, "AboutTitle", "About", 28, White, new Vector2(0, startY - rowH * (row - 1) - 40));
        CreateTMPText(canvas.transform, "VersionText", "Version 1.0.0", 20, WhiteSec, new Vector2(0, startY - rowH * (row - 1) - 80));
        CreateTMPText(canvas.transform, "MadeWithLove", "Made with ❤️ by Pito Games", 18, WhiteSec, new Vector2(0, startY - rowH * (row - 1) - 115));

        // ── Wire AudioSettingsUI ──
        var audioSettings = canvas.AddComponent<AudioSettingsUI>();
        var so = new SerializedObject(audioSettings);
        so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider.GetComponent<Slider>();
        so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider.GetComponent<Slider>();
        so.FindProperty("muteToggle").objectReferenceValue = muteToggle.GetComponent<Toggle>();
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Settings.unity");
        Debug.Log("[SceneBuilder] Settings built (v2).");
    }

    // ═══════════════════════════════════════════════════
    // BUILD SETTINGS
    // ═══════════════════════════════════════════════════
    [MenuItem("BrainBlast/Fix Build Settings")]
    public static void FixBuildSettings()
    {
        string[] scenePaths = {
            "Assets/Scenes/SplashScreen.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Gameplay.unity",
            "Assets/Scenes/Settings.unity"
        };
        var scenes = new EditorBuildSettingsScene[scenePaths.Length];
        for (int i = 0; i < scenePaths.Length; i++)
            scenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[SceneBuilder] Build settings updated.");
    }

    // ═══════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════

    private static void CreateCamera(Color bgColor)
    {
        var go = new GameObject("Main Camera");
        var cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.orthographic = true;
        go.AddComponent<AudioListener>();
    }

    private static GameObject CreateCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    private static GameObject CreateTMPText(Transform parent, string name, string text, int fontSize, Color color, Vector2 pos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(900, fontSize + 30);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.overflowMode = TextOverflowModes.Ellipsis;

        return go;
    }

    private static GameObject CreateImage(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.AddComponent<Image>();
        return go;
    }

    private static GameObject CreateStretchPanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    private static GameObject CreateAnchoredPanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        go.AddComponent<Image>();
        return go;
    }

    private static GameObject CreateStyledButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size, Color bgColor, Color textColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        var img = go.AddComponent<Image>();
        img.color = bgColor;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        btn.colors = colors;
        btn.targetGraphic = img;

        var txtGo = new GameObject("Label");
        txtGo.transform.SetParent(go.transform, false);
        var txtRT = txtGo.AddComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.sizeDelta = Vector2.zero;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        var tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        return go;
    }

    private static GameObject CreateIconButton(Transform parent, string name, string icon,
        Vector2 pos, Vector2 size, Color bgColor)
    {
        var go = CreateStyledButton(parent, name, icon, pos, size, bgColor, White);
        go.GetComponentInChildren<TextMeshProUGUI>().fontSize = 28;
        return go;
    }

    private static GameObject CreateThemedSlider(Transform parent, string name, Vector2 pos, Color fillColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(800, 50);

        // Background track
        var bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.1f);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.3f);
        bgRT.anchorMax = new Vector2(1, 0.7f);
        bgRT.sizeDelta = Vector2.zero;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Fill area
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        var fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.3f);
        fillAreaRT.anchorMax = new Vector2(1, 0.7f);
        fillAreaRT.sizeDelta = new Vector2(-20, 0);
        fillAreaRT.offsetMin = new Vector2(5, 0);
        fillAreaRT.offsetMax = new Vector2(-5, 0);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = fillColor;
        var fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        // Handle slide area
        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        var handleAreaRT = handleArea.AddComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.sizeDelta = new Vector2(-20, 0);
        handleAreaRT.offsetMin = new Vector2(10, 0);
        handleAreaRT.offsetMax = new Vector2(-10, 0);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = White;
        var handleRT = handle.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(40, 40);

        var slider = go.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImg;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.7f;

        return go;
    }

    private static GameObject CreateThemedToggle(Transform parent, string name, string label, Vector2 pos, Color activeColor)
    {
        var container = new GameObject(name + "Container");
        container.transform.SetParent(parent, false);
        var containerRT = container.AddComponent<RectTransform>();
        containerRT.anchoredPosition = pos;
        containerRT.sizeDelta = new Vector2(800, 60);

        // Label
        var labelGo = CreateTMPText(container.transform, name + "Label", label, 26, White, new Vector2(-200, 0));
        labelGo.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);
        labelGo.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        // Toggle track
        var toggleGo = new GameObject(name);
        toggleGo.transform.SetParent(container.transform, false);
        var toggleRT = toggleGo.AddComponent<RectTransform>();
        toggleRT.anchoredPosition = new Vector2(300, 0);
        toggleRT.sizeDelta = new Vector2(80, 40);

        // Track background
        var trackBg = new GameObject("Background");
        trackBg.transform.SetParent(toggleGo.transform, false);
        var trackImg = trackBg.AddComponent<Image>();
        trackImg.color = new Color(1, 1, 1, 0.15f);
        var trackRT = trackBg.GetComponent<RectTransform>();
        trackRT.anchorMin = Vector2.zero;
        trackRT.anchorMax = Vector2.one;
        trackRT.sizeDelta = Vector2.zero;

        // Checkmark (knob)
        var checkGo = new GameObject("Checkmark");
        checkGo.transform.SetParent(trackBg.transform, false);
        var checkImg = checkGo.AddComponent<Image>();
        checkImg.color = activeColor;
        var checkRT = checkGo.GetComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.5f, 0.1f);
        checkRT.anchorMax = new Vector2(0.95f, 0.9f);
        checkRT.sizeDelta = Vector2.zero;
        checkRT.offsetMin = Vector2.zero;
        checkRT.offsetMax = Vector2.zero;

        var toggle = toggleGo.AddComponent<Toggle>();
        toggle.targetGraphic = trackImg;
        toggle.graphic = checkImg;
        toggle.isOn = true;

        return toggleGo;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
}
