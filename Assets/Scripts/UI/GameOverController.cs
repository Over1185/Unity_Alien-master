using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    [Header("UI Elements")]
    public Text finalScoreText;
    public Text highScoreText;
    public Text gameOverTitleText;
    public Button playAgainButton;
    public Button mainMenuButton;
    public Button shareScoreButton;

    [Header("Animation")]
    public Animator gameOverAnimator;
    public float delayBeforeShow = 1f;

    [Header("Audio")]
    public AudioClip gameOverSound;
    public AudioClip newHighScoreSound;

    private int finalScore = 0;
    private bool isNewHighScore = false;

    void Start()
    {
        // Configurar botones
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (shareScoreButton != null)
            shareScoreButton.onClick.AddListener(ShareScore);

        // Ocultar al inicio
        gameObject.SetActive(false);
    }

    public void ShowGameOver(int score)
    {
        StartCoroutine(ShowGameOverCoroutine(score));
    }

    IEnumerator ShowGameOverCoroutine(int score)
    {
        finalScore = score;

        // Esperar antes de mostrar
        yield return new WaitForSeconds(delayBeforeShow);

        // Activar el panel
        gameObject.SetActive(true);

        // Verificar si es nuevo high score
        int previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        isNewHighScore = score > previousHighScore;

        if (isNewHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        // Actualizar textos
        UpdateScoreTexts();

        // Reproducir sonido apropiado
        PlayGameOverSound();

        // Animación si existe
        if (gameOverAnimator != null)
        {
            gameOverAnimator.SetTrigger("ShowGameOver");
        }
    }

    void UpdateScoreTexts()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Puntuación Final: {finalScore}";
        }

        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"Mejor Puntuación: {highScore}";

            if (isNewHighScore)
            {
                highScoreText.color = Color.yellow;
                highScoreText.text += " ¡NUEVO RÉCORD!";
            }
        }

        if (gameOverTitleText != null)
        {
            if (isNewHighScore)
            {
                gameOverTitleText.text = "¡NUEVO RÉCORD!";
                gameOverTitleText.color = Color.yellow;
            }
            else
            {
                gameOverTitleText.text = "Juego Terminado";
                gameOverTitleText.color = Color.white;
            }
        }
    }

    void PlayGameOverSound()
    {
        if (AudioManager.Instance != null)
        {
            if (isNewHighScore && newHighScoreSound != null)
            {
                AudioManager.Instance.PlaySFX(newHighScoreSound);
            }
            else if (gameOverSound != null)
            {
                AudioManager.Instance.PlaySFX(gameOverSound);
            }
        }
    }

    public void PlayAgain()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.RestartGame();
        }

        gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ReturnToMainMenu();
        }

        gameObject.SetActive(false);
    }

    public void ShareScore()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        // Implementar funcionalidad de compartir (redes sociales, etc.)
        string shareText = $"¡Logré {finalScore} puntos en Alien Hunter AR!";

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);
            
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
#elif UNITY_IOS && !UNITY_EDITOR
            // iOS sharing implementation
            Debug.Log("iOS sharing: " + shareText);
#else
        // Fallback para editor
        GUIUtility.systemCopyBuffer = shareText;
        Debug.Log("Puntuación copiada al portapapeles: " + shareText);
#endif
    }
}
