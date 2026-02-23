using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple helper that wires a Button click to a UIManager method by action name.
/// Attached by SceneBuilder to buttons that need to call UIManager at runtime.
/// </summary>
public class UIButtonAction : MonoBehaviour
{
    [SerializeField] private string action;

    private void Start()
    {
        var btn = GetComponent<Button>();
        if (btn == null) return;

        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (UIManager.Instance == null) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();

        switch (action)
        {
            case "ShowMainMenu":
                UIManager.Instance.ShowMainMenu();
                break;
            case "ShowLevelSelect":
                UIManager.Instance.ShowLevelSelect();
                break;
            case "ShowSettings":
                UIManager.Instance.ShowSettings();
                break;
            case "ShowShop":
                UIManager.Instance.ShowShop();
                break;
            case "ShowPause":
                UIManager.Instance.ShowPause();
                break;
            case "ResumePause":
                UIManager.Instance.ResumePause();
                break;
            case "BackToMainMenu":
                UIManager.Instance.BackToMainMenu();
                break;
            case "ShowDaily":
                if (DailyPuzzleManager.Instance != null)
                    DailyPuzzleManager.Instance.PlayDailyPuzzle();
                break;
            default:
                Debug.LogWarning($"[UIButtonAction] Unknown action: {action}");
                break;
        }
    }
}
