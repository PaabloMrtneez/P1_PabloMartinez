using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float velocidad = 5.0f;
    [SerializeField] private float gradosPorGiro = 90f;
    [SerializeField] private float velocidadGiro = 360f;
    [SerializeField] private float fuerzaSalto = 5.0f;
    [SerializeField] private float multiplicadorSprint = 3.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private float tiempoInvulnerable = 0.5f;

    private Rigidbody rb;
    private bool estaEnSuelo = false;
    private bool haDobleSaltado = false;
    private float entradaMovimiento = 0f;
    private bool teclaSprintPresionada = false;
    private bool saltoEnAire = false;
    private float proximoDamagePermitido = 0f;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int IsSprintingHash = Animator.StringToHash("isSprinting");
    private static readonly int JumpHash = Animator.StringToHash("jump");

    public int coins = 0;

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
    }

    private void Update()
    {
        // Giro
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            AnadirGiro(gradosPorGiro);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            AnadirGiro(-gradosPorGiro);

        // Movimiento
        entradaMovimiento = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            entradaMovimiento = 1f;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            entradaMovimiento = -1f;

        // Sprint
        teclaSprintPresionada = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (animator != null)
        {
            bool estaMoviendose = Mathf.Abs(entradaMovimiento) > 0.1f;
            bool estaSprintfando = estaMoviendose && teclaSprintPresionada && entradaMovimiento > 0.1f;
            animator.SetBool(IsMovingHash, estaMoviendose);
            animator.SetBool(IsSprintingHash, estaSprintfando);
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (estaEnSuelo)
            {
                EjecutarSalto();
                haDobleSaltado = false;
            }
            else if (!haDobleSaltado)
            {
                EjecutarSalto();
                haDobleSaltado = true;
            }
        }
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

    private void EjecutarSalto()
    {
        if (animator != null)
            animator.SetTrigger(JumpHash);

        saltoEnAire = true;

        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

    public void ReceiveDamage()
    {
        if (Time.time < proximoDamagePermitido) return;

        proximoDamagePermitido = Time.time + tiempoInvulnerable;

        if (GameManagerClass.instancia != null)
            GameManagerClass.instancia.LoseLife();
    }

    private void OnCollisionEnter(Collision colision)
    {
        if (colision.gameObject.CompareTag("Ground"))
        {
            estaEnSuelo = true;
            haDobleSaltado = false;
            saltoEnAire = false;
        }

    }

    private void OnCollisionStay(Collision colision)
    {
        if (colision.gameObject.CompareTag("Ground"))
            estaEnSuelo = true;
    }

    private void OnCollisionExit(Collision colision)
    {
        if (colision.gameObject.CompareTag("Ground"))
        {
            estaEnSuelo = false;
            if (!saltoEnAire && animator != null)
                animator.SetTrigger(JumpHash);
        }
    }
}
