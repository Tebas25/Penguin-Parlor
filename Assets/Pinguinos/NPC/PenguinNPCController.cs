using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PenguinNPCController : MonoBehaviour
{
    public GameObject selectorCircle;
    private NavMeshAgent agent;
    private Animator animator;

    private Transform targetSitPoint;
    private SitPointMarker sitPointMarker;
    private bool isSitting = false;
    public float snapDistance = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        SetSelected(false);
    }

    void Update()
    {
        if (isSitting || targetSitPoint == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);

        float dist = Vector3.Distance(transform.position, targetSitPoint.position);
        if (dist <= snapDistance)
        {
            StartCoroutine(SnapToSitPoint());
        }
    }

    IEnumerator SnapToSitPoint()
    {
        isSitting = true;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        float duration = 0.25f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, targetSitPoint.position, t);
            transform.rotation = Quaternion.Slerp(startRot, targetSitPoint.rotation, t);
            yield return null;
        }

        transform.position = targetSitPoint.position;
        transform.rotation = targetSitPoint.rotation;

        animator.SetBool("sitting", true);
    }

    public void SetSelected(bool selected)
    {
        if (selectorCircle != null)
            selectorCircle.SetActive(selected);
    }

    public bool IsSelected() => selectorCircle.activeSelf;

    public bool AsignarSitPoint(Transform sitPoint)
    {
        var marker = sitPoint.GetComponent<SitPointMarker>();
        if (marker == null)
        {
            Debug.LogWarning("NO TIENE marker");
            return false;
        }

        if (!marker.EstaDisponible())
        {
            Debug.LogWarning("YA OCUPADO");
            return false;
        }

        targetSitPoint = sitPoint;
        sitPointMarker = marker;

        // PROBEMOS primero movernos
        bool resultado = agent.SetDestination(sitPoint.position);
        Debug.Log($"Intento mover a {sitPoint.position}, resultado: {resultado}, distancia: {Vector3.Distance(transform.position, sitPoint.position)}");

        if (!resultado)
        {
            Debug.LogWarning("NO PUDO CALCULAR RUTA");
            return false;
        }

        // Ahora sí marcamos ocupado
        sitPointMarker.MarcarOcupado(true);

        agent.isStopped = false;
        animator.SetBool("sitting", false);
        isSitting = false;

        return true;
    }
}