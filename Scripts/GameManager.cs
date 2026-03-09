using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerClass : MonoBehaviour
{
    public static GameManagerClass instancia;

    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private UI ui;
    [SerializeField] private int initialLives = 6;

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

        lives = Mathf.Max(0, initialLives);
    }

    private void Start()
    {
        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();

        if (ui == null)
            ui = FindFirstObjectByType<UI>();

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

        if (ui == null)
            ui = FindFirstObjectByType<UI>();

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
            Debug.Log("Sin vidas");
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("nivel1");
    }

    private void OnDestroy()
    {
        if (instancia == this)
            instancia = null;
    }
}
