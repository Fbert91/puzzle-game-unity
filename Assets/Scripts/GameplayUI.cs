using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    private int moves = 0;
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
    }

    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            if (timerText) timerText.text = $"Time: {Mathf.FloorToInt(timer / 60)}:{Mathf.FloorToInt(timer % 60):00}";
        }
    }

    public void AddMove()
    {
        moves++;
        if (movesText) movesText.text = $"Moves: {moves}";
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"\u2B50 {score}";
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
