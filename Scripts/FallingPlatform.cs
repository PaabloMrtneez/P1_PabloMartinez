using UnityEngine;

public class FallingPlatformClase : MonoBehaviour
{
   
    [SerializeField] private float velocidad = 2.0f;
    [SerializeField] private GameObject destino;

    private float retardoAntesDeCaer = 0.75f; 
    private float esperaAbajo = 0.75f;        

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private Rigidbody rb;

    // Estado
    private enum Estado { Idle, CuentaAtras, Cayendo, EsperandoAbajo, Volviendo }
    private Estado estado = Estado.Idle;

    private float tiempoInicioCuentaAtras = -1f;
    private float tiempoInicioEsperaAbajo = -1f;

    private void Start()
    {
        posicionInicial = transform.position;

        posicionFinal = destino.transform.position;
           

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        switch (estado)
        {
            case Estado.Idle:
                break;

            case Estado.CuentaAtras:
                if (Time.time - tiempoInicioCuentaAtras >= retardoAntesDeCaer)
                {
                    estado = Estado.Cayendo;
                }
                break;

            case Estado.Cayendo:
                // Mover hacia abajo
                Vector3 nuevaPosicionCaida = Vector3.MoveTowards(
                    transform.position,
                    posicionFinal,
                    velocidad * Time.fixedDeltaTime
                );
                rb.MovePosition(nuevaPosicionCaida);

                if (Vector3.Distance(transform.position, posicionFinal) < 0.01f)
                {
                    estado = Estado.EsperandoAbajo;
                    tiempoInicioEsperaAbajo = Time.time;
                }
                break;

            case Estado.EsperandoAbajo:
                if (Time.time - tiempoInicioEsperaAbajo >= esperaAbajo)
                {
                    estado = Estado.Volviendo;
                }
                break;

            case Estado.Volviendo:
                // Mover hacia arriba
                Vector3 nuevaPosicionVuelta = Vector3.MoveTowards(
                    transform.position,
                    posicionInicial,
                    velocidad * Time.fixedDeltaTime
                );
                rb.MovePosition(nuevaPosicionVuelta);

                if (Vector3.Distance(transform.position, posicionInicial) < 0.01f)
                {
                    // Reiniciar ciclo
                    estado = Estado.Idle;
                    tiempoInicioCuentaAtras = -1f;
                    tiempoInicioEsperaAbajo = -1f;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (estado != Estado.Idle) return;
        
        if (!collision.collider.CompareTag("Player")) return;

        tiempoInicioCuentaAtras = Time.time;
        estado = Estado.CuentaAtras;
    }
}