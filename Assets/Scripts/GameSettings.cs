using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

    [Header("Graphics Settings")]
    public Dropdown qualityDropdown;
    public Toggle vsyncToggle;
    public Slider brightnessSlider;

    [Header("Gameplay Settings")]
    public Slider sensitivitySlider;
    public Toggle vibrationToggle;
    public Dropdown difficultyDropdown;

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("AR Settings")]
    public Toggle planeVisualizationToggle;
    public Slider trackingQualitySlider;

    [Header("Default Values")]
    public float defaultSensitivity = 1.0f;
    public float defaultMasterVolume = 0.8f;
    public float defaultMusicVolume = 0.7f;
    public float defaultSFXVolume = 0.8f;
    public float defaultBrightness = 1.0f;

    private const string PREFS_SENSITIVITY = "GameSensitivity";
    private const string PREFS_MASTER_VOLUME = "MasterVolume";
    private const string PREFS_MUSIC_VOLUME = "MusicVolume";
    private const string PREFS_SFX_VOLUME = "SFXVolume";
    private const string PREFS_QUALITY = "GraphicsQuality";
    private const string PREFS_VSYNC = "VSync";
    private const string PREFS_VIBRATION = "Vibration";
    private const string PREFS_DIFFICULTY = "Difficulty";
    private const string PREFS_BRIGHTNESS = "Brightness";
    private const string PREFS_PLANE_VIS = "PlaneVisualization";
    private const string PREFS_TRACKING_QUALITY = "TrackingQuality";

    void Start()
    {
        LoadSettings();
        InitializeUI();
    }

    void InitializeUI()
    {
        // Configurar dropdowns
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Bajo", "Medio", "Alto", "Ultra"
            });
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Fácil", "Normal", "Difícil", "Extremo"
            });
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        // Configurar sliders
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        if (trackingQualitySlider != null)
            trackingQualitySlider.onValueChanged.AddListener(OnTrackingQualityChanged);

        // Configurar toggles
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);

        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);

        if (planeVisualizationToggle != null)
            planeVisualizationToggle.onValueChanged.AddListener(OnPlaneVisualizationChanged);
    }

    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        LoadSettings(); // Refrescar valores actuales
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        SaveSettings();
    }

    public void LoadSettings()
    {
        // Cargar configuraciones guardadas
        float sensitivity = PlayerPrefs.GetFloat(PREFS_SENSITIVITY, defaultSensitivity);
        float masterVolume = PlayerPrefs.GetFloat(PREFS_MASTER_VOLUME, defaultMasterVolume);
        float musicVolume = PlayerPrefs.GetFloat(PREFS_MUSIC_VOLUME, defaultMusicVolume);
        float sfxVolume = PlayerPrefs.GetFloat(PREFS_SFX_VOLUME, defaultSFXVolume);
        float brightness = PlayerPrefs.GetFloat(PREFS_BRIGHTNESS, defaultBrightness);
        float trackingQuality = PlayerPrefs.GetFloat(PREFS_TRACKING_QUALITY, 1.0f);

        int quality = PlayerPrefs.GetInt(PREFS_QUALITY, 2);
        int difficulty = PlayerPrefs.GetInt(PREFS_DIFFICULTY, 1);
        bool vsync = PlayerPrefs.GetInt(PREFS_VSYNC, 1) == 1;
        bool vibration = PlayerPrefs.GetInt(PREFS_VIBRATION, 1) == 1;
        bool planeVis = PlayerPrefs.GetInt(PREFS_PLANE_VIS, 1) == 1;

        // Aplicar a UI
        if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (brightnessSlider != null) brightnessSlider.value = brightness;
        if (trackingQualitySlider != null) trackingQualitySlider.value = trackingQuality;

        if (qualityDropdown != null) qualityDropdown.value = quality;
        if (difficultyDropdown != null) difficultyDropdown.value = difficulty;
        if (vsyncToggle != null) vsyncToggle.isOn = vsync;
        if (vibrationToggle != null) vibrationToggle.isOn = vibration;
        if (planeVisualizationToggle != null) planeVisualizationToggle.isOn = planeVis;

        // Aplicar configuraciones al sistema
        ApplySettings();
    }

    public void SaveSettings()
    {
        // Guardar todas las configuraciones
        if (sensitivitySlider != null)
            PlayerPrefs.SetFloat(PREFS_SENSITIVITY, sensitivitySlider.value);

        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat(PREFS_MASTER_VOLUME, masterVolumeSlider.value);

        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat(PREFS_MUSIC_VOLUME, musicVolumeSlider.value);

        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat(PREFS_SFX_VOLUME, sfxVolumeSlider.value);

        if (brightnessSlider != null)
            PlayerPrefs.SetFloat(PREFS_BRIGHTNESS, brightnessSlider.value);

        if (trackingQualitySlider != null)
            PlayerPrefs.SetFloat(PREFS_TRACKING_QUALITY, trackingQualitySlider.value);

        if (qualityDropdown != null)
            PlayerPrefs.SetInt(PREFS_QUALITY, qualityDropdown.value);

        if (difficultyDropdown != null)
            PlayerPrefs.SetInt(PREFS_DIFFICULTY, difficultyDropdown.value);

        if (vsyncToggle != null)
            PlayerPrefs.SetInt(PREFS_VSYNC, vsyncToggle.isOn ? 1 : 0);

        if (vibrationToggle != null)
            PlayerPrefs.SetInt(PREFS_VIBRATION, vibrationToggle.isOn ? 1 : 0);

        if (planeVisualizationToggle != null)
            PlayerPrefs.SetInt(PREFS_PLANE_VIS, planeVisualizationToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        if (sensitivitySlider != null) sensitivitySlider.value = defaultSensitivity;
        if (masterVolumeSlider != null) masterVolumeSlider.value = defaultMasterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = defaultMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = defaultSFXVolume;
        if (brightnessSlider != null) brightnessSlider.value = defaultBrightness;
        if (trackingQualitySlider != null) trackingQualitySlider.value = 1.0f;

        if (qualityDropdown != null) qualityDropdown.value = 2;
        if (difficultyDropdown != null) difficultyDropdown.value = 1;
        if (vsyncToggle != null) vsyncToggle.isOn = true;
        if (vibrationToggle != null) vibrationToggle.isOn = true;
        if (planeVisualizationToggle != null) planeVisualizationToggle.isOn = true;

        ApplySettings();
    }

    void ApplySettings()
    {
        // Aplicar configuración de calidad gráfica
        if (qualityDropdown != null)
            QualitySettings.SetQualityLevel(qualityDropdown.value);

        // Aplicar VSync
        if (vsyncToggle != null)
            QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;

        // Aplicar configuraciones de audio
        if (AudioManager.Instance != null)
        {
            if (masterVolumeSlider != null)
                AudioManager.Instance.SetMasterVolume(masterVolumeSlider.value);

            if (musicVolumeSlider != null)
                AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);

            if (sfxVolumeSlider != null)
                AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
        }
    }

    // Event handlers
    void OnSensitivityChanged(float value) { ApplySettings(); }
    void OnMasterVolumeChanged(float value) { ApplySettings(); }
    void OnMusicVolumeChanged(float value) { ApplySettings(); }
    void OnSFXVolumeChanged(float value) { ApplySettings(); }
    void OnBrightnessChanged(float value) { ApplySettings(); }
    void OnTrackingQualityChanged(float value) { ApplySettings(); }
    void OnQualityChanged(int value) { ApplySettings(); }
    void OnDifficultyChanged(int value) { ApplySettings(); }
    void OnVSyncChanged(bool value) { ApplySettings(); }
    void OnVibrationChanged(bool value) { ApplySettings(); }
    void OnPlaneVisualizationChanged(bool value) { ApplySettings(); }

    // Getters para usar en otros scripts
    public float GetSensitivity() => sensitivitySlider != null ? sensitivitySlider.value : defaultSensitivity;
    public int GetDifficulty() => difficultyDropdown != null ? difficultyDropdown.value : 1;
    public bool GetVibrationEnabled() => vibrationToggle != null ? vibrationToggle.isOn : true;
    public bool GetPlaneVisualizationEnabled() => planeVisualizationToggle != null ? planeVisualizationToggle.isOn : true;
    public float GetTrackingQuality() => trackingQualitySlider != null ? trackingQualitySlider.value : 1.0f;
}
