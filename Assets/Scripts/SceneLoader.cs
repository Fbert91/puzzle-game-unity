using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName) { SceneManager.LoadScene(sceneName); }
    public void LoadMainMenu() { SceneManager.LoadScene("MainMenu"); }
    public void LoadGameplay() { SceneManager.LoadScene("Gameplay"); }
    public void LoadSettings() { SceneManager.LoadScene("Settings"); }
    public void QuitGame() { Application.Quit(); }
}
