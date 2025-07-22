using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Background Music")]
    public AudioSource menuMusic;
    public AudioSource gameMusic;
    public AudioClip[] backgroundTracks;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip buttonClickClip;
    public AudioClip alienDestroyClip;
    public AudioClip energyBallShootClip;
    public AudioClip gameStartClip;
    public AudioClip gameOverClip;
    public AudioClip victoryClip;

    [Header("UI Controls")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle muteToggle;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeAudioControls();
        PlayMenuMusic();
    }

    void InitializeAudioControls()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            muteToggle.onValueChanged.AddListener(ToggleMute);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        if (audioMixer != null)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        SaveAudioSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        SaveAudioSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        SaveAudioSettings();
    }

    public void ToggleMute(bool isMuted)
    {
        if (audioMixer != null)
        {
            if (isMuted)
                audioMixer.SetFloat("MasterVolume", -80f);
            else
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        }

        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
    }

    public void PlayMenuMusic()
    {
        if (gameMusic != null && gameMusic.isPlaying)
            gameMusic.Stop();

        if (menuMusic != null && !menuMusic.isPlaying)
            menuMusic.Play();
    }

    public void PlayGameMusic()
    {
        if (menuMusic != null && menuMusic.isPlaying)
            menuMusic.Stop();

        if (gameMusic != null && !gameMusic.isPlaying)
            gameMusic.Play();
    }

    public void StopAllMusic()
    {
        if (menuMusic != null) menuMusic.Stop();
        if (gameMusic != null) gameMusic.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayAlienDestroy()
    {
        PlaySFX(alienDestroyClip);
    }

    public void PlayEnergyBallShoot()
    {
        PlaySFX(energyBallShootClip);
    }

    public void PlayGameStart()
    {
        PlaySFX(gameStartClip);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverClip);
    }

    public void PlayVictory()
    {
        PlaySFX(victoryClip);
    }

    void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
}
