using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// BrainBlast Scene Builder — automatically assembles all game scenes.
/// Menu: BrainBlast > Build All Scenes
/// </summary>
public class SceneBuilder
{
    private static readonly Color DarkBlue = new Color(0.102f, 0.102f, 0.180f, 1f); // #1a1a2e
    private static readonly Color YellowAccent = new Color(0.902f, 0.722f, 0f, 1f); // #e6b800
    private static readonly Color White = Color.white;

    [MenuItem("BrainBlast/Build All Scenes")]
    public static void BuildAllScenes()
    {
        if (!EditorUtility.DisplayDialog("BrainBlast Scene Builder",
            "This will overwrite all scene files in Assets/Scenes/. Continue?", "Build", "Cancel"))
            return;

        BuildSplashScreen();
        BuildMainMenu();
        BuildGameplay();
        BuildSettings();
        FixBuildSettings();

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "All 4 scenes built successfully!\nBuild settings updated.", "OK");
    }

    // ─────────────────────────────────────────────
    // SPLASH SCREEN
    // ─────────────────────────────────────────────
    [MenuItem("BrainBlast/Build Scenes/1 - SplashScreen")]
    public static void BuildSplashScreen()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = DarkBlue;
        cam.orthographic = true;
        camGo.AddComponent<AudioListener>();

        // Canvas
        var canvasGo = CreateCanvas("SplashCanvas");
        var canvasGroup = canvasGo.AddComponent<CanvasGroup>();

        // Logo placeholder image
        var logoGo = CreateImage(canvasGo.transform, "LogoImage", new Vector2(0, 100), new Vector2(300, 300));
        var logoImg = logoGo.GetComponent<Image>();
        logoImg.color = new Color(1, 1, 1, 0.3f);

        // Logo text
        var logoText = CreateTMPText(canvasGo.transform, "LogoText", "Pito Games", 48, White, new Vector2(0, -100));

        // Title
        var titleText = CreateTMPText(canvasGo.transform, "TitleText", "BrainBlast", 64, YellowAccent, new Vector2(0, -200));

        // SplashScreen script
        var splashScript = canvasGo.AddComponent<SplashScreen>();
        // Wire serialized fields via SerializedObject
        var so = new SerializedObject(splashScript);
        so.FindProperty("logoImage").objectReferenceValue = logoImg;
        so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
        so.ApplyModifiedPropertiesWithoutUndo();

        // EventSystem
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SplashScreen.unity");
        Debug.Log("[SceneBuilder] SplashScreen built.");
    }

    // ─────────────────────────────────────────────
    // MAIN MENU
    // ─────────────────────────────────────────────
    [MenuItem("BrainBlast/Build Scenes/2 - MainMenu")]
    public static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = DarkBlue;
        cam.orthographic = true;
        camGo.AddComponent<AudioListener>();

        // Canvas
        var canvasGo = CreateCanvas("MainMenuCanvas");

        // Title
        CreateTMPText(canvasGo.transform, "TitleText", "BrainBlast", 72, YellowAccent, new Vector2(0, 650));

        // Mascot placeholder
        var mascot = CreateImage(canvasGo.transform, "PitouMascot", new Vector2(0, 350), new Vector2(250, 250));
        mascot.GetComponent<Image>().color = new Color(1, 0.9f, 0.7f, 0.5f);

        // Buttons
        float btnY = 50;
        float btnSpacing = -130;
        var playBtn = CreateButton(canvasGo.transform, "PlayButton", "Play", new Vector2(0, btnY));
        var dailyBtn = CreateButton(canvasGo.transform, "DailyPuzzleButton", "Daily Puzzle", new Vector2(0, btnY + btnSpacing));
        var settingsBtn = CreateButton(canvasGo.transform, "SettingsButton", "Settings", new Vector2(0, btnY + btnSpacing * 2));
        var quitBtn = CreateButton(canvasGo.transform, "QuitButton", "Quit", new Vector2(0, btnY + btnSpacing * 3));

        // MainMenuUI script — expects: playButton, settingsButton, levelsButton
        var menuUI = canvasGo.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.FindProperty("levelsButton").objectReferenceValue = dailyBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // SceneLoader (for quit button etc.)
        var sceneLoader = canvasGo.AddComponent<SceneLoader>();

        // Wire quit button to SceneLoader.QuitGame via UnityEvent
        var quitBtnComp = quitBtn.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(quitBtnComp.onClick, sceneLoader.QuitGame);

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("[SceneBuilder] MainMenu built.");
    }

    // ─────────────────────────────────────────────
    // GAMEPLAY
    // ─────────────────────────────────────────────
    [MenuItem("BrainBlast/Build Scenes/3 - Gameplay")]
    public static void BuildGameplay()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = DarkBlue;
        cam.orthographic = true;
        camGo.AddComponent<AudioListener>();

        // Canvas
        var canvasGo = CreateCanvas("GameplayCanvas");

        // ── Top Bar ──
        var topBar = CreatePanel(canvasGo.transform, "TopBar",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(0, -20), new Vector2(0, 80));
        topBar.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);

        var backBtn = CreateButton(topBar.transform, "BackButton", "<", new Vector2(-420, 0), new Vector2(80, 60));
        var levelTxt = CreateTMPText(topBar.transform, "LevelText", "Level 1", 28, White, new Vector2(-250, 0));
        var timerTxt = CreateTMPText(topBar.transform, "TimerText", "0:00", 28, White, new Vector2(100, 0));
        var scoreTxt = CreateTMPText(topBar.transform, "ScoreText", "Score: 0", 28, YellowAccent, new Vector2(350, 0));

        // ── Game Board Area ──
        var boardPanel = CreatePanel(canvasGo.transform, "BoardPanel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 50), new Vector2(900, 900));
        boardPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
        var boardRenderer = boardPanel.AddComponent<BoardRenderer>();

        // ── Bottom Bar ──
        var bottomBar = CreatePanel(canvasGo.transform, "BottomBar",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 20), new Vector2(0, 100));
        bottomBar.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);

        var movesTxt = CreateTMPText(bottomBar.transform, "MovesText", "Moves: 0", 24, White, new Vector2(-300, 0));
        var comboTxt = CreateTMPText(bottomBar.transform, "ComboText", "", 24, YellowAccent, new Vector2(-100, 0));
        var hintBtn = CreateButton(bottomBar.transform, "HintButton", "Hint", new Vector2(200, 0), new Vector2(140, 60));
        var undoBtn = CreateButton(bottomBar.transform, "UndoButton", "Undo", new Vector2(370, 0), new Vector2(140, 60));

        // ── Level Complete Panel (hidden) ──
        var lcPanel = CreatePanel(canvasGo.transform, "LevelCompletePanel",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1),
            Vector2.zero, Vector2.zero);
        lcPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
        SetStretch(lcPanel);

        var lcTitle = CreateTMPText(lcPanel.transform, "CompleteTitle", "Level Complete!", 48, YellowAccent, new Vector2(0, 200));
        var nextBtn = CreateButton(lcPanel.transform, "NextLevelButton", "Next Level", new Vector2(0, -50));
        var menuBtn = CreateButton(lcPanel.transform, "MenuButton", "Main Menu", new Vector2(0, -180));
        lcPanel.SetActive(false);

        // ── GameplayUI script ──
        var gpUI = canvasGo.AddComponent<GameplayUI>();
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

        // ── GameInitializer (bootstraps all managers) ──
        var initGo = new GameObject("GameInitializer");
        initGo.AddComponent<GameInitializer>();

        // ── PuzzleGame ──
        var puzzleGo = new GameObject("PuzzleGame");
        puzzleGo.AddComponent<PuzzleGame>();

        // ── UIManager (many fields optional — just attach, GameInitializer creates missing managers) ──
        var uiMgrGo = new GameObject("UIManager");
        uiMgrGo.AddComponent<UIManager>();

        // ── LevelCompleteManager ──
        var lcMgrGo = new GameObject("LevelCompleteManager");
        var lcMgr = lcMgrGo.AddComponent<LevelCompleteManager>();
        var lcSo = new SerializedObject(lcMgr);
        lcSo.FindProperty("celebrationPanel").objectReferenceValue = lcPanel;
        lcSo.FindProperty("continueButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        lcSo.FindProperty("homeButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        lcSo.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Gameplay.unity");
        Debug.Log("[SceneBuilder] Gameplay built.");
    }

    // ─────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────
    [MenuItem("BrainBlast/Build Scenes/4 - Settings")]
    public static void BuildSettings()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = DarkBlue;
        cam.orthographic = true;
        camGo.AddComponent<AudioListener>();

        // Canvas
        var canvasGo = CreateCanvas("SettingsCanvas");

        // Title
        CreateTMPText(canvasGo.transform, "SettingsTitle", "Settings", 56, YellowAccent, new Vector2(0, 700));

        // Back button
        var backBtn = CreateButton(canvasGo.transform, "BackButton", "< Back", new Vector2(-350, 750), new Vector2(200, 60));
        var sceneLoader = canvasGo.AddComponent<SceneLoader>();
        var backBtnComp = backBtn.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtnComp.onClick, sceneLoader.LoadMainMenu);

        // Music volume
        CreateTMPText(canvasGo.transform, "MusicLabel", "Music Volume", 32, White, new Vector2(0, 400));
        var musicSlider = CreateSlider(canvasGo.transform, "MusicVolumeSlider", new Vector2(0, 330));

        // SFX volume
        CreateTMPText(canvasGo.transform, "SFXLabel", "SFX Volume", 32, White, new Vector2(0, 200));
        var sfxSlider = CreateSlider(canvasGo.transform, "SFXVolumeSlider", new Vector2(0, 130));

        // Mute toggle
        CreateTMPText(canvasGo.transform, "MuteLabel", "Mute All", 32, White, new Vector2(-100, 0));
        var muteGo = new GameObject("MuteToggle");
        muteGo.transform.SetParent(canvasGo.transform, false);
        var muteRT = muteGo.AddComponent<RectTransform>();
        muteRT.anchoredPosition = new Vector2(150, 0);
        muteRT.sizeDelta = new Vector2(40, 40);
        // Background
        var muteBg = new GameObject("Background");
        muteBg.transform.SetParent(muteGo.transform, false);
        var muteBgImg = muteBg.AddComponent<Image>();
        muteBgImg.color = new Color(0.3f, 0.3f, 0.3f);
        var muteBgRT = muteBg.GetComponent<RectTransform>();
        muteBgRT.anchorMin = Vector2.zero; muteBgRT.anchorMax = Vector2.one;
        muteBgRT.sizeDelta = Vector2.zero;
        // Checkmark
        var checkGo = new GameObject("Checkmark");
        checkGo.transform.SetParent(muteBg.transform, false);
        var checkImg = checkGo.AddComponent<Image>();
        checkImg.color = YellowAccent;
        var checkRT = checkGo.GetComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.1f, 0.1f); checkRT.anchorMax = new Vector2(0.9f, 0.9f);
        checkRT.sizeDelta = Vector2.zero;
        var toggle = muteGo.AddComponent<Toggle>();
        toggle.targetGraphic = muteBgImg;
        toggle.graphic = checkImg;
        toggle.isOn = false;

        // Theme label (placeholder)
        CreateTMPText(canvasGo.transform, "ThemeLabel", "Theme", 32, White, new Vector2(0, -150));
        var themeBtn1 = CreateButton(canvasGo.transform, "ThemeDefault", "Default", new Vector2(-150, -230), new Vector2(200, 60));
        var themeBtn2 = CreateButton(canvasGo.transform, "ThemeDark", "Ocean", new Vector2(150, -230), new Vector2(200, 60));

        // AudioSettingsUI
        var audioSettings = canvasGo.AddComponent<AudioSettingsUI>();
        var so = new SerializedObject(audioSettings);
        so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider.GetComponent<Slider>();
        so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider.GetComponent<Slider>();
        so.FindProperty("muteToggle").objectReferenceValue = toggle;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Settings.unity");
        Debug.Log("[SceneBuilder] Settings built.");
    }

    // ─────────────────────────────────────────────
    // FIX BUILD SETTINGS
    // ─────────────────────────────────────────────
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
        {
            var guid = AssetDatabase.AssetPathToGUID(scenePaths[i]);
            scenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
        }
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[SceneBuilder] Build settings updated with correct scene GUIDs.");
    }

    // ═════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════

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
        rt.sizeDelta = new Vector2(800, fontSize + 30);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2? size = null)
    {
        var btnSize = size ?? new Vector2(500, 80);
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = btnSize;

        var img = go.AddComponent<Image>();
        img.color = YellowAccent;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = YellowAccent;
        colors.highlightedColor = new Color(1f, 0.85f, 0.2f);
        colors.pressedColor = new Color(0.8f, 0.65f, 0f);
        btn.colors = colors;

        // Label
        var txtGo = new GameObject("Label");
        txtGo.transform.SetParent(go.transform, false);
        var txtRT = txtGo.AddComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.sizeDelta = Vector2.zero;

        var tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 32;
        tmp.color = DarkBlue;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

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

    private static GameObject CreatePanel(Transform parent, string name,
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

    private static void SetStretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static GameObject CreateSlider(Transform parent, string name, Vector2 pos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(600, 40);

        // Background
        var bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.3f, 0.3f);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.25f);
        bgRT.anchorMax = new Vector2(1, 0.75f);
        bgRT.sizeDelta = Vector2.zero;

        // Fill area
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        var fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.25f);
        fillAreaRT.anchorMax = new Vector2(1, 0.75f);
        fillAreaRT.sizeDelta = Vector2.zero;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = YellowAccent;
        var fillRT = fill.GetComponent<RectTransform>();
        fillRT.sizeDelta = Vector2.zero;

        // Handle
        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        var handleAreaRT = handleArea.AddComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.sizeDelta = new Vector2(-20, 0);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = White;
        var handleRT = handle.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(30, 30);

        // Slider component
        var slider = go.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImg;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.7f;

        return go;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
}
