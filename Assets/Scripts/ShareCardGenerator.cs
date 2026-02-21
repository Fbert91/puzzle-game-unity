using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

/// <summary>
/// Share Card Generator - Generate shareable images on level complete
/// "I solved BrainBlast Level X in Y moves! 🧠" + Pitou sprite + branding
/// Native share dialog on Android (Intent.ACTION_SEND)
/// </summary>
public class ShareCardGenerator : MonoBehaviour
{
    public static ShareCardGenerator Instance { get; private set; }

    [Header("Share Card")]
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Canvas shareCardCanvas;
    [SerializeField] private Text shareCardTitleText;
    [SerializeField] private Text shareCardScoreText;
    [SerializeField] private Text shareCardBrandingText;
    [SerializeField] private Image shareCardPitouImage;
    [SerializeField] private Image shareCardBackground;
    [SerializeField] private Image[] shareCardStars;

    [Header("Settings")]
    [SerializeField] private int cardWidth = 1080;
    [SerializeField] private int cardHeight = 1920;
    [SerializeField] private string shareHashtag = "#BrainBlast";
    [SerializeField] private string appUrl = "https://play.google.com/store/apps/details?id=com.fbert91.puzzlegame";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Generate share card and open native share dialog
    /// </summary>
    public void GenerateAndShare(int levelId, int moves, int stars)
    {
        StartCoroutine(GenerateShareCardCoroutine(levelId, moves, stars));
    }

    /// <summary>
    /// Generate the share card image
    /// </summary>
    private IEnumerator GenerateShareCardCoroutine(int levelId, int moves, int stars)
    {
        // Update share card content
        string levelText;
        if (levelId == -1)
            levelText = "I solved BrainBlast's Daily Puzzle! 🧠";
        else
            levelText = $"I solved BrainBlast Level {levelId} in {moves} moves! 🧠";

        if (shareCardTitleText != null)
            shareCardTitleText.text = levelText;

        if (shareCardScoreText != null)
        {
            string starStr = "";
            for (int i = 0; i < stars; i++) starStr += "⭐";
            shareCardScoreText.text = starStr;
        }

        if (shareCardBrandingText != null)
            shareCardBrandingText.text = "BrainBlast Puzzle Game";

        // Update stars
        if (shareCardStars != null)
        {
            for (int i = 0; i < shareCardStars.Length; i++)
            {
                if (shareCardStars[i] != null)
                {
                    shareCardStars[i].color = i < stars ?
                        new Color(1f, 0.84f, 0f) : new Color(0.5f, 0.5f, 0.5f, 0.3f);
                }
            }
        }

        // Apply theme colors
        if (shareCardBackground != null && ThemeManager.Instance != null)
        {
            shareCardBackground.color = ThemeManager.Instance.PrimaryColor;
        }

        yield return new WaitForEndOfFrame();

        // Generate image
        Texture2D shareImage = GenerateCardTexture(levelId, moves, stars);

        // Save to file
        string filePath = SaveShareImage(shareImage);

        // Share
        ShareImage(filePath, levelText);

        // Cleanup
        Destroy(shareImage);
    }

    /// <summary>
    /// Generate share card as a texture (procedural if no render camera)
    /// </summary>
    private Texture2D GenerateCardTexture(int levelId, int moves, int stars)
    {
        // If we have a render camera, use it
        if (renderCamera != null && shareCardCanvas != null)
        {
            RenderTexture rt = new RenderTexture(cardWidth, cardHeight, 24);
            renderCamera.targetTexture = rt;
            renderCamera.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(cardWidth, cardHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, cardWidth, cardHeight), 0, 0);
            tex.Apply();

            renderCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            return tex;
        }

        // Fallback: generate a simple card programmatically
        Texture2D card = new Texture2D(cardWidth, cardHeight, TextureFormat.RGB24, false);

        // Fill background
        Color bgColor = ThemeManager.Instance != null ?
            ThemeManager.Instance.PrimaryColor : new Color(0.608f, 0.498f, 1f);

        Color[] pixels = new Color[cardWidth * cardHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            // Gradient from top to bottom
            float y = (float)(i / cardWidth) / cardHeight;
            pixels[i] = Color.Lerp(bgColor, bgColor * 0.7f, y);
        }
        card.SetPixels(pixels);
        card.Apply();

        return card;
    }

    /// <summary>
    /// Save share image to persistent storage
    /// </summary>
    private string SaveShareImage(Texture2D image)
    {
        byte[] bytes = image.EncodeToPNG();
        string fileName = $"BrainBlast_Share_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"[ShareCard] Saved to: {filePath}");
        return filePath;
    }

    /// <summary>
    /// Share image using native share dialog
    /// </summary>
    private void ShareImage(string imagePath, string shareText)
    {
        string fullText = $"{shareText}\n\n{shareHashtag}\nDownload: {appUrl}";

#if UNITY_ANDROID && !UNITY_EDITOR
        ShareOnAndroid(imagePath, fullText);
#elif UNITY_IOS && !UNITY_EDITOR
        ShareOnIOS(imagePath, fullText);
#else
        Debug.Log($"[ShareCard] Share text: {fullText}");
        Debug.Log($"[ShareCard] Image path: {imagePath}");
#endif
    }

    /// <summary>
    /// Android native share using Intent.ACTION_SEND
    /// </summary>
    private void ShareOnAndroid(string imagePath, string text)
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "image/*");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);

            // Get content URI using FileProvider
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            AndroidJavaClass fileProviderClass = new AndroidJavaClass("androidx.core.content.FileProvider");
            AndroidJavaObject file = new AndroidJavaObject("java.io.File", imagePath);

            string authority = context.Call<string>("getPackageName") + ".fileprovider";
            AndroidJavaObject uri = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", context, authority, file);

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uri);
            intentObject.Call<AndroidJavaObject>("addFlags", intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));

            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share BrainBlast Score!");
            currentActivity.Call("startActivity", chooser);

            Debug.Log("[ShareCard] Android share dialog opened");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ShareCard] Android share failed: {e.Message}");
        }
#endif
    }

    /// <summary>
    /// iOS native share (placeholder)
    /// </summary>
    private void ShareOnIOS(string imagePath, string text)
    {
        // iOS implementation would use NativeShare plugin or native bridge
        Debug.Log($"[ShareCard] iOS share: {text}");
    }
}
