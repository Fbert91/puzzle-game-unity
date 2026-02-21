using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pitou Personality System - Singleton managing Pitou's reactions across all screens
/// Reactions tied to game events: level complete, wrong move, streak, idle, daily puzzle
/// Speech bubble UI with animation, 5+ text variants per reaction type
/// </summary>
public class PitouManager : MonoBehaviour
{
    public static PitouManager Instance { get; private set; }

    public enum ReactionType
    {
        LevelComplete,
        WrongMove,
        StreakMilestone,
        Idle,
        DailyAvailable,
        HintGiven,
        Encouraging,
        Thinking,
        Excited
    }

    [System.Serializable]
    public class ReactionData
    {
        public ReactionType type;
        public string[] messages;
        public string animationTrigger;
        public string pitouSound;
    }

    [Header("UI References")]
    [SerializeField] private GameObject speechBubbleObject;
    [SerializeField] private Text speechBubbleText;
    [SerializeField] private Image pitouImage;
    [SerializeField] private Animator pitouAnimator;

    [Header("Settings")]
    [SerializeField] private float speechBubbleDuration = 3f;
    [SerializeField] private float idleTimeout = 5f;
    [SerializeField] private float typewriterSpeed = 0.03f;

    // Reaction messages
    private Dictionary<ReactionType, ReactionData> reactions = new Dictionary<ReactionType, ReactionData>();

    // State
    private float lastInteractionTime;
    private bool isSpeechBubbleVisible = false;
    private bool isIdleTriggered = false;
    private Coroutine speechCoroutine;
    private Coroutine idleCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeReactions();
        lastInteractionTime = Time.time;

        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);

        // Start idle check
        idleCoroutine = StartCoroutine(IdleCheck());
    }

    /// <summary>
    /// Initialize all reaction data
    /// </summary>
    private void InitializeReactions()
    {
        reactions[ReactionType.LevelComplete] = new ReactionData
        {
            type = ReactionType.LevelComplete,
            messages = new string[] {
                "Amazing! 🐱", "Wow! 🌟", "Purr-fect! 😻",
                "Nailed it! 🎯", "Meow-velous! ✨",
                "You're brilliant! 🧠", "Incredible! 🎉"
            },
            animationTrigger = "Celebrating",
            pitouSound = "excited"
        };

        reactions[ReactionType.WrongMove] = new ReactionData
        {
            type = ReactionType.WrongMove,
            messages = new string[] {
                "Hmm... 🤔", "Try again! 💪", "Almost! 😸",
                "Don't give up! 🐾", "Close one! 🌀",
                "Keep trying! ✨", "You got this! 💫"
            },
            animationTrigger = "Thinking",
            pitouSound = "sad"
        };

        reactions[ReactionType.StreakMilestone] = new ReactionData
        {
            type = ReactionType.StreakMilestone,
            messages = new string[] {
                "On fire! 🔥🐱", "Unstoppable! 💥",
                "Streak champion! 🏆", "You're amazing! 🌟",
                "Keep it going! 🎯", "Legendary! ⚡"
            },
            animationTrigger = "Celebrating",
            pitouSound = "excited"
        };

        reactions[ReactionType.Idle] = new ReactionData
        {
            type = ReactionType.Idle,
            messages = new string[] {
                "*yawn* 😴", "*stretches* 🐱",
                "*blinks slowly* 😸", "*plays with tail* 🐾",
                "*purrs quietly* 💤", "*looks around curiously* 👀"
            },
            animationTrigger = "Idle",
            pitouSound = ""
        };

        reactions[ReactionType.DailyAvailable] = new ReactionData
        {
            type = ReactionType.DailyAvailable,
            messages = new string[] {
                "Daily puzzle ready! 📅", "New puzzle today! 🎁",
                "Come solve today's! 🧩", "Fresh puzzle awaits! ✨",
                "Let's play today's! 🐱"
            },
            animationTrigger = "Happy",
            pitouSound = "happy"
        };

        reactions[ReactionType.HintGiven] = new ReactionData
        {
            type = ReactionType.HintGiven,
            messages = new string[] {
                "Try this one! 🐾", "Look here! 👀",
                "Psst... this might help! 😸", "I found something! 🌟",
                "How about this? 🐱"
            },
            animationTrigger = "Thinking",
            pitouSound = "thinking"
        };

        reactions[ReactionType.Encouraging] = new ReactionData
        {
            type = ReactionType.Encouraging,
            messages = new string[] {
                "You can do it! 💪", "Believe in yourself! 🌟",
                "Almost there! 🎯", "So close! ✨",
                "Keep going! 🐱"
            },
            animationTrigger = "Encouraging",
            pitouSound = "happy"
        };

        reactions[ReactionType.Thinking] = new ReactionData
        {
            type = ReactionType.Thinking,
            messages = new string[] {
                "Hmm... 🤔", "Let me think... 💭",
                "Interesting... 🧐", "I see... 👀",
                "What if... 💡"
            },
            animationTrigger = "Thinking",
            pitouSound = "thinking"
        };

        reactions[ReactionType.Excited] = new ReactionData
        {
            type = ReactionType.Excited,
            messages = new string[] {
                "Yay! 🎉", "Woohoo! 🌟", "So exciting! ✨",
                "Let's goooo! 🚀", "Awesome! 💫",
                "This is great! 🐱"
            },
            animationTrigger = "Celebrating",
            pitouSound = "excited"
        };
    }

    /// <summary>
    /// Trigger a reaction by type
    /// </summary>
    public void TriggerReaction(ReactionType type)
    {
        lastInteractionTime = Time.time;
        isIdleTriggered = false;

        if (!reactions.ContainsKey(type)) return;

        ReactionData reaction = reactions[type];

        // Pick random message
        string message = reaction.messages[Random.Range(0, reaction.messages.Length)];

        // Show speech bubble
        ShowSpeechBubble(message);

        // Trigger animation
        if (pitouAnimator != null && !string.IsNullOrEmpty(reaction.animationTrigger))
        {
            pitouAnimator.SetTrigger(reaction.animationTrigger);
        }

        // Play sound
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(reaction.pitouSound))
        {
            SoundManager.Instance.PlayPitouSound(reaction.pitouSound);
        }
    }

    /// <summary>
    /// Called when level is completed
    /// </summary>
    public void OnLevelComplete(int stars)
    {
        TriggerReaction(ReactionType.LevelComplete);
    }

    /// <summary>
    /// Called on wrong move
    /// </summary>
    public void OnWrongMove()
    {
        TriggerReaction(ReactionType.WrongMove);
    }

    /// <summary>
    /// Called on streak milestone
    /// </summary>
    public void OnStreakMilestone(int streakDay)
    {
        TriggerReaction(ReactionType.StreakMilestone);
    }

    /// <summary>
    /// Called when daily puzzle is available
    /// </summary>
    public void OnDailyPuzzleAvailable()
    {
        TriggerReaction(ReactionType.DailyAvailable);
    }

    /// <summary>
    /// Show speech bubble with message
    /// </summary>
    public void ShowSpeechBubble(string message)
    {
        if (speechCoroutine != null)
            StopCoroutine(speechCoroutine);

        speechCoroutine = StartCoroutine(SpeechBubbleCoroutine(message));
    }

    /// <summary>
    /// Speech bubble animation coroutine
    /// </summary>
    private IEnumerator SpeechBubbleCoroutine(string message)
    {
        isSpeechBubbleVisible = true;

        if (speechBubbleObject != null)
        {
            speechBubbleObject.SetActive(true);
            speechBubbleObject.transform.localScale = Vector3.zero;

            // Pop-in animation
            float elapsed = 0f;
            float popDuration = 0.2f;
            while (elapsed < popDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / popDuration;
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.1f;
                speechBubbleObject.transform.localScale = Vector3.one * Mathf.Min(scale, 1.05f);
                yield return null;
            }
            speechBubbleObject.transform.localScale = Vector3.one;
        }

        // Typewriter text
        if (speechBubbleText != null)
        {
            speechBubbleText.text = "";
            for (int i = 0; i < message.Length; i++)
            {
                speechBubbleText.text += message[i];
                yield return new WaitForSeconds(typewriterSpeed);
            }
        }

        // Wait for duration
        yield return new WaitForSeconds(speechBubbleDuration);

        // Pop-out animation
        if (speechBubbleObject != null)
        {
            float elapsed2 = 0f;
            float popOutDuration = 0.15f;
            while (elapsed2 < popOutDuration)
            {
                elapsed2 += Time.deltaTime;
                float t = 1f - (elapsed2 / popOutDuration);
                speechBubbleObject.transform.localScale = Vector3.one * t;
                yield return null;
            }
            speechBubbleObject.SetActive(false);
        }

        isSpeechBubbleVisible = false;
    }

    /// <summary>
    /// Hide speech bubble immediately
    /// </summary>
    public void HideSpeechBubble()
    {
        if (speechCoroutine != null)
            StopCoroutine(speechCoroutine);

        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);

        isSpeechBubbleVisible = false;
    }

    /// <summary>
    /// Idle check coroutine - triggers idle animation after timeout
    /// </summary>
    private IEnumerator IdleCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (!isIdleTriggered && Time.time - lastInteractionTime > idleTimeout)
            {
                isIdleTriggered = true;
                TriggerReaction(ReactionType.Idle);
            }
        }
    }

    /// <summary>
    /// Register player interaction (resets idle timer)
    /// </summary>
    public void RegisterInteraction()
    {
        lastInteractionTime = Time.time;
        isIdleTriggered = false;
    }

    /// <summary>
    /// Set Pitou image/sprite
    /// </summary>
    public void SetPitouSprite(Sprite sprite)
    {
        if (pitouImage != null)
            pitouImage.sprite = sprite;
    }

    // Public getters
    public bool IsSpeechBubbleVisible() => isSpeechBubbleVisible;
}
