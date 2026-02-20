using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AudioSettingsUI - Manages audio settings UI (sliders, toggles)
/// Integrates with AudioManager to control volume and mute
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Toggle muteToggle;

    private float previousSFXVolume = 0.7f;
    private float previousMusicVolume = 0.5f;

    private void Start()
    {
        // Subscribe to slider value changes
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (muteToggle != null)
            muteToggle.onValueChanged.AddListener(OnMuteToggled);

        // Initialize UI with current audio manager values
        if (AudioManager.Instance != null)
        {
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();

            // Store previous volumes for mute/unmute
            previousSFXVolume = AudioManager.Instance.GetSFXVolume();
            previousMusicVolume = AudioManager.Instance.GetMusicVolume();
        }
    }

    /// <summary>
    /// Called when SFX volume slider changes
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            previousSFXVolume = value;  // Track for unmute
        }
    }

    /// <summary>
    /// Called when music volume slider changes
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
            previousMusicVolume = value;  // Track for unmute
        }
    }

    /// <summary>
    /// Called when mute toggle changes
    /// </summary>
    private void OnMuteToggled(bool isMuted)
    {
        if (AudioManager.Instance == null)
            return;

        if (isMuted)
        {
            // Mute: Set volumes to 0
            AudioManager.Instance.SetSFXVolume(0);
            AudioManager.Instance.SetMusicVolume(0);
        }
        else
        {
            // Unmute: Restore previous volumes
            AudioManager.Instance.SetSFXVolume(previousSFXVolume);
            AudioManager.Instance.SetMusicVolume(previousMusicVolume);
        }
    }
}
