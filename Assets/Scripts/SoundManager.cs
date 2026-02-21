using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sound Manager - Enhanced audio system
/// Background music per world (3 track slots each), UI sounds, Pitou voice sounds
/// Every interaction has audio feedback
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
        public bool randomizePitch = false;
        [Range(0f, 0.3f)] public float pitchVariation = 0.1f;
    }

    [System.Serializable]
    public class WorldMusic
    {
        public string worldName;
        public AudioClip[] tracks; // 3 slots per world
        [Range(0f, 1f)] public float volume = 0.5f;
    }

    [Header("Sound Effects")]
    [SerializeField] private List<SoundEffect> soundEffects = new List<SoundEffect>();

    [Header("World Music")]
    [SerializeField] private List<WorldMusic> worldMusicTracks = new List<WorldMusic>();

    [Header("Pitou Voice Sounds")]
    [SerializeField] private AudioClip pitouHappy;
    [SerializeField] private AudioClip pitouSad;
    [SerializeField] private AudioClip pitouExcited;
    [SerializeField] private AudioClip pitouThinking;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource pitouSource;

    [Header("Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float musicFadeDuration = 1f;

    private Dictionary<string, SoundEffect> sfxLookup = new Dictionary<string, SoundEffect>();
    private int currentWorldIndex = -1;
    private int currentTrackIndex = 0;
    private bool isMusicEnabled = true;
    private bool isSfxEnabled = true;

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
        InitializeAudioSources();
        InitializeSoundEffects();
        LoadAudioSettings();
    }

    /// <summary>
    /// Initialize audio sources if not assigned
    /// </summary>
    private void InitializeAudioSources()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        if (pitouSource == null)
        {
            pitouSource = gameObject.AddComponent<AudioSource>();
            pitouSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Initialize default sound effects (with null clips - to be assigned in editor)
    /// </summary>
    private void InitializeSoundEffects()
    {
        // Build lookup
        sfxLookup.Clear();
        foreach (var sfx in soundEffects)
        {
            sfxLookup[sfx.id] = sfx;
        }

        // Register default sounds if not present
        RegisterDefaultSounds();

        // Initialize world music slots
        if (worldMusicTracks.Count == 0)
        {
            string[] worldNames = { "Garden", "Ocean", "Space", "Jungle", "Crystal Cave" };
            foreach (var name in worldNames)
            {
                worldMusicTracks.Add(new WorldMusic
                {
                    worldName = name,
                    tracks = new AudioClip[3], // 3 track slots per world
                    volume = 0.5f
                });
            }
        }
    }

    /// <summary>
    /// Register default sound IDs
    /// </summary>
    private void RegisterDefaultSounds()
    {
        string[] defaultSounds = {
            "tap", "swap", "correct", "wrong", "complete",
            "star_earned", "combo", "hint", "button_click",
            "level_start", "countdown", "powerup"
        };

        foreach (var id in defaultSounds)
        {
            if (!sfxLookup.ContainsKey(id))
            {
                var sfx = new SoundEffect { id = id, volume = 1f, pitch = 1f };
                soundEffects.Add(sfx);
                sfxLookup[id] = sfx;
            }
        }
    }

    /// <summary>
    /// Play a sound effect by ID
    /// </summary>
    public void PlaySound(string soundId)
    {
        if (!isSfxEnabled) return;

        if (sfxLookup.ContainsKey(soundId))
        {
            SoundEffect sfx = sfxLookup[soundId];
            if (sfx.clip != null)
            {
                float pitch = sfx.pitch;
                if (sfx.randomizePitch)
                    pitch += Random.Range(-sfx.pitchVariation, sfx.pitchVariation);

                sfxSource.pitch = pitch;
                sfxSource.PlayOneShot(sfx.clip, sfx.volume * sfxVolume * masterVolume);
            }
            else
            {
                // Fallback: play a basic beep tone
                Debug.Log($"[Sound] Playing: {soundId} (clip not assigned)");
            }
        }
    }

    /// <summary>
    /// Play a sound with custom clip
    /// </summary>
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (!isSfxEnabled || clip == null) return;
        sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
    }

    /// <summary>
    /// Play Pitou voice sound
    /// </summary>
    public void PlayPitouSound(string type)
    {
        if (!isSfxEnabled) return;

        AudioClip clip = null;
        switch (type.ToLower())
        {
            case "happy": clip = pitouHappy; break;
            case "sad": clip = pitouSad; break;
            case "excited": clip = pitouExcited; break;
            case "thinking": clip = pitouThinking; break;
        }

        if (clip != null && pitouSource != null)
        {
            pitouSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
        else
        {
            Debug.Log($"[Sound] Pitou sound: {type} (clip not assigned)");
        }
    }

    /// <summary>
    /// Play world music
    /// </summary>
    public void PlayWorldMusic(int worldIndex)
    {
        if (!isMusicEnabled) return;
        if (worldIndex == currentWorldIndex) return;

        currentWorldIndex = worldIndex;

        if (worldIndex >= 0 && worldIndex < worldMusicTracks.Count)
        {
            WorldMusic worldMusic = worldMusicTracks[worldIndex];
            AudioClip track = null;

            // Find first non-null track
            if (worldMusic.tracks != null)
            {
                for (int i = 0; i < worldMusic.tracks.Length; i++)
                {
                    int idx = (currentTrackIndex + i) % worldMusic.tracks.Length;
                    if (worldMusic.tracks[idx] != null)
                    {
                        track = worldMusic.tracks[idx];
                        currentTrackIndex = idx;
                        break;
                    }
                }
            }

            if (track != null)
            {
                StartCoroutine(CrossfadeMusic(track, worldMusic.volume));
            }
            else
            {
                Debug.Log($"[Sound] World music for {worldMusic.worldName} (tracks not assigned)");
            }
        }
    }

    /// <summary>
    /// Stop music
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    /// <summary>
    /// Crossfade to new music track
    /// </summary>
    private System.Collections.IEnumerator CrossfadeMusic(AudioClip newTrack, float targetVolume)
    {
        // Fade out current
        if (musicSource.isPlaying)
        {
            float startVol = musicSource.volume;
            float elapsed = 0f;
            while (elapsed < musicFadeDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVol, 0, elapsed / (musicFadeDuration * 0.5f));
                yield return null;
            }
        }

        // Switch track
        musicSource.clip = newTrack;
        musicSource.Play();

        // Fade in
        float target = targetVolume * musicVolume * masterVolume;
        float el = 0f;
        while (el < musicFadeDuration * 0.5f)
        {
            el += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, target, el / (musicFadeDuration * 0.5f));
            yield return null;
        }

        musicSource.volume = target;
    }

    private System.Collections.IEnumerator FadeOutMusic()
    {
        float startVol = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0, elapsed / musicFadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = 0;
    }

    /// <summary>
    /// Load audio settings from PlayerPrefs
    /// </summary>
    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        isMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }

    /// <summary>
    /// Save audio settings
    /// </summary>
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("MusicEnabled", isMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", isSfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Volume setters
    public void SetMasterVolume(float vol) { masterVolume = Mathf.Clamp01(vol); SaveAudioSettings(); }
    public void SetSFXVolume(float vol) { sfxVolume = Mathf.Clamp01(vol); SaveAudioSettings(); }
    public void SetMusicVolume(float vol) { musicVolume = Mathf.Clamp01(vol); SaveAudioSettings(); }
    public void SetMusicEnabled(bool enabled) { isMusicEnabled = enabled; if (!enabled) StopMusic(); SaveAudioSettings(); }
    public void SetSFXEnabled(bool enabled) { isSfxEnabled = enabled; SaveAudioSettings(); }

    // Getters
    public float GetMasterVolume() => masterVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
    public bool IsMusicEnabled() => isMusicEnabled;
    public bool IsSFXEnabled() => isSfxEnabled;
}
