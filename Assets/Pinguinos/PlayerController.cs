using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float velocidad = 3f;
    public float velocidadRotacion = 720f;
    public Transform camara;

    private Animator animator;
    private Rigidbody rb;
    private Vector3 direccion;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Dirección del movimiento según la cámara
        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        direccion = (adelanteCamara * vertical + derechaCamara * horizontal).normalized;

        if (animator != null)
        {
            animator.SetFloat("speed", direccion.magnitude);
        }

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (direccion != Vector3.zero)
        {
            rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
        }
    }
}