using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MainMenuPlayModeStart
{
    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

    static MainMenuPlayModeStart()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        AssignMainMenuAsPlayModeStartScene();
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            AssignMainMenuAsPlayModeStartScene();
    }

    private static void AssignMainMenuAsPlayModeStartScene()
    {
        SceneAsset mainMenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath);
        if (mainMenuScene == null)
        {
            Debug.LogWarning($"No se encontro la escena principal en: {MainMenuScenePath}");
            return;
        }

        if (EditorSceneManager.playModeStartScene == mainMenuScene)
            return;

        EditorSceneManager.playModeStartScene = mainMenuScene;
    }
}
