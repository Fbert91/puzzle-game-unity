using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tutorial Manager - Onboarding system for first 3 levels
/// Pitou speech bubble overlay, progressive disclosure, highlight system
/// Level 1 = tap basics, Level 2 = swap mechanic, Level 3 = pattern matching
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        public string pitouMessage;
        public int highlightTileX;
        public int highlightTileY;
        public bool highlightAll;
        public float delay;
        public bool waitForAction;
        public string requiredAction; // "tap", "swap", "pattern"
    }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialOverlay;
    [SerializeField] private Image dimBackground;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Text speechBubbleText;
    [SerializeField] private GameObject highlightFrame;
    [SerializeField] private Button skipButton;
    [SerializeField] private Text skipButtonText;
    [SerializeField] private Button tapToContinueButton;

    [Header("Settings")]
    [SerializeField] private float dimAlpha = 0.6f;
    [SerializeField] private float speechAnimSpeed = 0.03f;
    [SerializeField] private float stepDelay = 0.5f;

    private Dictionary<int, List<TutorialStep>> tutorialLevels = new Dictionary<int, List<TutorialStep>>();
    private int currentStepIndex = 0;
    private int currentTutorialLevel = 0;
    private bool isTutorialActive = false;
    private bool waitingForAction = false;
    private bool hasCompletedTutorial = false;
    private Coroutine tutorialCoroutine;
    private Coroutine typewriterCoroutine;

    private const string TUTORIAL_COMPLETE_KEY = "TutorialCompleted";

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
        hasCompletedTutorial = PlayerPrefs.GetInt(TUTORIAL_COMPLETE_KEY, 0) == 1;
        InitializeTutorialData();

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(false);

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
            skipButton.gameObject.SetActive(hasCompletedTutorial);
        }

        if (tapToContinueButton != null)
        {
            tapToContinueButton.onClick.AddListener(OnTapToContinue);
        }
    }

    /// <summary>
    /// Initialize tutorial steps for levels 1-3
    /// </summary>
    private void InitializeTutorialData()
    {
        // Level 1: Tap basics
        tutorialLevels[1] = new List<TutorialStep>
        {
            new TutorialStep
            {
                pitouMessage = "Hi there! I'm Pitou! 🐱\nLet me show you how to play!",
                highlightAll = false,
                delay = 0.5f,
                waitForAction = false
            },
            new TutorialStep
            {
                pitouMessage = "Tap on tiles to select them!\nTry tapping this one! 👆",
                highlightTileX = 2,
                highlightTileY = 2,
                delay = 0.3f,
                waitForAction = true,
                requiredAction = "tap"
            },
            new TutorialStep
            {
                pitouMessage = "Great job! 🌟\nNow select tiles that add up\nto the target number!",
                highlightAll = false,
                delay = 0.3f,
                waitForAction = false
            },
            new TutorialStep
            {
                pitouMessage = "Try selecting more tiles!\nThe goal is shown at the top 👆",
                highlightAll = true,
                delay = 0.3f,
                waitForAction = true,
                requiredAction = "tap"
            },
            new TutorialStep
            {
                pitouMessage = "Purr-fect! You're a natural! 😻\nNow solve the puzzle!",
                delay = 0.3f,
                waitForAction = false
            }
        };

        // Level 2: Swap mechanic
        tutorialLevels[2] = new List<TutorialStep>
        {
            new TutorialStep
            {
                pitouMessage = "New trick! 🐾\nYou can swap tiles now!",
                delay = 0.5f,
                waitForAction = false
            },
            new TutorialStep
            {
                pitouMessage = "Select two tiles next to each other\nto swap them! Try it! 🔄",
                highlightTileX = 1,
                highlightTileY = 1,
                delay = 0.3f,
                waitForAction = true,
                requiredAction = "swap"
            },
            new TutorialStep
            {
                pitouMessage = "Meow-velous! 🎉\nSwapping helps you create\nthe right patterns!",
                delay = 0.3f,
                waitForAction = false
            }
        };

        // Level 3: Pattern matching
        tutorialLevels[3] = new List<TutorialStep>
        {
            new TutorialStep
            {
                pitouMessage = "Time for patterns! 🧩\nConnect matching tiles!",
                delay = 0.5f,
                waitForAction = false
            },
            new TutorialStep
            {
                pitouMessage = "Find tiles with the same value\nand connect them! 🔗",
                highlightAll = true,
                delay = 0.3f,
                waitForAction = true,
                requiredAction = "pattern"
            },
            new TutorialStep
            {
                pitouMessage = "Amazing! You've learned\nall the basics! 🌟🐱\nGo solve some puzzles!",
                delay = 0.3f,
                waitForAction = false
            }
        };
    }

    /// <summary>
    /// Check if tutorial should run for this level
    /// </summary>
    public bool ShouldShowTutorial(int levelId)
    {
        if (hasCompletedTutorial && !tutorialLevels.ContainsKey(levelId))
            return false;

        return tutorialLevels.ContainsKey(levelId) && !hasCompletedTutorial;
    }

    /// <summary>
    /// Start tutorial for a level
    /// </summary>
    public void StartTutorial(int levelId)
    {
        if (!tutorialLevels.ContainsKey(levelId)) return;

        currentTutorialLevel = levelId;
        currentStepIndex = 0;
        isTutorialActive = true;

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(true);

        if (skipButton != null)
            skipButton.gameObject.SetActive(hasCompletedTutorial);

        if (tutorialCoroutine != null)
            StopCoroutine(tutorialCoroutine);

        tutorialCoroutine = StartCoroutine(RunTutorial());
    }

    /// <summary>
    /// Run tutorial step by step
    /// </summary>
    private IEnumerator RunTutorial()
    {
        List<TutorialStep> steps = tutorialLevels[currentTutorialLevel];

        while (currentStepIndex < steps.Count)
        {
            TutorialStep step = steps[currentStepIndex];

            yield return new WaitForSeconds(step.delay);

            // Show speech bubble with message
            ShowSpeechBubble(step.pitouMessage);

            // Handle highlight
            if (step.highlightAll)
            {
                ShowDimBackground(true);
                if (highlightFrame != null)
                    highlightFrame.SetActive(false);
            }
            else if (step.highlightTileX >= 0 && step.highlightTileY >= 0)
            {
                ShowDimBackground(true);
                HighlightTile(step.highlightTileX, step.highlightTileY);
            }
            else
            {
                ShowDimBackground(false);
                if (highlightFrame != null)
                    highlightFrame.SetActive(false);
            }

            // Wait for action or tap
            if (step.waitForAction)
            {
                waitingForAction = true;
                while (waitingForAction && isTutorialActive)
                {
                    yield return null;
                }
            }
            else
            {
                // Wait for tap to continue
                waitingForAction = true;
                if (tapToContinueButton != null)
                    tapToContinueButton.gameObject.SetActive(true);

                while (waitingForAction && isTutorialActive)
                {
                    yield return null;
                }

                if (tapToContinueButton != null)
                    tapToContinueButton.gameObject.SetActive(false);
            }

            currentStepIndex++;
        }

        EndTutorial();
    }

    /// <summary>
    /// Show the speech bubble with typewriter effect
    /// </summary>
    private void ShowSpeechBubble(string message)
    {
        if (speechBubble != null)
            speechBubble.SetActive(true);

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypewriterEffect(message));

        // Pitou speech
        if (PitouManager.Instance != null)
            PitouManager.Instance.ShowSpeechBubble(message);
    }

    private IEnumerator TypewriterEffect(string message)
    {
        if (speechBubbleText == null) yield break;

        speechBubbleText.text = "";
        for (int i = 0; i < message.Length; i++)
        {
            speechBubbleText.text += message[i];
            yield return new WaitForSeconds(speechAnimSpeed);
        }
    }

    /// <summary>
    /// Show/hide dim background
    /// </summary>
    private void ShowDimBackground(bool show)
    {
        if (dimBackground != null)
        {
            Color c = dimBackground.color;
            c.a = show ? dimAlpha : 0f;
            dimBackground.color = c;
        }
    }

    /// <summary>
    /// Highlight a specific tile
    /// </summary>
    private void HighlightTile(int x, int y)
    {
        if (highlightFrame == null) return;

        highlightFrame.SetActive(true);
        // Position highlight frame over the tile (would need board reference for exact positioning)
        Debug.Log($"[Tutorial] Highlighting tile ({x},{y})");
    }

    /// <summary>
    /// Called when player performs the required tutorial action
    /// </summary>
    public void OnTutorialAction(string actionType)
    {
        if (!isTutorialActive || !waitingForAction) return;

        List<TutorialStep> steps = tutorialLevels[currentTutorialLevel];
        if (currentStepIndex < steps.Count)
        {
            TutorialStep step = steps[currentStepIndex];
            if (step.waitForAction && step.requiredAction == actionType)
            {
                waitingForAction = false;
            }
        }
    }

    /// <summary>
    /// Tap to continue handler
    /// </summary>
    private void OnTapToContinue()
    {
        waitingForAction = false;
    }

    /// <summary>
    /// Skip the tutorial
    /// </summary>
    public void SkipTutorial()
    {
        EndTutorial();
    }

    /// <summary>
    /// End the tutorial
    /// </summary>
    private void EndTutorial()
    {
        isTutorialActive = false;
        waitingForAction = false;

        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(false);

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (highlightFrame != null)
            highlightFrame.SetActive(false);

        ShowDimBackground(false);

        // Mark tutorial as completed after level 3
        if (currentTutorialLevel >= 3)
        {
            hasCompletedTutorial = true;
            PlayerPrefs.SetInt(TUTORIAL_COMPLETE_KEY, 1);
            PlayerPrefs.Save();
        }

        Debug.Log($"[Tutorial] Tutorial ended for level {currentTutorialLevel}");
    }

    /// <summary>
    /// Check if tutorial is currently active
    /// </summary>
    public bool IsTutorialActive() => isTutorialActive;

    /// <summary>
    /// Check if tutorials have been completed
    /// </summary>
    public bool HasCompletedTutorial() => hasCompletedTutorial;
}
