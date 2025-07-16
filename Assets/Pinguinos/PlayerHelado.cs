using UnityEngine;
using UnityEngine.Audio;

public class PlayerHelado : MonoBehaviour
{
    public Transform manoJugador;
    public GameObject conoPrefab;

    public Material materialFresa;
    public Material materialVainilla;
    public Material materialChocolate;

    private GameObject conoActual;
    private string saborActual = "";

    private AudioSource audioSource;
    public AudioClip sonidoDespensa;
    public AudioClip sonidoHelado;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (conoActual == null)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("DespensaCono"))
                    {
                        RecogerCono();
                        audioSource.PlayOneShot(sonidoDespensa);
                        break;
                    }
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("SaborFresa"))
                    {
                        ServirHelado("fresa", hit);
                        audioSource.PlayOneShot(sonidoHelado);
                    }
                    else if (hit.CompareTag("SaborVainilla"))
                    {
                        ServirHelado("vainilla", hit);
                        audioSource.PlayOneShot(sonidoHelado);
                    } 
                    else if (hit.CompareTag("SaborChocolate"))
                    {
                        ServirHelado("chocolate", hit);
                        audioSource.PlayOneShot(sonidoHelado);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && conoActual != null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (var hit in hits)
            {
                var npc = hit.GetComponent<PenguinNPCController>();
                if (npc != null && npc.EsperaOrden())
                {
                    Debug.Log($"Detectado NPC: {npc.name}, esperando: {npc.EsperaOrden()}");
                    if (System.Enum.TryParse(saborActual, true, out PenguinNPCController.SaborHelado saborEnum))
                    {
                        npc.RecibirHelado(saborEnum);

                        Destroy(conoActual);
                        conoActual = null;
                        saborActual = "";
                        Debug.Log("Helado entregado éxitosamente");
                    }
                    else
                    {
                        Debug.LogWarning("Sabor no válido: " + saborActual);
                    }
                }
            }
        }
    }

    void RecogerCono()
    {
        conoActual = Instantiate(conoPrefab, manoJugador);
        conoActual.transform.SetParent(manoJugador, false);
        conoActual.transform.localPosition = Vector3.zero;
        conoActual.transform.localRotation = Quaternion.identity;
        conoActual.transform.localScale = Vector3.one;
        saborActual = "";
        Debug.Log("Cono fue recogido");
    }

    void ServirHelado(string sabor, Collider hit)
    {
        if (saborActual != "") return;

        //saborActual = sabor;
        ContenedorHelado cont = hit.GetComponent<ContenedorHelado>();
        if (cont != null && !cont.EstaVacio())
        {
            if (cont.UsarHelado())
            {
                saborActual = sabor;
                // Aplica material como ya tienes hecho
            }
        }
        else
        {
            Debug.Log("Contenedor vacío, no se puede servir.");
        }

        Transform bola = conoActual.transform.Find("BolaHelado");
        if (bola != null)
        {
            bola.gameObject.SetActive(true);
            Renderer rend = bola.GetComponent<Renderer>();
            if (rend != null)
            {
                switch (sabor)
                {
                    case "fresa":
                        rend.material = materialFresa;
                        break;
                    case "chocolate":
                        rend.material = materialChocolate;
                        break;
                    case "vainilla":
                        rend.material = materialVainilla;
                        break;
                    default:
                        Debug.LogWarning("Sabor desconocido: " + sabor);
                        break;
                }
            }
        }
    }
}