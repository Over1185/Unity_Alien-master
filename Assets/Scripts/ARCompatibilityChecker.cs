using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARCompatibilityChecker : MonoBehaviour
{
    [Header("UI References")]
    public GameObject compatibilityWarningPanel;
    public Text warningText;
    public Button continueAnywayButton;
    public Button exitButton;

    [Header("AR Components")]
    public ARSession arSession;

    private bool isARSupported = false;

    void Start()
    {
        StartCoroutine(CheckARCompatibility());
    }

    IEnumerator CheckARCompatibility()
    {
        // Verificar si AR está soportado en el dispositivo
        yield return ARSession.CheckAvailability();

        switch (ARSession.state)
        {
            case ARSessionState.Unsupported:
                ShowIncompatibilityWarning("Tu dispositivo no soporta Realidad Aumentada.\n\nEste juego requiere un dispositivo compatible con ARCore (Android) o ARKit (iOS).");
                isARSupported = false;
                break;

            case ARSessionState.NeedsInstall:
                ShowIncompatibilityWarning("Se requiere instalar ARCore.\n\nPor favor instala ARCore desde Google Play Store para continuar.");
                isARSupported = false;
                break;

            case ARSessionState.Installing:
                ShowLoadingMessage("Instalando ARCore...");
                yield return new WaitForSeconds(2f);
                // Volver a verificar después de la instalación
                StartCoroutine(CheckARCompatibility());
                break;

            case ARSessionState.Ready:
                isARSupported = true;
                HideCompatibilityWarning();
                break;

            case ARSessionState.SessionInitializing:
                ShowLoadingMessage("Inicializando AR...");
                yield return new WaitForSeconds(1f);
                StartCoroutine(CheckARCompatibility());
                break;

            case ARSessionState.SessionTracking:
                isARSupported = true;
                HideCompatibilityWarning();
                break;

            default:
                ShowIncompatibilityWarning("Error desconocido al verificar soporte AR.\n\nPuedes intentar continuar pero la experiencia puede no funcionar correctamente.");
                isARSupported = false;
                break;
        }
    }

    void ShowIncompatibilityWarning(string message)
    {
        if (compatibilityWarningPanel != null)
        {
            compatibilityWarningPanel.SetActive(true);

            if (warningText != null)
                warningText.text = message;

            if (continueAnywayButton != null)
                continueAnywayButton.onClick.AddListener(ContinueAnyway);

            if (exitButton != null)
                exitButton.onClick.AddListener(ExitGame);
        }
    }

    void ShowLoadingMessage(string message)
    {
        if (compatibilityWarningPanel != null)
        {
            compatibilityWarningPanel.SetActive(true);

            if (warningText != null)
                warningText.text = message;

            // Ocultar botones durante la carga
            if (continueAnywayButton != null)
                continueAnywayButton.gameObject.SetActive(false);
            if (exitButton != null)
                exitButton.gameObject.SetActive(false);
        }
    }

    void HideCompatibilityWarning()
    {
        if (compatibilityWarningPanel != null)
            compatibilityWarningPanel.SetActive(false);
    }

    public void ContinueAnyway()
    {
        HideCompatibilityWarning();
        isARSupported = true; // Forzar soporte para permitir continuar
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsARSupported()
    {
        return isARSupported;
    }

    // Método para obtener información detallada del dispositivo
    public string GetDeviceInfo()
    {
        string info = $"Dispositivo: {SystemInfo.deviceModel}\n";
        info += $"SO: {SystemInfo.operatingSystem}\n";
        info += $"RAM: {SystemInfo.systemMemorySize} MB\n";
        info += $"GPU: {SystemInfo.graphicsDeviceName}\n";

        return info;
    }
}
