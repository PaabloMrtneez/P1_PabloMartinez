using UnityEngine;

public class Coleccionable : MonoBehaviour
{
    private bool fueRecogido;

    private void OnTriggerEnter(Collider other)
    {
        if (fueRecogido)
            return;

        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        fueRecogido = true;

        if (GameManagerClass.instancia != null)
            GameManagerClass.instancia.AddMoneda();

        Destroy(gameObject);
    }
}
