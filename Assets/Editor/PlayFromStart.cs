using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Forces Play mode to always start from SplashScreen scene.
/// Toggle via BrainBlast > Play From Start (checkmark = enabled)
/// </summary>
[InitializeOnLoad]
public static class PlayFromStart
{
    private const string MenuPath = "BrainBlast/Play From Start";
    private const string PrefKey = "BrainBlast_PlayFromStart";
    private const string StartScene = "Assets/Scenes/SplashScreen.unity";

    static PlayFromStart()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool current = EditorPrefs.GetBool(PrefKey, true);
        EditorPrefs.SetBool(PrefKey, !current);
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PrefKey, true));
        return true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode && EditorPrefs.GetBool(PrefKey, true))
        {
            if (EditorSceneManager.GetActiveScene().path != StartScene)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(StartScene);
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}
