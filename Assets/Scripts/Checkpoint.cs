using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform puntoRespawn;
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Color colorActivado = Color.green;
    [SerializeField] private bool desactivarTrasActivarse = true;

    private Collider checkpointCollider;
    private bool activado;

    private void Awake()
    {
        checkpointCollider = GetComponent<Collider>();
        checkpointCollider.isTrigger = true;

        if (visualRenderer == null)
            visualRenderer = GetComponentInChildren<Renderer>();
    }

    private void OnValidate()
    {
        Collider targetCollider = GetComponent<Collider>();
        if (targetCollider != null)
            targetCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activado)
            return;

        PlayerController jugador = other.GetComponentInParent<PlayerController>();
        if (jugador == null)
            return;

        Transform destinoRespawn = puntoRespawn != null ? puntoRespawn : transform;
        jugador.SetRespawnPoint(destinoRespawn.position, destinoRespawn.rotation);

        ActivarCheckpoint();
    }

    private void ActivarCheckpoint()
    {
        activado = true;

        if (visualRenderer != null && visualRenderer.material.HasProperty("_Color"))
            visualRenderer.material.color = colorActivado;

        if (!desactivarTrasActivarse)
            return;

        if (checkpointCollider != null)
            checkpointCollider.enabled = false;

        enabled = false;
    }
}
