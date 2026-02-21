using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Level Complete Celebration System
/// 1-3 star rating, particle explosion, score tally animation, Pitou reactions
/// </summary>
public class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject celebrationPanel;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text movesText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text pitouReactionText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button shareButton;

    [Header("Animation Settings")]
    [SerializeField] private float starAnimDelay = 0.5f;
    [SerializeField] private float scoreTallySpeed = 50f;
    [SerializeField] private float celebrationDelay = 0.3f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem celebrationParticles;
    [SerializeField] private ParticleSystem starParticles;
    [SerializeField] private Color starActiveColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color starInactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    // Pitou celebration texts
    private static readonly string[] CelebrationTexts = {
        "Amazing! 🐱",
        "Wow! 🌟",
        "Purr-fect! 😻",
        "Nailed it! 🎯",
        "Meow-velous! ✨"
    };

    private int currentLevelId;
    private int earnedStars;
    private int finalScore;
    private int totalMoves;
    private float completionTime;
    private Coroutine tallyCoroutine;

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
        if (celebrationPanel != null)
            celebrationPanel.SetActive(false);

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                OnContinueClicked();
            });
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                OnReplayClicked();
            });
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                OnHomeClicked();
            });
        }

        if (shareButton != null)
        {
            shareButton.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
                ShareCardGenerator.Instance?.GenerateAndShare(currentLevelId, totalMoves, earnedStars);
            });
        }
    }

    /// <summary>
    /// Show level complete celebration
    /// </summary>
    public void ShowCelebration(int levelId, int score, int stars, int moves, float time)
    {
        currentLevelId = levelId;
        finalScore = score;
        earnedStars = stars;
        totalMoves = moves;
        completionTime = time;

        if (celebrationPanel != null)
            celebrationPanel.SetActive(true);

        // Reset stars
        if (starImages != null)
        {
            foreach (var star in starImages)
            {
                if (star != null)
                {
                    star.color = starInactiveColor;
                    star.transform.localScale = Vector3.zero;
                }
            }
        }

        // Reset score
        if (scoreText != null) scoreText.text = "0";
        if (movesText != null) movesText.text = $"Moves: {moves}";
        if (timeText != null) timeText.text = time > 0 ? $"Time: {time:F1}s" : "";

        // Start celebration sequence
        if (tallyCoroutine != null) StopCoroutine(tallyCoroutine);
        tallyCoroutine = StartCoroutine(CelebrationSequence());

        // Play celebration particles
        if (celebrationParticles != null)
            celebrationParticles.Play();

        // Play complete sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("complete");

        // Pitou reaction
        if (PitouManager.Instance != null)
            PitouManager.Instance.OnLevelComplete(stars);

        Debug.Log($"[LevelComplete] Level {levelId} complete! Score: {score}, Stars: {stars}");
    }

    /// <summary>
    /// Celebration animation sequence
    /// </summary>
    private IEnumerator CelebrationSequence()
    {
        yield return new WaitForSecondsRealtime(celebrationDelay);

        // Animate score tally
        float currentScore = 0;
        while (currentScore < finalScore)
        {
            currentScore = Mathf.MoveTowards(currentScore, finalScore, scoreTallySpeed * Time.unscaledDeltaTime * 10f);
            if (scoreText != null)
                scoreText.text = Mathf.RoundToInt(currentScore).ToString();
            yield return null;
        }
        if (scoreText != null)
            scoreText.text = finalScore.ToString();

        // Animate stars one by one
        for (int i = 0; i < earnedStars && i < starImages.Length; i++)
        {
            yield return new WaitForSecondsRealtime(starAnimDelay);

            if (starImages[i] != null)
            {
                starImages[i].color = starActiveColor;

                // Pop-in animation
                StartCoroutine(StarPopAnimation(starImages[i].transform));

                // Play star sound
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySound("star_earned");

                // Star particles
                if (starParticles != null)
                {
                    starParticles.transform.position = starImages[i].transform.position;
                    starParticles.Play();
                }
            }
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // Show Pitou reaction text
        if (pitouReactionText != null)
        {
            pitouReactionText.text = CelebrationTexts[Random.Range(0, CelebrationTexts.Length)];
            pitouReactionText.gameObject.SetActive(true);
            StartCoroutine(TextPopAnimation(pitouReactionText.transform));
        }
    }

    /// <summary>
    /// Star pop-in animation
    /// </summary>
    private IEnumerator StarPopAnimation(Transform starTransform)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Overshoot bounce
            float scale;
            if (t < 0.6f)
                scale = t / 0.6f * 1.3f;
            else
                scale = 1.3f - (t - 0.6f) / 0.4f * 0.3f;

            starTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        starTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// Text pop animation
    /// </summary>
    private IEnumerator TextPopAnimation(Transform textTransform)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        textTransform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.1f;
            if (t > 0.7f) scale = Mathf.Lerp(1.1f, 1f, (t - 0.7f) / 0.3f);

            textTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        textTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// Calculate stars based on moves and time
    /// </summary>
    public int CalculateStars(int levelId, int moves, float time)
    {
        ProceduralLevelGenerator.LevelData levelData = null;
        if (ProceduralLevelGenerator.Instance != null)
            levelData = ProceduralLevelGenerator.Instance.GetLevelData(levelId);

        if (levelData == null)
        {
            // Fallback calculation
            if (moves <= 15) return 3;
            if (moves <= 25) return 2;
            return 1;
        }

        // Moves-based star calculation
        if (moves <= levelData.starThresholds.star3)
            return 3;
        if (moves <= levelData.starThresholds.star2)
            return 2;
        return 1;
    }

    private void OnContinueClicked()
    {
        HideCelebration();
        int nextLevel = currentLevelId + 1;
        if (nextLevel <= 200 && UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameplay(nextLevel);
        }
        else if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMainMenu();
        }
    }

    private void OnReplayClicked()
    {
        HideCelebration();
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameplay(currentLevelId);
    }

    private void OnHomeClicked()
    {
        HideCelebration();
        if (UIManager.Instance != null)
            UIManager.Instance.ShowMainMenu();
    }

    /// <summary>
    /// Hide celebration panel
    /// </summary>
    public void HideCelebration()
    {
        if (tallyCoroutine != null)
        {
            StopCoroutine(tallyCoroutine);
            tallyCoroutine = null;
        }
        if (celebrationPanel != null)
            celebrationPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    /// <summary>
    /// Get celebration text for current stars
    /// </summary>
    public string GetCelebrationText()
    {
        return CelebrationTexts[Random.Range(0, CelebrationTexts.Length)];
    }
}
