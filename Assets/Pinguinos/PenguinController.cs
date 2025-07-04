using UnityEngine;
using UnityEngine.AI;

public class PenguinController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent player;
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
            if (Physics.Raycast(ray, out RaycastHit hitpoint))
            {
                player.SetDestination(hitpoint.point);
            }
        }

        // Cambio animación por la velocidad del agente
        float speed = player.velocity.magnitude;
        if (animator != null)
        {
            animator.SetFloat("speed", speed);
        }
    }
}