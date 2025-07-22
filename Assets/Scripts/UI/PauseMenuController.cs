using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    void Start()
    {
        // Configurar botones
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Asegurar que el menú esté oculto al inicio
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ResumeGame()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ResumeGame();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void RestartGame()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.RestartGame();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void ReturnToMainMenu()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ReturnToMainMenu();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }

    public void QuitGame()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.QuitGame();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }
    }
}
