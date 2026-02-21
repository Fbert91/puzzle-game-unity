using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button levelsButton;

    private void Start()
    {
        if (playButton) playButton.onClick.AddListener(OnPlayClick);
        if (settingsButton) settingsButton.onClick.AddListener(OnSettingsClick);
        if (levelsButton) levelsButton.onClick.AddListener(OnLevelsClick);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMenuMusic();
    }

    private void OnPlayClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("Gameplay");
    }

    private void OnSettingsClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("Settings");
    }

    private void OnLevelsClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        Debug.Log("Level select coming soon!");
    }
}
