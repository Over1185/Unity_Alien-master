using UnityEngine;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource buttonClickSound;

    [Header("Button Animation")]
    public Animator buttonAnimator;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    public void PlayButtonSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger("Pressed");
    }

    // Métodos específicos para cada botón del juego
    public void OnStartGamePressed()
    {
        PlayButtonSound();
        StartMenuManager startMenu = FindObjectOfType<StartMenuManager>();
        if (startMenu != null)
            startMenu.StartGame();
    }

    public void OnQuitGamePressed()
    {
        PlayButtonSound();
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.QuitGame();
    }

    public void OnReturnToMenuPressed()
    {
        PlayButtonSound();
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.ReturnToMainMenu();
    }

    public void OnPlayAgainPressed()
    {
        PlayButtonSound();
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.PlayAgain();
    }

    public void OnShowLeaderboardPressed()
    {
        PlayButtonSound();
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.ShowLeaderBoard();
    }

    public void OnRestartGamePressed()
    {
        PlayButtonSound();
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.RestartGame();
    }
}
