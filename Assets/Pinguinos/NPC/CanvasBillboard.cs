using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.forward = cam.transform.forward;
        }
    }
}
