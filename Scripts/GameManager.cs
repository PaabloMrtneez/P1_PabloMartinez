using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerClass : MonoBehaviour
{
    private const string MainMenuSceneName = "Interfaz";
    private const string MainMenuScenePath = "Assets/Scenes/Interfaz.unity";

    public static GameManagerClass instancia;

    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private UI ui;
    [SerializeField] private int totalvidas = 6;

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

        lives = Mathf.Max(0, totalvidas);
    }

    private void Start()
    {
        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();

        NotifyLivesChanged();
    }

    public void AddMoneda()
    {
        monedas++;

        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();
    }

    private void NotifyLivesChanged()
    {
        OnLivesChanged?.Invoke(lives);

        if (ui != null)
            ui.ActualizarVidas(lives);
    }

    public void LoseLife()
    {
        if (lives <= 0)
            return;

        lives--;
        NotifyLivesChanged();

        if (lives <= 0)
        {
            ReturnToMainMenu();
            return;
        }

        RespawnPlayer();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("nivel1");
    }

    private void RespawnPlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.Respawn();
    }

    private void ReturnToMainMenu()
    {
        monedas = 0;
        lives = Mathf.Max(0, totalvidas);

        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();

        if (TryLoadSceneFromBuildSettings(MainMenuSceneName))
            return;

#if UNITY_EDITOR
        if (TryLoadSceneInEditor(MainMenuScenePath))
            return;
#endif

        Debug.LogError($"No se pudo cargar la escena de menu: {MainMenuSceneName}");
    }

    private void OnDestroy()
    {
        if (instancia == this)
            instancia = null;
    }

    private static bool TryLoadSceneFromBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!string.Equals(buildSceneName, sceneName, StringComparison.OrdinalIgnoreCase))
                continue;

            SceneManager.LoadScene(i);
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    private static bool TryLoadSceneInEditor(string scenePath)
    {
        UnityEngine.Object sceneAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(scenePath);
        if (sceneAsset == null)
            return false;

        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
            scenePath,
            new LoadSceneParameters(LoadSceneMode.Single)
        );
        return true;
    }
#endif
}
