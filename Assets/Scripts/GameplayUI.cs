using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("Top Bar - Modern Layout")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button backButton;

    [Header("Stats Bar")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI comboText;

    [Header("Action Buttons")]
    [SerializeField] private Button hintButton;
    [SerializeField] private Button undoButton;

    [Header("Level Complete")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    private int moves = 0;
    private int combo = 0;
    private float timer = 0f;
    private bool isPlaying = true;

    private void Start()
    {
        if (levelCompletePanel) levelCompletePanel.SetActive(false);
        if (backButton) backButton.onClick.AddListener(OnBackClick);
        if (hintButton) hintButton.onClick.AddListener(OnHintClick);
        if (undoButton) undoButton.onClick.AddListener(OnUndoClick);
        if (nextLevelButton) nextLevelButton.onClick.AddListener(OnNextLevel);
        if (menuButton) menuButton.onClick.AddListener(OnBackClick);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameplayMusic();

        UpdateCombo(0);
    }

    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            if (timerText) timerText.text = $"⏱ {minutes}:{seconds:00}";
        }
    }

    public void AddMove()
    {
        moves++;
        if (movesText) movesText.text = $"Moves: {moves}";
    }

    public void UpdateCombo(int newCombo)
    {
        combo = newCombo;
        if (comboText) comboText.text = combo > 1 ? $"Combo: x{combo}" : "";
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"⭐ {score}";
    }

    public void SetLevel(int level)
    {
        if (levelText) levelText.text = $"Level {level}";
    }

    public void ShowLevelComplete()
    {
        isPlaying = false;
        if (levelCompletePanel) levelCompletePanel.SetActive(true);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelComplete();
    }

    private void OnBackClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnHintClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        Debug.Log("Hint used!");
    }

    private void OnUndoClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        Debug.Log("Undo!");
    }

    private void OnNextLevel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("Gameplay");
    }
}
