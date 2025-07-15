using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PenguinNPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    [Header("Visual y Audio")]
    public GameObject selectorCircle;
    public GameObject bienParticleSys;
    public GameObject malParticleSys;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;

    [Header("Lógica")]
    private Transform targetSitPoint;
    private SitPointMarker sitPointMarker;
    private bool isSitting = false;
    public float snapDistance = 1.5f;
    private bool permitirSeleccion = true;
    public Transform puntoSalida;

    private Vector3 mesaDestino;
    private Transform mesaTransform;
    public float distanciaAutoAsignacion = 2f;

    [Header("Pedidos")]
    public GameObject pedidoBubble;
    public Sprite[] iconosHelado;
    public enum SaborHelado { Fresa, Chocolate, Vainilla }
    private SaborHelado saborDeseado;
    private bool esperandoHelado = false;
    private bool recibioOrden = false;
    public bool EsperaOrden() => isSitting && !recibioOrden;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = gameObject.AddComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        SetSelected(false);
    }
    void Update()
    {
        if (isSitting) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);

        if (targetSitPoint != null)
        {
            float dist = Vector3.Distance(transform.position, targetSitPoint.position);
            if (dist <= snapDistance)
            {
                StartCoroutine(SnapToSitPoint());
            }
            return;
        }

        // Solo busca asiento si hay una mesa activa
        if (mesaTransform != null && Vector3.Distance(transform.position, mesaDestino) <= distanciaAutoAsignacion)
        {
            AsignarSitPointCercano();
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

        // Desactiva selección y visual
        permitirSeleccion = false;
        if (selectorCircle != null)
            selectorCircle.SetActive(false);

        StartCoroutine(FlujoCliente());
    }

    public void SetSelected(bool selected)
    {
        if (!permitirSeleccion) return;

        if (selectorCircle != null)
            selectorCircle.SetActive(selected);
    }

    public bool IsSelected() => selectorCircle != null && selectorCircle.activeSelf;

    public bool AsignarSitPoint(Transform sitPoint)
    {
        var marker = sitPoint.GetComponent<SitPointMarker>();
        if (marker == null)
        {
            Debug.LogWarning("No tiene marker");
            return false;
        }

        if (!marker.EstaDisponible())
        {
            Debug.LogWarning("Está ocupado");
            return false;
        }

        targetSitPoint = sitPoint;
        sitPointMarker = marker;

        bool resultado = agent.SetDestination(sitPoint.position);
        Debug.Log($"Intento mover a {sitPoint.position}, resultado: {resultado}, distancia: {Vector3.Distance(transform.position, sitPoint.position)}");

        if (!resultado)
        {
            Debug.LogWarning("No pudo calcular ruta");
            return false;
        }

        sitPointMarker.MarcarOcupado(true);
        agent.isStopped = false;
        animator.SetBool("sitting", false);
        isSitting = false;

        return true;
    }

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

    IEnumerator FlujoCliente()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        if (pedidoBubble != null)
        {
            pedidoBubble.SetActive(true);
            var icono = pedidoBubble.GetComponentInChildren<UnityEngine.UI.Image>();
            if (icono != null && iconosHelado.Length > 0)
            {
                int index = Random.Range(0, iconosHelado.Length);
                saborDeseado = (SaborHelado)index;
                icono.sprite = iconosHelado[index];
            }
        }

        esperandoHelado = true;

        while (esperandoHelado)
            yield return null;

        Debug.Log($"{name} recibió el pedido. Comiendo...");
        yield return new WaitForSeconds(4f);

        animator.SetBool("sitting", false);
        yield return new WaitForSeconds(1f);

        if (sitPointMarker != null)
            sitPointMarker.MarcarOcupado(false);

        mesaTransform = null;
        mesaDestino = Vector3.zero;
        targetSitPoint = null;
        sitPointMarker = null;
        isSitting = false;
        snapDistance = -1f;

        if (puntoSalida != null)
        {
            agent.SetDestination(puntoSalida.position);
            agent.isStopped = false;
            Debug.Log($"{name} caminando a salida '{puntoSalida.name}'");
        }
        else
        {
            Debug.LogWarning($"{name}: puntoSalida no asignado.");
        }

        while (puntoSalida != null && Vector3.Distance(transform.position, puntoSalida.position) > 1f)
            yield return null;

        Destroy(gameObject);
    }

    public void RecibirHelado(SaborHelado saborEntregado)
    {
        if (!esperandoHelado) return;

        if (saborEntregado == saborDeseado)
        {
            Debug.Log($"{name} recibió el sabor correcto: {saborEntregado}");
            esperandoHelado = false;
            recibioOrden = true;

            if (pedidoBubble != null)
                pedidoBubble.SetActive(false);

            if (bienParticleSys != null)
                Instantiate(bienParticleSys, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            audioSource.PlayOneShot(sonidoCorrecto);
        }
        else
        {
            Debug.Log($"{name} rechazó el sabor {saborEntregado}, esperaba {saborDeseado}");
            if (malParticleSys != null)
                Instantiate(malParticleSys, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            audioSource.PlayOneShot(sonidoIncorrecto);
        }
    }
}