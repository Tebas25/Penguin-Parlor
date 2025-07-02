using UnityEngine;
using UnityEngine.AI;

public class PenguinController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // RaycastHit hitpoint;
            if (Physics.Raycast(ray, out RaycastHit hitpoint))
            {
                player.SetDestination(hitpoint.point);
            }
        }
        
    }
}
