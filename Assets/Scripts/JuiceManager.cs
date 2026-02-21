using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Juice Manager - Game Feel System
/// Provides visual feedback for all game interactions
/// Singleton with easy API: JuiceManager.Instance.Squish(tile), .Shake(), .Glow(tile), etc.
/// </summary>
public class JuiceManager : MonoBehaviour
{
    public static JuiceManager Instance { get; private set; }

    [Header("Squish Settings")]
    [SerializeField] private float squishScale = 0.9f;
    [SerializeField] private float squishDuration = 0.15f;
    [SerializeField] private float squishBounce = 1.05f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 3f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = new Color(0f, 1f, 0.5f, 0.8f);
    [SerializeField] private float glowDuration = 0.5f;

    [Header("Combo Settings")]
    [SerializeField] private float comboTextScale = 1.5f;
    [SerializeField] private float comboTextDuration = 0.8f;
    [SerializeField] private Color comboColor = new Color(1f, 0.84f, 0f);

    [Header("Camera Zoom")]
    [SerializeField] private float zoomAmount = 0.9f;
    [SerializeField] private float zoomDuration = 0.5f;

    // Combo tracking
    private int comboCount = 0;
    private float lastCorrectMoveTime = 0f;
    private float comboTimeout = 2f;

    // Reference to main camera
    private Camera mainCamera;
    private float originalCameraSize;
    private Vector3 originalCameraPosition;

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
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraSize = mainCamera.orthographicSize;
            originalCameraPosition = mainCamera.transform.position;
        }
    }

    /// <summary>
    /// Squish effect on tile tap (scale to 0.9, bounce back to 1.05, settle to 1.0)
    /// </summary>
    public void Squish(Transform target)
    {
        if (target == null) return;
        StartCoroutine(SquishCoroutine(target));
    }

    /// <summary>
    /// Squish with GameObject reference
    /// </summary>
    public void Squish(GameObject target)
    {
        if (target == null) return;
        Squish(target.transform);
    }

    private IEnumerator SquishCoroutine(Transform target)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.localScale;
        Vector3 squished = originalScale * squishScale;
        Vector3 bounced = originalScale * squishBounce;

        // Squish down
        float elapsed = 0f;
        float halfDuration = squishDuration * 0.4f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            if (target != null)
                target.localScale = Vector3.Lerp(originalScale, squished, t);
            yield return null;
        }

        // Bounce up
        elapsed = 0f;
        float bounceDuration = squishDuration * 0.3f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            if (target != null)
                target.localScale = Vector3.Lerp(squished, bounced, t);
            yield return null;
        }

        // Settle back
        elapsed = 0f;
        float settleDuration = squishDuration * 0.3f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settleDuration;
            if (target != null)
                target.localScale = Vector3.Lerp(bounced, originalScale, t);
            yield return null;
        }

        if (target != null)
            target.localScale = originalScale;
    }

    /// <summary>
    /// Screen shake for wrong moves (2-3px magnitude, 0.2s duration)
    /// </summary>
    public void Shake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
            StartCoroutine(ShakeCoroutine());
    }

    /// <summary>
    /// Shake a specific transform
    /// </summary>
    public void Shake(Transform target, float magnitude = -1f)
    {
        if (target != null)
            StartCoroutine(ShakeTransformCoroutine(target, magnitude > 0 ? magnitude : shakeMagnitude));
    }

    private IEnumerator ShakeCoroutine()
    {
        if (mainCamera == null) yield break;

        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = 1f - (elapsed / shakeDuration);
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude) * strength * 0.01f;
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude) * strength * 0.01f;

            mainCamera.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }

    private IEnumerator ShakeTransformCoroutine(Transform target, float magnitude)
    {
        if (target == null) yield break;

        Vector3 originalPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = 1f - (elapsed / shakeDuration);
            float offsetX = Random.Range(-magnitude, magnitude) * strength;
            float offsetY = Random.Range(-magnitude, magnitude) * strength;

            if (target != null)
                target.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        if (target != null)
            target.localPosition = originalPos;
    }

    /// <summary>
    /// Glow effect for correct moves
    /// </summary>
    public void Glow(Transform target)
    {
        if (target == null) return;
        StartCoroutine(GlowCoroutine(target));
    }

    /// <summary>
    /// Glow with GameObject
    /// </summary>
    public void Glow(GameObject target)
    {
        if (target != null) Glow(target.transform);
    }

    private IEnumerator GlowCoroutine(Transform target)
    {
        if (target == null) yield break;

        // Try to get a SpriteRenderer or Image
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        UnityEngine.UI.Image img = target.GetComponent<UnityEngine.UI.Image>();

        Color originalColor = Color.white;
        if (sr != null) originalColor = sr.color;
        else if (img != null) originalColor = img.color;

        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / glowDuration;

            // Pulse glow: ramp up then down
            float intensity = t < 0.3f ? t / 0.3f : 1f - ((t - 0.3f) / 0.7f);
            Color current = Color.Lerp(originalColor, glowColor, intensity);

            if (sr != null) sr.color = current;
            else if (img != null) img.color = current;

            yield return null;
        }

        if (sr != null) sr.color = originalColor;
        else if (img != null) img.color = originalColor;
    }

    /// <summary>
    /// Register a correct move for combo tracking
    /// </summary>
    public void RegisterCorrectMove()
    {
        if (Time.time - lastCorrectMoveTime < comboTimeout)
        {
            comboCount++;
        }
        else
        {
            comboCount = 1;
        }
        lastCorrectMoveTime = Time.time;

        if (comboCount >= 2)
        {
            ShowComboText(comboCount);
        }

        // Play correct sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("correct");
    }

    /// <summary>
    /// Register a wrong move
    /// </summary>
    public void RegisterWrongMove()
    {
        comboCount = 0;
        Shake();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("wrong");
    }

    /// <summary>
    /// Show combo counter with visual multiplier text
    /// </summary>
    public void ShowComboText(int combo)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("combo");

        // Create floating combo text
        StartCoroutine(FloatingComboText(combo));
    }

    private IEnumerator FloatingComboText(int combo)
    {
        // Create a temporary Canvas text for the combo
        GameObject comboObj = new GameObject("ComboText");
        Canvas canvas = comboObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(comboObj.transform);

        UnityEngine.UI.Text comboText = textObj.AddComponent<UnityEngine.UI.Text>();
        comboText.text = $"x{combo} COMBO!";
        comboText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        comboText.fontSize = 48;
        comboText.alignment = TextAnchor.MiddleCenter;
        comboText.color = comboColor;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 100);
        rect.sizeDelta = new Vector2(400, 100);

        float elapsed = 0f;
        Vector2 startPos = rect.anchoredPosition;

        while (elapsed < comboTextDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / comboTextDuration;

            // Float up and fade out
            rect.anchoredPosition = startPos + new Vector2(0, t * 80f);
            float scale = t < 0.2f ? t / 0.2f * comboTextScale : Mathf.Lerp(comboTextScale, 0.8f, (t - 0.2f) / 0.8f);
            rect.localScale = Vector3.one * scale;

            Color c = comboText.color;
            c.a = t < 0.7f ? 1f : 1f - ((t - 0.7f) / 0.3f);
            comboText.color = c;

            yield return null;
        }

        Destroy(comboObj);
    }

    /// <summary>
    /// Camera zoom on final puzzle move
    /// </summary>
    public void ZoomCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
            StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        if (mainCamera == null) yield break;

        float startSize = mainCamera.orthographicSize;
        float targetSize = startSize * zoomAmount;
        float elapsed = 0f;
        float halfDuration = zoomDuration * 0.5f;

        // Zoom in
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        // Zoom out
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            mainCamera.orthographicSize = Mathf.Lerp(targetSize, startSize, t);
            yield return null;
        }

        mainCamera.orthographicSize = startSize;
    }

    /// <summary>
    /// Reset combo counter
    /// </summary>
    public void ResetCombo()
    {
        comboCount = 0;
    }

    /// <summary>
    /// Get current combo count
    /// </summary>
    public int GetComboCount() => comboCount;

    /// <summary>
    /// Pulse animation (generic scale pulse)
    /// </summary>
    public void Pulse(Transform target, float pulseSscale = 1.15f, float duration = 0.3f)
    {
        if (target != null)
            StartCoroutine(PulseCoroutine(target, pulseSscale, duration));
    }

    private IEnumerator PulseCoroutine(Transform target, float pulseScale, float duration)
    {
        if (target == null) yield break;

        Vector3 original = target.localScale;
        Vector3 pulsed = original * pulseScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float wave = Mathf.Sin(t * Mathf.PI);
            if (target != null)
                target.localScale = Vector3.Lerp(original, pulsed, wave);
            yield return null;
        }

        if (target != null)
            target.localScale = original;
    }
}
