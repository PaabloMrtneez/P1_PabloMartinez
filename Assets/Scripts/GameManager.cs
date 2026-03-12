using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerClass : MonoBehaviour
{
   
    private const int DefaultStartingLives = 6;

    public static GameManagerClass instancia;

    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private UI ui;
    [SerializeField] private int totalvidas = DefaultStartingLives;

    private static bool runStateInitialized = false;
    private static int configuredStartingLives = DefaultStartingLives;
    private static int persistentMonedas = 0;
    private static int persistentLives = DefaultStartingLives;

    private int monedas = 0;
    private int lives;

    public event Action<int> OnLivesChanged;
    public int Lives => lives;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResolveSceneReferences();

        configuredStartingLives = Mathf.Max(0, totalvidas);
        EnsureRunStateInitialized();

        lives = Mathf.Clamp(persistentLives, 0, configuredStartingLives);
        monedas = Mathf.Max(0, persistentMonedas);
    }

    private void Start()
    {
        ResolveSceneReferences();
        RefreshCoinsText();
        NotifyLivesChanged();
    }

    public void AddMoneda()
    {
        monedas++;
        SaveRunState();
        RefreshCoinsText();
    }

    public void LoseLife()
    {
        if (lives <= 0)
            return;

        lives--;
        SaveRunState();
        NotifyLivesChanged();

        if (lives <= 0)
        {
            LoadSceneByName("muerte");
            return;
        }

        RespawnPlayer();
    }

    // Boton "Reintentar" desde muerte.
    public void Reintentar()
    {
        ResetRunAndLoadScene("juego");
    }

    // Boton "Ir a MainMenu" desde muerte.
    public void IrAMain()
    {
        ResetRunAndLoadScene("MainMenu");
    }

    private static void ReiniciarProgresoPartida()
    {
        runStateInitialized = true;
        persistentMonedas = 0;
        persistentLives = Mathf.Max(0, configuredStartingLives);
    }

    private void ResetRunAndLoadScene(string sceneName)
    {
        ReiniciarProgresoPartida();
        LoadSceneByName(sceneName);
    }

    private void NotifyLivesChanged()
    {
        OnLivesChanged?.Invoke(lives);

        if (ui != null)
            ui.ActualizarVidas(lives);
    }

    private void ResolveSceneReferences()
    {
        if (ui == null)
        {
            UI[] uiCandidates = FindObjectsByType<UI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (uiCandidates.Length > 0)
                ui = uiCandidates[0];
        }

        if (textoMonedas == null)
            textoMonedas = FindCoinsText();
    }

    private static TextMeshProUGUI FindCoinsText()
    {
        TextMeshProUGUI[] textCandidates = FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        TextMeshProUGUI fallback = null;

        foreach (TextMeshProUGUI candidate in textCandidates)
        {
            string objectName = candidate.gameObject.name.ToLowerInvariant();
            if (objectName.Contains("texto monedas"))
                return candidate;

            if (fallback == null && objectName.Contains("moneda"))
                fallback = candidate;
        }

        return fallback;
    }

    private void RefreshCoinsText()
    {
        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();
    }

    private void RespawnPlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.Respawn();
    }

    private void SaveRunState()
    {
        runStateInitialized = true;
        persistentMonedas = Mathf.Max(0, monedas);
        persistentLives = Mathf.Clamp(lives, 0, configuredStartingLives);
    }

    private static void EnsureRunStateInitialized()
    {
        if (runStateInitialized)
            return;

        runStateInitialized = true;
        persistentMonedas = 0;
        persistentLives = Mathf.Max(0, configuredStartingLives);
    }

    private void OnDestroy()
    {
        if (instancia == this)
            instancia = null;
    }

    private static void LoadSceneByName(string sceneName)
    {
        if (TryLoadSceneFromBuildSettings(sceneName))
            return;

#if UNITY_EDITOR
        if (TryLoadSceneInEditor(sceneName))
            return;
#endif

        Debug.LogError($"No se pudo cargar la escena: {sceneName}");
    }

    private static bool TryLoadSceneFromBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (!string.Equals(buildSceneName, sceneName, StringComparison.OrdinalIgnoreCase))
                continue;

            SceneManager.LoadScene(i);
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    private static bool TryLoadSceneInEditor(string sceneName)
    {
        string[] sceneGuids = UnityEditor.AssetDatabase.FindAssets($"t:Scene {sceneName}");
        foreach (string guid in sceneGuids)
        {
            string scenePath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            string assetSceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (!string.Equals(assetSceneName, sceneName, StringComparison.OrdinalIgnoreCase))
                continue;

            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                scenePath,
                new LoadSceneParameters(LoadSceneMode.Single)
            );
            return true;
        }

        return false;
    }
#endif
}
