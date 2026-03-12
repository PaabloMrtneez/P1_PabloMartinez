using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string levelSceneName = "juego";

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelSceneName);
    }
}
