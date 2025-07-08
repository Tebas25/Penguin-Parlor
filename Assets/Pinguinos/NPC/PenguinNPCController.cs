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

    [Header("Detección de mesas")]
    private Vector3 mesaDestino;
    private Transform mesaTransform;
    public float distanciaAutoAsignacion = 2f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        SetSelected(false);
    }

    void Update()
    {
        if (isSitting) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);

        // Si ya tenemos SitPoint asignado, hacer snap
        if (targetSitPoint != null)
        {
            float dist = Vector3.Distance(transform.position, targetSitPoint.position);
            if (dist <= snapDistance)
            {
                StartCoroutine(SnapToSitPoint());
            }
            return;
        }

        // Si estamos cerca de la mesa pero aún no tenemos SitPoint
        if (mesaTransform != null && Vector3.Distance(transform.position, mesaDestino) <= distanciaAutoAsignacion)
        {
            AsignarSitPointCercano();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (targetSitPoint != null)
                Gizmos.DrawLine(transform.position, targetSitPoint.position);
            else if (mesaTransform != null)
                Gizmos.DrawLine(transform.position, mesaDestino);
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

    // Movimiento a mesa
    private void AsignarSitPointCercano()
    {
        var markers = mesaTransform.GetComponentsInChildren<SitPointMarker>();
        Debug.Log($"{name}: Intentando asignar SitPoint entre {markers.Length} disponibles.");

        foreach (var marker in markers)
        {
            Debug.Log($"{name}: Evaluando SitPoint '{marker.name}' → ocupado: {marker.EstaDisponible() == false}");

            if (marker.EstaDisponible())
            {
                marker.MarcarOcupado(true);
                targetSitPoint = marker.transform;
                sitPointMarker = marker;

                bool valid = agent.SetDestination(targetSitPoint.position);
                Debug.Log($"{name}: Asignado a SitPoint '{marker.name}'. ¿Ruta válida? {valid}");

                return;
            }
        }

        Debug.LogWarning($"{name}: No se encontró SitPoint libre o accesible en '{mesaTransform.name}'.");
    }


    public void MoveToMesa(Vector3 destino, Transform mesa)
    {
        mesaDestino = destino;
        mesaTransform = mesa;
        targetSitPoint = null;
        sitPointMarker = null;

        bool pathOk = agent.SetDestination(destino);
        agent.isStopped = false;
        isSitting = false;
        animator.SetBool("sitting", false);

        Debug.Log($"{name}: Enviando a mesa '{mesa.name}', destino: {destino}, path válido: {pathOk}");

        if (!pathOk)
            Debug.LogWarning($"{name}: No se pudo calcular ruta hacia la mesa '{mesa.name}'.");
    }

}