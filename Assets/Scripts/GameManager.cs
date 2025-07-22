using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // public Game Objects
    [Header("Targets")]
    public GameObject targetPrefab;
    public int targetsNum = 5;
    public int ammo = 7;

    [Header("UI Canvas Objects")]
    public GameObject planeSearchingCanvas;
    public GameObject selectPlaneCanvas;
    public GameObject startButton;
    public GameObject gameUI;
    public Text scoreTxt;
    public GameObject ammoImagePrefab;
    public GameObject ammoImageGrid;
    public GameObject playAgainButton;
    public GameObject leaderBoardButton;
    public GameObject leaderBoardUI;

    [Header("Sounds")]
    public AudioSource EndingSound;
    public AudioSource planeSelectedSound;

    [Header("Scripts")]
    public Leaderboard leaderBoard;

    [Header("Materials")]
    public Material PlaneOcclusionMaterial;

    // private variables
    int totalPoints = 0;
    bool gameStarted = false;

    // private GameObjects
    ARPlane selectedPlane = null;
    ARRaycastManager raycastManager;
    ARPlaneManager planeManager;
    SlingShot slingShot;
    ARSession session;
    // StartMenuManager startMenuManager; // Comentado temporalmente

    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    Dictionary<int, GameObject> targets = new Dictionary<int, GameObject>();

    //Events
    public delegate void PlaneSelectedEventHandler(ARPlane thePlane);
    public event PlaneSelectedEventHandler OnPlaneSelected;

    void Awake()
    {
        session = FindObjectOfType<ARSession>();
        session.Reset();

        // Encontrar el StartMenuManager
        // GameObject startMenuObj = GameObject.Find("StartMenuManager");
        // if (startMenuObj != null)
        //     startMenuManager = startMenuObj.GetComponent<StartMenuManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        slingShot = FindObjectOfType<SlingShot>();

        // Inicialmente deshabilitar componentes AR hasta que se inicie el juego
        if (planeManager != null) planeManager.enabled = false;
        if (raycastManager != null) raycastManager.enabled = false;

        // El juego ahora es controlado por GameFlowManager
        // Solo configuramos los componentes, no iniciamos automáticamente
        Debug.Log("GameManager inicializado. Esperando comando de GameFlowManager para iniciar.");
    }

        // Ocultar todos los canvas del juego inicialmente
        if (planeSearchingCanvas != null) planeSearchingCanvas.SetActive(false);
        if (selectPlaneCanvas != null) selectPlaneCanvas.SetActive(false);
        if (startButton != null) startButton.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
        if (playAgainButton != null) playAgainButton.SetActive(false);
        if (leaderBoardButton != null) leaderBoardButton.SetActive(false);
        if (leaderBoardUI != null) leaderBoardUI.SetActive(false);
    }

// Método llamado por StartMenuManager para iniciar el juego AR
public void StartARGame()
{
    Debug.Log("Iniciando juego AR desde GameFlowManager...");

    // Habilitar componentes AR
    if (planeManager != null)
    {
        planeManager.enabled = true;
        Debug.Log("ARPlaneManager habilitado");
    }

    if (raycastManager != null)
    {
        raycastManager.enabled = true;
        Debug.Log("ARRaycastManager habilitado");
    }

    gameStarted = true;

    // Activar la UI de instrucciones para buscar planos
    if (spawnInstruction != null)
    {
        spawnInstruction.SetActive(true);
    }

    // Configurar audio
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlayGameMusic();
    }

    Debug.Log("Juego AR iniciado correctamente. Busca el suelo para comenzar.");
}

public void ReturnToMainMenu()
{
    Debug.Log("Regresando al menú principal...");

    // Resetear estado del juego
    gameStarted = false;
    selectedPlane = null;
    totalPoints = 0;

    // Deshabilitar componentes AR
    if (planeManager != null) planeManager.enabled = false;
    if (raycastManager != null) raycastManager.enabled = false;

    // Limpiar targets existentes
    ClearAllTargets();

    // Deshabilitar UI del juego
    if (spawnInstruction != null) spawnInstruction.SetActive(false);
    if (planeFoundPanel != null) planeFoundPanel.SetActive(false);
    if (gamePanel != null) gamePanel.SetActive(false);

    // Resetear sesión AR
    if (session != null) session.Reset();

    Debug.Log("Estado del juego reseteado para el menú principal.");
}

void ClearAllTargets()
{
    foreach (var target in targets.Values)
    {
        if (target != null)
            Destroy(target);
    }
    targets.Clear();
}

// Update is called once per frame
void Update()
{
    // Solo procesar input si el juego ha iniciado
    if (!gameStarted) return;

    if (Input.touchCount > 0 && selectedPlane == null && planeManager != null && planeManager.trackables.count > 0)
    {
        SelectPlane();
    }
}
private void SelectPlane()
{
    Touch touch = Input.GetTouch(0);


    if (touch.phase == TouchPhase.Began)
    {
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            selectedPlane = planeManager.GetPlane(hit.trackableId);
            selectedPlane.GetComponent<LineRenderer>().positionCount = 0;

            selectedPlane.GetComponent<Renderer>().material = PlaneOcclusionMaterial;
            // SetMaterialTransparent(selectedPlane);

            foreach (ARPlane plane in planeManager.trackables)
            {
                if (plane != selectedPlane)
                {
                    plane.gameObject.SetActive(false);
                }
            }
            planeManager.enabled = false;
            selectPlaneCanvas.SetActive(false);
            OnPlaneSelected?.Invoke(selectedPlane);
        }
    }
}

void SetMaterialTransparent(ARPlane plane)
{
    foreach (Material material in plane.GetComponent<Renderer>().materials)
    {
        material.SetFloat("_Mode", 2);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        // material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
void PlanesFound(ARPlanesChangedEventArgs args)
{
    if (selectedPlane == null && planeManager.trackables.count > 0)
    {
        planeSearchingCanvas.SetActive(false);
        selectPlaneCanvas.SetActive(true);
        planeManager.planesChanged -= PlanesFound;
    }
}

void PlaneSelected(ARPlane plane)
{
    planeSelectedSound.Play();
    foreach (KeyValuePair<int, GameObject> target in targets)
    {
        Destroy(target.Value);
    }
    targets.Clear();
    // Instanciar los aliens enemigos en el área del portal
    startButton.SetActive(true);
    for (int i = 1; i <= targetsNum; i++)
    {
        // Offset aleatorio alrededor del centro del plano, más separado
        Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
        Vector3 spawnPos = plane.center + offset * 2f; // separa más los aliens
        GameObject target = Instantiate(targetPrefab, spawnPos, plane.transform.rotation, plane.transform);
        target.transform.localScale = Vector3.one * 0.1f; // alien más pequeño
        target.GetComponent<MoveRandomly>().StartMoving(plane);
        target.GetComponent<Target>().ID = i;
        target.GetComponent<Target>().OnTargetDestroy += UpdateGameWhenHitTarget;
        targets.Add(i, target);
    }
}

void UpdateGameWhenHitTarget(int id, int points)
{
    targets.Remove(id);
    totalPoints += points;
    scoreTxt.text = totalPoints.ToString();
    if (targets.Count == 0)
    {
        ShowPlayAgainButton();
    }
}
public void StartGame()
{
    slingShot.AmmoLeft = ammo;
    slingShot.OnReload += SlingShootReload;
    slingShot.Reload();
    totalPoints = 0;
    scoreTxt.text = totalPoints.ToString();
    startButton.SetActive(false);
    gameUI.SetActive(true);



    for (int i = 0; i < slingShot.AmmoLeft; i++)
    {
        GameObject ammoGO = Instantiate(ammoImagePrefab);
        ammoGO.transform.SetParent(ammoImageGrid.transform, false);
    }
}
void SlingShootReload(int ammoLeft)
{
    if (ammoImageGrid.transform.childCount > 0 && ammoLeft >= 0)
    {
        Destroy(ammoImageGrid.transform.GetChild(0).gameObject);
    }
    else if (ammoLeft == 0)
    {
        ShowPlayAgainButton();
        ShowLeaderBoard();
    }
}
public void ShowPlayAgainButton()
{
    EndingSound.Play();
    // gameUI.SetActive(false);
    leaderBoard.SetLeader(totalPoints);
    foreach (Transform ammoImge in ammoImageGrid.transform)
    {
        Destroy(ammoImge.gameObject);
    }
    slingShot.Clear();
    slingShot.OnReload -= SlingShootReload;
    playAgainButton.SetActive(true);
    leaderBoardButton.SetActive(true);
}
public void ShowLeaderBoard()
{
    leaderBoard.PrintLeaderBoard();
    leaderBoardUI.SetActive(true);
}

public void PlayAgain()
{
    leaderBoardButton.SetActive(false);
    PlaneSelected(selectedPlane);
    EndingSound.Stop();
}
public void QuitGame()
{
    Application.Quit();
}

public void ReturnToMainMenu()
{
    // Reiniciar el estado del juego
    gameStarted = false;
    selectedPlane = null;
    totalPoints = 0;

    // Limpiar targets
    foreach (KeyValuePair<int, GameObject> target in targets)
    {
        if (target.Value != null) Destroy(target.Value);
    }
    targets.Clear();

    // Deshabilitar componentes AR
    if (planeManager != null)
    {
        planeManager.enabled = false;
        planeManager.planesChanged -= PlanesFound;
    }
    if (raycastManager != null) raycastManager.enabled = false;

    // Limpiar SlingShot
    if (slingShot != null)
    {
        slingShot.Clear();
        slingShot.OnReload -= SlingShootReload;
    }

    // Ocultar todos los canvas del juego
    if (planeSearchingCanvas != null) planeSearchingCanvas.SetActive(false);
    if (selectPlaneCanvas != null) selectPlaneCanvas.SetActive(false);
    if (startButton != null) startButton.SetActive(false);
    if (gameUI != null) gameUI.SetActive(false);
    if (playAgainButton != null) playAgainButton.SetActive(false);
    if (leaderBoardButton != null) leaderBoardButton.SetActive(false);
    if (leaderBoardUI != null) leaderBoardUI.SetActive(false);

    // Mostrar menú principal
    // if (startMenuManager != null)
    //     startMenuManager.ShowStartMenu();

    // Por ahora, simplemente cargar la escena del menú
    Debug.Log("Regresando al menú principal");

    // Detener sonidos
    if (EndingSound != null && EndingSound.isPlaying)
        EndingSound.Stop();
}

public void RestartGame()
{
    SceneManager.LoadScene("ARSlingshotGame");
}
}
