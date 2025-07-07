using UnityEngine;
using UnityEngine.AI;

public class PenguinController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent player;
    public LayerMask ignoreClickLayers;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Si se hizo clic sobre un objeto que est� en las capas ignoradas, no mover
            if (Physics.Raycast(ray, out RaycastHit hitIgnore, 100f, ignoreClickLayers))
            {
                return;
            }

            // Solo si no se hizo clic sobre ping�inos u objetos bloqueados
            if (Physics.Raycast(ray, out RaycastHit hitpoint))
            {
                player.SetDestination(hitpoint.point);
            }
        }

        // Cambio animaci�n por la velocidad del agente
        float speed = player.velocity.magnitude;
        if (animator != null)
        {
            animator.SetFloat("speed", speed);
        }
    }
}