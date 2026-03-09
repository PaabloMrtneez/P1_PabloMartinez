using UnityEngine;

public class Coleccionable : MonoBehaviour
{
    private bool fueRecogido = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController jugador = other.GetComponentInParent<PlayerController>();
        if (jugador != null)
        {
            fueRecogido = true;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (fueRecogido && GameManagerClass.instancia != null)
            GameManagerClass.instancia.AddMoneda();
    }
}
