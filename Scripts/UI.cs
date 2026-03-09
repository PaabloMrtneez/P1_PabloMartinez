using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image imagenVidas;
    [SerializeField] private Sprite[] spritesPorVida;
    private GameManagerClass gameManager;

    private void OnEnable()
    {
        TryBindGameManager();
    }

    private void Start()
    {
        TryBindGameManager();
    }

    private void Update()
    {
        if (gameManager == null && GameManagerClass.instancia != null)
            TryBindGameManager();
    }

    private void OnDisable()
    {
        if (gameManager != null)
            gameManager.OnLivesChanged -= ActualizarVidas;

        gameManager = null;
    }

    private void TryBindGameManager()
    {
        GameManagerClass instancia = GameManagerClass.instancia;
        if (instancia == null || gameManager == instancia)
            return;

        if (gameManager != null)
            gameManager.OnLivesChanged -= ActualizarVidas;

        gameManager = instancia;
        gameManager.OnLivesChanged += ActualizarVidas;
        ActualizarVidas(gameManager.Lives);
    }

    public void ActualizarVidas(int vidasActuales)
    {
        if (imagenVidas == null || spritesPorVida == null || spritesPorVida.Length == 0)
            return;

        int indice = Mathf.Clamp(vidasActuales, 0, spritesPorVida.Length - 1);
        imagenVidas.sprite = spritesPorVida[indice];
    }
}
