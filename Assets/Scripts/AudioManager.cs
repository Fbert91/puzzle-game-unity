using UnityEngine;

/// <summary>
/// AudioManager - Singleton for managing all game audio (SFX and music)
/// Persists across scenes and provides easy API for audio playback
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // Audio clips - drag and drop in inspector
    [SerializeField] private AudioClip tilePickupSFX;
    [SerializeField] private AudioClip levelCompleteSFX;
    [SerializeField] private AudioClip invalidMoveSFX;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip gameplayBGM;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    [SerializeField] private float sfxVolume = 0.7f;
    [SerializeField] private float musicVolume = 0.5f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
    }

    // ==================== SFX Methods ====================

    /// <summary>
    /// Play tile pickup/selection sound
    /// Happy pop sound for user feedback
    /// </summary>
    public void PlayTilePickup()
    {
        if (tilePickupSFX != null)
            sfxSource.PlayOneShot(tilePickupSFX, sfxVolume);
    }

    /// <summary>
    /// Play level complete/success sound
    /// Triumphant fanfare for victory feedback
    /// </summary>
    public void PlayLevelComplete()
    {
        if (levelCompleteSFX != null)
            sfxSource.PlayOneShot(levelCompleteSFX, sfxVolume * 1.2f);  // Slightly louder
    }

    /// <summary>
    /// Play invalid move/error sound
    /// Gentle buzz for invalid action feedback
    /// </summary>
    public void PlayInvalidMove()
    {
        if (invalidMoveSFX != null)
            sfxSource.PlayOneShot(invalidMoveSFX, sfxVolume * 0.8f);  // Slightly quieter
    }

    /// <summary>
    /// Play button click sound
    /// Subtle feedback for UI interactions
    /// </summary>
    public void PlayButtonClick()
    {
        if (buttonClickSFX != null)
            sfxSource.PlayOneShot(buttonClickSFX, sfxVolume);
    }

    // ==================== Music Methods ====================

    /// <summary>
    /// Play menu background music
    /// Upbeat, non-intrusive loop for menu screens
    /// </summary>
    public void PlayMenuMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        if (menuBGM != null)
        {
            musicSource.clip = menuBGM;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Play gameplay background music
    /// Energetic puzzle-solving vibe for active gameplay
    /// </summary>
    public void PlayGameplayMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        if (gameplayBGM != null)
        {
            musicSource.clip = gameplayBGM;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stop music playback
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ==================== Volume Control ====================

    /// <summary>
    /// Set SFX volume (0.0 - 1.0)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Set music volume (0.0 - 1.0)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Get current SFX volume
    /// </summary>
    public float GetSFXVolume() => sfxVolume;

    /// <summary>
    /// Get current music volume
    /// </summary>
    public float GetMusicVolume() => musicVolume;
}
