using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject startMenuCanvas;
    public Button startGameButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button exitButton;
    public Text titleText;
    public Text instructionsText;
    public Text versionText;
    public GameObject loadingPanel;
    public Text loadingText;
    public Image loadingProgressBar;

    [Header("Sub Panels")]
    public GameObject creditsPanel;
    public GameObject aboutPanel;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioSource buttonClickSound;

    [Header("Animation")]
    public Animator titleAnimator;
    public Animator instructionsAnimator;
    public Animator buttonAnimator;

    [Header("Game Info")]
    public string gameVersion = "1.0.0";
    public string buildDate = "2025";

    private ARCompatibilityChecker arChecker;

    void Start()
    {
        InitializeStartMenu();

        // Buscar el ARCompatibilityChecker
        arChecker = FindObjectOfType<ARCompatibilityChecker>();

        // Reproducir música del menú
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // Configurar información de versión
        if (versionText != null)
            versionText.text = $"v{gameVersion} - {buildDate}";
    }

    void InitializeStartMenu()
    {
        // Configurar elementos de UI
        if (titleText != null)
            titleText.text = "AR ALIEN HUNTER";

        if (instructionsText != null)
        {
            instructionsText.text = "¡Bienvenido, Cazador!\n\n" +
                                   "📱 PREPARACIÓN:\n" +
                                   "• Busca un espacio amplio y bien iluminado\n" +
                                   "• Apunta tu cámara hacia el suelo\n" +
                                   "• Mueve lentamente el dispositivo\n\n" +
                                   "🎯 OBJETIVO:\n" +
                                   "• Detecta aliens invasores en tu superficie\n" +
                                   "• Dispara energy balls para eliminarlos\n" +
                                   "• ¡Consigue la máxima puntuación!\n\n" +
                                   "💡 CONSEJO: Encuentra una superficie plana como el suelo, una mesa o alfombra para mejores resultados.\n\n" +
                                   "🚀 Toca INICIAR JUEGO cuando estés listo";
        }

        // Configurar botones
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ShowSettings);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(ShowCredits);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(QuitGame);
        }

        // Ocultar panel de carga inicialmente
        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        // Reproducir música de fondo si está disponible
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    public void StartGame()
    {
        // Verificar compatibilidad AR antes de iniciar
        if (arChecker != null && !arChecker.IsARSupported())
        {
            Debug.LogWarning("AR no soportado en este dispositivo");
            // Mostrar mensaje de advertencia pero permitir continuar
        }

        // Reproducir sonido del botón usando AudioManager
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        else if (buttonClickSound != null)
            buttonClickSound.Play();

        // Mostrar pantalla de carga
        StartCoroutine(LoadGameWithAnimation());
    }

    public void ShowSettings()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();

        GameSettings gameSettings = FindObjectOfType<GameSettings>();
        if (gameSettings != null)
            gameSettings.ShowSettings();
    }

    public void ShowCredits()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            startMenuCanvas.SetActive(false);
        }
    }

    public void ShowAbout()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
            startMenuCanvas.SetActive(false);
        }
    }

    public void HideCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);
    }

    public void HideAbout()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);

        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);
    }

    IEnumerator LoadGameWithAnimation()
    {
        // Activar panel de carga
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // Resetear barra de progreso
        if (loadingProgressBar != null)
            loadingProgressBar.fillAmount = 0f;

        // Animar texto de carga con barra de progreso
        if (loadingText != null)
        {
            loadingText.text = "Verificando dispositivo AR...";
            yield return UpdateProgressBar(0.2f, 1f);

            loadingText.text = "Iniciando cámara AR...";
            yield return UpdateProgressBar(0.4f, 1f);

            loadingText.text = "Preparando detección de planos...";
            yield return UpdateProgressBar(0.6f, 1f);

            loadingText.text = "Cargando modelos 3D...";
            yield return UpdateProgressBar(0.8f, 1f);

            loadingText.text = "¡Listo para cazar aliens!";
            yield return UpdateProgressBar(1f, 1f);

            yield return new WaitForSeconds(0.5f);
        }

        // Ocultar menú principal y activar juego
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        // Activar el GameManager principal
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.StartARGame();
        }
    }

    IEnumerator UpdateProgressBar(float targetProgress, float duration)
    {
        if (loadingProgressBar == null) yield break;

        float startProgress = loadingProgressBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            loadingProgressBar.fillAmount = Mathf.Lerp(startProgress, targetProgress, elapsedTime / duration);
            yield return null;
        }

        loadingProgressBar.fillAmount = targetProgress;
    }

    public void QuitGame()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Método para mostrar el menú principal desde otros scripts
    public void ShowStartMenu()
    {
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    // Método para ocultar el menú principal
    public void HideStartMenu()
    {
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        if (backgroundMusic != null && backgroundMusic.isPlaying)
            backgroundMusic.Stop();
    }
}
