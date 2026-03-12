using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string GroundTag = "Ground";

    [SerializeField] private float velocidad = 5.0f;
    [SerializeField] private float gradosPorGiro = 90f;
    [SerializeField] private float velocidadGiro = 360f;
    [SerializeField] private float fuerzaSalto = 5.0f;
    [SerializeField] private float multiplicadorSprint = 3.0f;
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private bool estaEnSuelo;
    private bool haDobleSaltado;
    private float entradaMovimiento;
    private bool teclaSprintPresionada;
    private bool saltoEnAire;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int IsSprintingHash = Animator.StringToHash("isSprinting");
    private static readonly int JumpHash = Animator.StringToHash("jump");

    // Rotacion
    private Quaternion rotacionObjetivo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rotacionObjetivo = rb.rotation;
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    private void Update()
    {
        ProcesarGiro();
        ProcesarMovimiento();
        ActualizarAnimator();
        ProcesarSalto();
    }

    private void FixedUpdate()
    {
        bool estaGirando = Quaternion.Angle(rb.rotation, rotacionObjetivo) > 0.1f;

        Quaternion nuevaRotacion = Quaternion.RotateTowards(
            rb.rotation,
            rotacionObjetivo,
            velocidadGiro * Time.fixedDeltaTime
        );
        rb.MoveRotation(nuevaRotacion);

        float movimientoFinal = estaGirando ? 0f : entradaMovimiento;
        bool estaCorriendo = teclaSprintPresionada && movimientoFinal > 0.1f;
        float velocidadActual = velocidad * (estaCorriendo ? multiplicadorSprint : 1f);
        Vector3 desplazamiento = transform.forward * (movimientoFinal * velocidadActual * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + desplazamiento);
    }

    private void AnadirGiro(float grados)
    {
        Quaternion paso = Quaternion.Euler(0f, grados, 0f);
        rotacionObjetivo = rotacionObjetivo * paso;
    }

    private void ProcesarGiro()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            AnadirGiro(gradosPorGiro);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            AnadirGiro(-gradosPorGiro);
    }

    private void ProcesarMovimiento()
    {
        entradaMovimiento = 0f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            entradaMovimiento = 1f;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            entradaMovimiento = -1f;

        teclaSprintPresionada = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void ActualizarAnimator()
    {
        if (animator == null)
            return;

        bool estaMoviendose = Mathf.Abs(entradaMovimiento) > 0.1f;
        animator.SetBool(IsMovingHash, estaMoviendose);
        animator.SetBool(
            IsSprintingHash,
            estaMoviendose && teclaSprintPresionada && entradaMovimiento > 0.1f
        );
    }

    private void ProcesarSalto()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!estaEnSuelo && haDobleSaltado)
            return;

        EjecutarSalto();
        haDobleSaltado = !estaEnSuelo;
    }

    private void EjecutarSalto()
    {
        if (animator != null)
            animator.SetTrigger(JumpHash);

        saltoEnAire = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

    public void ReceiveDamage()
    {
        if (GameManagerClass.instancia != null)
            GameManagerClass.instancia.LoseLife();
    }

    public void SetRespawnPoint(Vector3 nuevaPosicion, Quaternion nuevaRotacion)
    {
        respawnPosition = nuevaPosicion;
        respawnRotation = nuevaRotacion;
    }

    public void Respawn()
    {
        transform.SetPositionAndRotation(respawnPosition, respawnRotation);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rotacionObjetivo = respawnRotation;
        entradaMovimiento = 0f;
        teclaSprintPresionada = false;
        estaEnSuelo = false;
        saltoEnAire = false;
        haDobleSaltado = false;

        if (animator != null)
        {
            animator.SetBool(IsMovingHash, false);
            animator.SetBool(IsSprintingHash, false);
        }
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (EsSuelo(colision))
        {
            estaEnSuelo = true;
            haDobleSaltado = false;
            saltoEnAire = false;
        }
    }

    private void OnCollisionStay(Collision colision)
    {
        if (EsSuelo(colision))
            estaEnSuelo = true;
    }

    private void OnCollisionExit(Collision colision)
    {
        if (EsSuelo(colision))
        {
            estaEnSuelo = false;
            if (!saltoEnAire && animator != null)
                animator.SetTrigger(JumpHash);
        }
    }

    private static bool EsSuelo(Collision colision)
    {
        return colision.gameObject.CompareTag(GroundTag);
    }
}
