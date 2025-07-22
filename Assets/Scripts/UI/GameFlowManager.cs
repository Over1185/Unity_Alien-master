using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "ARSlingshotGame";

    [Header("UI References")]
    public GameObject startMenuCanvas;
    public GameObject gameplayUI;
    public GameObject pauseMenuCanvas;
    public GameObject gameOverCanvas;

    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isGameOver = false;

    public static GameFlowManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver,
        Settings
    }

    [Header("Current State")]
    public GameState currentState = GameState.MainMenu;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Configurar estado inicial
        SetGameState(GameState.MainMenu);
    }

    void Update()
    {
        // Manejar input del botón de pausa/back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    public void SetGameState(GameState newState)
    {
        GameState previousState = currentState;
        currentState = newState;

        // Lógica de transición entre estados
        switch (newState)
        {
            case GameState.MainMenu:
                ShowMainMenu();
                break;

            case GameState.Loading:
                ShowLoadingScreen();
                break;

            case GameState.Playing:
                StartGameplay();
                break;

            case GameState.Paused:
                PauseGame();
                break;

            case GameState.GameOver:
                EndGame();
                break;

            case GameState.Settings:
                ShowSettings();
                break;
        }

        Debug.Log($"Game State cambió de {previousState} a {newState}");
    }

    void HandleBackButton()
    {
        switch (currentState)
        {
            case GameState.Playing:
                SetGameState(GameState.Paused);
                break;

            case GameState.Paused:
                SetGameState(GameState.Playing);
                break;

            case GameState.Settings:
                SetGameState(GameState.MainMenu);
                break;

            case GameState.GameOver:
                SetGameState(GameState.MainMenu);
                break;

            case GameState.MainMenu:
                Application.Quit();
                break;
        }
    }

    void ShowMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        isGameOver = false;

        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        // Reproducir música del menú
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    void ShowLoadingScreen()
    {
        // La lógica de loading se maneja en StartMenuManager
        Debug.Log("Mostrando pantalla de carga...");
    }

    void StartGameplay()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        isGameOver = false;

        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        // Reproducir música del juego
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameMusic();

        // Activar GameManager si existe
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.StartARGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);
    }

    void EndGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        isGameOver = true;

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    void ShowSettings()
    {
        // La lógica de settings se maneja en GameSettings
        Debug.Log("Mostrando configuraciones...");
    }

    // Métodos públicos para ser llamados desde UI
    public void StartNewGame()
    {
        SetGameState(GameState.Loading);
        StartCoroutine(LoadGameAsync());
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }

    public void RestartGame()
    {
        SetGameState(GameState.Loading);
        StartCoroutine(RestartGameAsync());
    }

    public void ReturnToMainMenu()
    {
        SetGameState(GameState.MainMenu);

        // Limpiar estado del juego
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ReturnToMainMenu();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator LoadGameAsync()
    {
        // Simular carga del juego
        yield return new WaitForSeconds(0.5f);
        SetGameState(GameState.Playing);
    }

    IEnumerator RestartGameAsync()
    {
        // Limpiar estado actual
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ReturnToMainMenu();
        }

        yield return new WaitForSeconds(0.2f);
        SetGameState(GameState.Playing);
    }

    // Getters para otros scripts
    public bool IsGameActive()
    {
        return currentState == GameState.Playing;
    }

    public bool IsGamePaused()
    {
        return currentState == GameState.Paused;
    }

    public bool IsInMainMenu()
    {
        return currentState == GameState.MainMenu;
    }
}
