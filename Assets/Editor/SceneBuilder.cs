using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// BrainBlast Scene Builder v3 — Complete rewrite.
/// Fixes: proper CanvasScaler, SerializedObject wiring, no Unicode emoji, tutorial overlay, settings back button.
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

    // ── Reference resolution ──
    private const float REF_W = 1080f;
    private const float REF_H = 1920f;

    [MenuItem("BrainBlast/Build All Scenes")]
    public static void BuildAllScenes()
    {
        if (!EditorUtility.DisplayDialog("BrainBlast Scene Builder v3",
            "This will overwrite all scene files in Assets/Scenes/. Continue?", "Build", "Cancel"))
            return;

        System.IO.Directory.CreateDirectory("Assets/Scenes");

        BuildSplashScreen();
        BuildMainMenu();
        BuildGameplay();
        BuildSettings();

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "All scenes built successfully!", "OK");
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

        // Background
        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        // CanvasGroup on a center container
        var center = CreateEmpty(canvas.transform, "CenterGroup");
        StretchFull(center.GetComponent<RectTransform>());
        var cg = center.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // Logo placeholder
        var logo = CreateImage(center.transform, "Logo", Purple);
        SetAnchors(logo.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        logo.rectTransform.sizeDelta = new Vector2(300, 300);

        // Title text
        var title = CreateTMP(center.transform, "TitleText", "BrainBlast", 64, TextWhite, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        title.rectTransform.sizeDelta = new Vector2(800, 100);
        title.rectTransform.anchoredPosition = new Vector2(0, -220);

        // SplashScreen script
        var splashComp = center.AddComponent<SplashScreen>();
        var so = new SerializedObject(splashComp);
        so.FindProperty("logoImage").objectReferenceValue = logo;
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.ApplyModifiedProperties();

        SaveScene(scene, "Assets/Scenes/SplashScreen.unity");
    }

    // ════════════════════════════════════════════════════════════════
    // MAIN MENU
    // ════════════════════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Main Menu")]
    public static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(DarkBg);
        CreateEventSystem();
        var canvas = CreateCanvas("MainMenuCanvas");

        // Background
        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        // ── Layout: vertical center area ──
        // Title area — top 30%
        var titleText = CreateTMP(canvas.transform, "TitleText", "BrainBlast", 80, TextWhite, TextAlignmentOptions.Center);
        titleText.fontStyle = FontStyles.Bold;
        SetAnchors(titleText.rectTransform, 0.1f, 0.78f, 0.9f, 0.88f);
        titleText.rectTransform.offsetMin = Vector2.zero;
        titleText.rectTransform.offsetMax = Vector2.zero;

        var subtitle = CreateTMP(canvas.transform, "SubtitleText", "Train Your Brain!", 36, TextSecondary, TextAlignmentOptions.Center);
        SetAnchors(subtitle.rectTransform, 0.1f, 0.72f, 0.9f, 0.78f);
        subtitle.rectTransform.offsetMin = Vector2.zero;
        subtitle.rectTransform.offsetMax = Vector2.zero;

        // Pitou mascot placeholder (colored circle)
        var pitouCircle = CreateImage(canvas.transform, "PitouMascot", LightPurple);
        SetAnchors(pitouCircle.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        pitouCircle.rectTransform.sizeDelta = new Vector2(200, 200);
        pitouCircle.rectTransform.anchoredPosition = new Vector2(0, 150);

        // Buttons container — center area
        var btnContainer = CreateEmpty(canvas.transform, "ButtonContainer");
        var btnRect = btnContainer.GetComponent<RectTransform>();
        SetAnchors(btnRect, 0.15f, 0.28f, 0.85f, 0.52f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var vlg = btnContainer.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 30;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;

        // Play Button
        var playBtn = CreateButton(btnContainer.transform, "PlayButton", "Play", 42, Purple, TextWhite);
        var playLE = playBtn.gameObject.AddComponent<LayoutElement>();
        playLE.preferredHeight = 120;

        // Levels Button
        var levelsBtn = CreateButton(btnContainer.transform, "LevelsButton", "Levels", 36, DarkCard, TextWhite);
        var levelsLE = levelsBtn.gameObject.AddComponent<LayoutElement>();
        levelsLE.preferredHeight = 100;

        // Settings Button
        var settingsBtn = CreateButton(btnContainer.transform, "SettingsButton", "Settings", 36, DarkCard, TextWhite);
        var settingsLE = settingsBtn.gameObject.AddComponent<LayoutElement>();
        settingsLE.preferredHeight = 100;

        // Footer
        var footer = CreateTMP(canvas.transform, "FooterText", "v1.0 - Made with love", 24, TextSecondary, TextAlignmentOptions.Center);
        SetAnchors(footer.rectTransform, 0.1f, 0.03f, 0.9f, 0.07f);
        footer.rectTransform.offsetMin = Vector2.zero;
        footer.rectTransform.offsetMax = Vector2.zero;

        // MainMenuUI script
        var menuComp = canvas.gameObject.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuComp);
        so.FindProperty("playButton").objectReferenceValue = playBtn;
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
        so.FindProperty("levelsButton").objectReferenceValue = levelsBtn;
        so.ApplyModifiedProperties();

        SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    // ════════════════════════════════════════════════════════════════
    // GAMEPLAY
    // ════════════════════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Gameplay")]
    public static void BuildGameplay()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(DarkBg);
        CreateEventSystem();
        var canvas = CreateCanvas("GameplayCanvas");

        // Background
        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        // ── TOP BAR ── anchors: top 8%
        var topBar = CreateImage(canvas.transform, "TopBar", DarkCard);
        SetAnchors(topBar.rectTransform, 0f, 0.92f, 1f, 1f);
        topBar.rectTransform.offsetMin = Vector2.zero;
        topBar.rectTransform.offsetMax = Vector2.zero;

        // Back button (top-left)
        var backBtn = CreateButton(topBar.transform, "BackButton", "<", 36, Red, TextWhite);
        SetAnchors(backBtn.GetComponent<RectTransform>(), 0f, 0f, 0f, 1f);
        backBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 0);
        backBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 0);

        // Level text (center-left)
        var levelText = CreateTMP(topBar.transform, "LevelText", "Level 1", 32, TextWhite, TextAlignmentOptions.Center);
        SetAnchors(levelText.rectTransform, 0.12f, 0f, 0.35f, 1f);
        levelText.rectTransform.offsetMin = Vector2.zero;
        levelText.rectTransform.offsetMax = Vector2.zero;

        // Timer text (center)
        var timerText = CreateTMP(topBar.transform, "TimerText", "Time: 0:00", 28, YellowGold, TextAlignmentOptions.Center);
        SetAnchors(timerText.rectTransform, 0.35f, 0f, 0.65f, 1f);
        timerText.rectTransform.offsetMin = Vector2.zero;
        timerText.rectTransform.offsetMax = Vector2.zero;

        // Score text (right)
        var scoreText = CreateTMP(topBar.transform, "ScoreText", "Score: 0", 28, Green, TextAlignmentOptions.Center);
        SetAnchors(scoreText.rectTransform, 0.65f, 0f, 1f, 1f);
        scoreText.rectTransform.offsetMin = Vector2.zero;
        scoreText.rectTransform.offsetMax = Vector2.zero;

        // ── RULE TEXT ── below top bar
        var ruleText = CreateTMP(canvas.transform, "RuleText", "Select tiles that sum to 10!", 30, YellowGold, TextAlignmentOptions.Center);
        ruleText.fontStyle = FontStyles.Bold;
        SetAnchors(ruleText.rectTransform, 0.05f, 0.87f, 0.95f, 0.92f);
        ruleText.rectTransform.offsetMin = Vector2.zero;
        ruleText.rectTransform.offsetMax = Vector2.zero;

        // ── STATS BAR ── moves + combo
        var statsBar = CreateEmpty(canvas.transform, "StatsBar");
        SetAnchors(statsBar.GetComponent<RectTransform>(), 0.05f, 0.83f, 0.95f, 0.87f);
        statsBar.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        statsBar.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var movesText = CreateTMP(statsBar.transform, "MovesText", "Moves: 0", 26, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(movesText.rectTransform, 0f, 0f, 0.5f, 1f);
        movesText.rectTransform.offsetMin = Vector2.zero;
        movesText.rectTransform.offsetMax = Vector2.zero;

        var comboText = CreateTMP(statsBar.transform, "ComboText", "", 26, YellowGold, TextAlignmentOptions.Right);
        SetAnchors(comboText.rectTransform, 0.5f, 0f, 1f, 1f);
        comboText.rectTransform.offsetMin = Vector2.zero;
        comboText.rectTransform.offsetMax = Vector2.zero;

        // ── BOARD AREA ── center 60% of screen (left for BoardRenderer)
        var boardArea = CreateEmpty(canvas.transform, "BoardArea");
        SetAnchors(boardArea.GetComponent<RectTransform>(), 0.05f, 0.2f, 0.95f, 0.83f);
        boardArea.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        boardArea.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // ── ACTION BUTTONS ── bottom area
        var actionBar = CreateEmpty(canvas.transform, "ActionBar");
        SetAnchors(actionBar.GetComponent<RectTransform>(), 0.05f, 0.1f, 0.95f, 0.18f);
        actionBar.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        actionBar.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        var hlg = actionBar.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 30;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        var hintBtn = CreateButton(actionBar.transform, "HintButton", "Hint", 28, Blue, TextWhite);
        var undoBtn = CreateButton(actionBar.transform, "UndoButton", "Undo", 28, DarkCard, TextWhite);

        // ── LEVEL COMPLETE PANEL ── overlay
        var lcPanel = CreateImage(canvas.transform, "LevelCompletePanel", new Color(0, 0, 0, 0.8f));
        StretchFull(lcPanel.rectTransform);

        var lcCard = CreateImage(lcPanel.transform, "LCCard", DarkCard);
        SetAnchors(lcCard.rectTransform, 0.1f, 0.3f, 0.9f, 0.7f);
        lcCard.rectTransform.offsetMin = Vector2.zero;
        lcCard.rectTransform.offsetMax = Vector2.zero;

        var lcTitle = CreateTMP(lcCard.transform, "LCTitle", "Level Complete!", 48, YellowGold, TextAlignmentOptions.Center);
        SetAnchors(lcTitle.rectTransform, 0.05f, 0.7f, 0.95f, 0.95f);
        lcTitle.rectTransform.offsetMin = Vector2.zero;
        lcTitle.rectTransform.offsetMax = Vector2.zero;

        var lcBtnContainer = CreateEmpty(lcCard.transform, "LCButtons");
        SetAnchors(lcBtnContainer.GetComponent<RectTransform>(), 0.1f, 0.1f, 0.9f, 0.5f);
        lcBtnContainer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        lcBtnContainer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        var lcVlg = lcBtnContainer.AddComponent<VerticalLayoutGroup>();
        lcVlg.spacing = 20;
        lcVlg.childAlignment = TextAnchor.MiddleCenter;
        lcVlg.childControlWidth = true;
        lcVlg.childControlHeight = true;
        lcVlg.childForceExpandWidth = true;
        lcVlg.childForceExpandHeight = true;

        var nextLevelBtn = CreateButton(lcBtnContainer.transform, "NextLevelButton", "Next Level", 32, Green, TextWhite);
        var menuBtn = CreateButton(lcBtnContainer.transform, "MenuButton", "Main Menu", 32, DarkCard, TextWhite);

        lcPanel.gameObject.SetActive(false);

        // ── TUTORIAL OVERLAY ──
        var tutOverlay = CreateEmpty(canvas.transform, "TutorialOverlay");
        StretchFull(tutOverlay.GetComponent<RectTransform>());

        // Dim background
        var dimBg = CreateImage(tutOverlay.transform, "DimBackground", DimBlack);
        StretchFull(dimBg.rectTransform);

        // Speech bubble
        var speechBubble = CreateImage(tutOverlay.transform, "SpeechBubble", DarkCard);
        SetAnchors(speechBubble.rectTransform, 0.1f, 0.55f, 0.9f, 0.72f);
        speechBubble.rectTransform.offsetMin = Vector2.zero;
        speechBubble.rectTransform.offsetMax = Vector2.zero;

        var speechText = CreateTMP(speechBubble.transform, "SpeechBubbleText", "", 28, TextWhite, TextAlignmentOptions.Center);
        StretchFull(speechText.rectTransform);
        speechText.rectTransform.offsetMin = new Vector2(20, 10);
        speechText.rectTransform.offsetMax = new Vector2(-20, -10);

        // Highlight frame
        var highlightFrame = CreateImage(tutOverlay.transform, "HighlightFrame", new Color(1, 1, 0, 0.3f));
        SetAnchors(highlightFrame.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f);
        highlightFrame.rectTransform.sizeDelta = new Vector2(150, 150);
        highlightFrame.gameObject.SetActive(false);

        // Skip button (top-right)
        var skipBtn = CreateButton(tutOverlay.transform, "SkipButton", "Skip", 24, Red, TextWhite);
        SetAnchors(skipBtn.GetComponent<RectTransform>(), 1f, 1f, 1f, 1f);
        skipBtn.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
        skipBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 60);
        skipBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, -20);

        // Get skip button text
        var skipBtnTextComp = skipBtn.GetComponentInChildren<TextMeshProUGUI>();

        // Tap to continue (bottom center, full-width invisible button)
        var tapBtn = CreateButton(tutOverlay.transform, "TapToContinueButton", "Tap to continue", 24, new Color(0, 0, 0, 0.01f), TextSecondary);
        SetAnchors(tapBtn.GetComponent<RectTransform>(), 0f, 0f, 1f, 0.15f);
        tapBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        tapBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        tutOverlay.SetActive(false);

        // ── Wire GameplayUI ──
        var gameplayUI = canvas.gameObject.AddComponent<GameplayUI>();
        var gso = new SerializedObject(gameplayUI);
        gso.FindProperty("timerText").objectReferenceValue = timerText;
        gso.FindProperty("scoreText").objectReferenceValue = scoreText;
        gso.FindProperty("levelText").objectReferenceValue = levelText;
        gso.FindProperty("backButton").objectReferenceValue = backBtn;
        gso.FindProperty("movesText").objectReferenceValue = movesText;
        gso.FindProperty("comboText").objectReferenceValue = comboText;
        gso.FindProperty("hintButton").objectReferenceValue = hintBtn;
        gso.FindProperty("undoButton").objectReferenceValue = undoBtn;
        gso.FindProperty("levelCompletePanel").objectReferenceValue = lcPanel.gameObject;
        gso.FindProperty("nextLevelButton").objectReferenceValue = nextLevelBtn;
        gso.FindProperty("menuButton").objectReferenceValue = menuBtn;
        gso.ApplyModifiedProperties();

        // ── Wire TutorialManager ──
        var tutMgr = canvas.gameObject.AddComponent<TutorialManager>();
        var tso = new SerializedObject(tutMgr);
        tso.FindProperty("tutorialOverlay").objectReferenceValue = tutOverlay;
        tso.FindProperty("dimBackground").objectReferenceValue = dimBg;
        tso.FindProperty("speechBubble").objectReferenceValue = speechBubble.gameObject;
        tso.FindProperty("speechBubbleText").objectReferenceValue = speechText.GetComponent<TextMeshProUGUI>() != null ? null : speechText.gameObject.GetComponent<UnityEngine.UI.Text>();
        tso.FindProperty("highlightFrame").objectReferenceValue = highlightFrame.gameObject;
        tso.FindProperty("skipButton").objectReferenceValue = skipBtn;
        tso.FindProperty("skipButtonText").objectReferenceValue = skipBtnTextComp != null ? null : null;
        tso.FindProperty("tapToContinueButton").objectReferenceValue = tapBtn;
        tso.ApplyModifiedProperties();

        // Wait — TutorialManager uses UnityEngine.UI.Text for speechBubbleText and skipButtonText
        // Let me re-check: the TutorialManager has Text (not TMP) fields. Need to add legacy Text components.
        // Actually, looking at TutorialManager: speechBubbleText is UnityEngine.UI.Text, skipButtonText is UnityEngine.UI.Text
        // So we need to add legacy Text components alongside or instead of TMP for those specific fields.

        // Fix: Add legacy Text to speech bubble for TutorialManager compatibility
        var legacySpeechText = speechBubble.gameObject.AddComponent<UnityEngine.UI.Text>();
        legacySpeechText.text = "";
        legacySpeechText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        legacySpeechText.fontSize = 28;
        legacySpeechText.color = TextWhite;
        legacySpeechText.alignment = TextAnchor.MiddleCenter;

        // Add legacy Text to skip button for skipButtonText
        // The skip button already has TMP text child; add a legacy text too
        var skipLegacyTextObj = new GameObject("SkipLegacyText");
        skipLegacyTextObj.transform.SetParent(skipBtn.transform, false);
        var skipLegacyText = skipLegacyTextObj.AddComponent<UnityEngine.UI.Text>();
        skipLegacyText.text = "Skip";
        skipLegacyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        skipLegacyText.fontSize = 24;
        skipLegacyText.color = TextWhite;
        skipLegacyText.alignment = TextAnchor.MiddleCenter;
        var skipLegacyRect = skipLegacyTextObj.GetComponent<RectTransform>();
        StretchFull(skipLegacyRect);
        skipLegacyTextObj.SetActive(false); // hidden, just for reference

        // Re-wire TutorialManager with correct types
        tso = new SerializedObject(tutMgr);
        tso.FindProperty("tutorialOverlay").objectReferenceValue = tutOverlay;
        tso.FindProperty("dimBackground").objectReferenceValue = dimBg;
        tso.FindProperty("speechBubble").objectReferenceValue = speechBubble.gameObject;
        tso.FindProperty("speechBubbleText").objectReferenceValue = legacySpeechText;
        tso.FindProperty("highlightFrame").objectReferenceValue = highlightFrame.gameObject;
        tso.FindProperty("skipButton").objectReferenceValue = skipBtn;
        tso.FindProperty("skipButtonText").objectReferenceValue = skipLegacyText;
        tso.FindProperty("tapToContinueButton").objectReferenceValue = tapBtn;
        tso.ApplyModifiedProperties();

        SaveScene(scene, "Assets/Scenes/Gameplay.unity");
    }

    // ════════════════════════════════════════════════════════════════
    // SETTINGS
    // ════════════════════════════════════════════════════════════════
    [MenuItem("BrainBlast/Build Settings")]
    public static void BuildSettings()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(DarkBg);
        CreateEventSystem();
        var canvas = CreateCanvas("SettingsCanvas");

        // Background
        var bg = CreateImage(canvas.transform, "Background", DarkBg);
        StretchFull(bg.rectTransform);

        // Title
        var title = CreateTMP(canvas.transform, "TitleText", "Settings", 56, TextWhite, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        SetAnchors(title.rectTransform, 0.1f, 0.85f, 0.9f, 0.93f);
        title.rectTransform.offsetMin = Vector2.zero;
        title.rectTransform.offsetMax = Vector2.zero;

        // Settings card
        var card = CreateImage(canvas.transform, "SettingsCard", DarkCard);
        SetAnchors(card.rectTransform, 0.08f, 0.4f, 0.92f, 0.82f);
        card.rectTransform.offsetMin = Vector2.zero;
        card.rectTransform.offsetMax = Vector2.zero;

        // SFX Volume
        var sfxLabel = CreateTMP(card.transform, "SFXLabel", "SFX Volume", 28, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(sfxLabel.rectTransform, 0.05f, 0.78f, 0.95f, 0.92f);
        sfxLabel.rectTransform.offsetMin = Vector2.zero;
        sfxLabel.rectTransform.offsetMax = Vector2.zero;

        var sfxSlider = CreateSlider(card.transform, "SFXVolumeSlider", 0.7f);
        SetAnchors(sfxSlider.GetComponent<RectTransform>(), 0.05f, 0.64f, 0.95f, 0.78f);
        sfxSlider.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        sfxSlider.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Music Volume
        var musicLabel = CreateTMP(card.transform, "MusicLabel", "Music Volume", 28, TextWhite, TextAlignmentOptions.Left);
        SetAnchors(musicLabel.rectTransform, 0.05f, 0.48f, 0.95f, 0.62f);
        musicLabel.rectTransform.offsetMin = Vector2.zero;
        musicLabel.rectTransform.offsetMax = Vector2.zero;

        var musicSlider = CreateSlider(card.transform, "MusicVolumeSlider", 0.5f);
        SetAnchors(musicSlider.GetComponent<RectTransform>(), 0.05f, 0.34f, 0.95f, 0.48f);
        musicSlider.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        musicSlider.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Mute Toggle
        var muteContainer = CreateEmpty(card.transform, "MuteContainer");
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

        // Back Button
        var backBtn = CreateButton(canvas.transform, "BackButton", "Back", 36, Purple, TextWhite);
        SetAnchors(backBtn.GetComponent<RectTransform>(), 0.2f, 0.25f, 0.8f, 0.35f);
        backBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        backBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Wire SceneLoader for back button
        var sceneLoader = canvas.gameObject.AddComponent<SceneLoader>();
        UnityEventTools.AddPersistentListener(backBtn.onClick, sceneLoader.LoadMainMenu);

        // Wire AudioSettingsUI
        var audioUI = canvas.gameObject.AddComponent<AudioSettingsUI>();
        var aso = new SerializedObject(audioUI);
        aso.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
        aso.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
        aso.FindProperty("muteToggle").objectReferenceValue = muteToggle;
        aso.ApplyModifiedProperties();

        SaveScene(scene, "Assets/Scenes/Settings.unity");
    }

    // ════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ════════════════════════════════════════════════════════════════

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

    private static Button CreateButton(Transform parent, string name, string label, int fontSize, Color bgColor, Color textColor)
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

        // Text child
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        StretchFull(textObj.GetComponent<RectTransform>());

        return btn;
    }

    private static Slider CreateSlider(Transform parent, string name, float defaultValue)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        // Background
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(obj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.15f);
        StretchFull(bgObj.GetComponent<RectTransform>());

        // Fill area
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(obj.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        StretchFull(fillAreaRect);
        fillAreaRect.offsetMin = new Vector2(0, 0);
        fillAreaRect.offsetMax = new Vector2(0, 0);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = Purple;
        StretchFull(fill.GetComponent<RectTransform>());

        // Handle slide area
        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(obj.transform, false);
        StretchFull(handleArea.AddComponent<RectTransform>());

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = TextWhite;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(40, 0);

        // Slider component
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

        // Background
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(obj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.2f);
        StretchFull(bgObj.GetComponent<RectTransform>());

        // Checkmark
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
