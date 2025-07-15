using UnityEngine;

public class PlayerHelado : MonoBehaviour
{
    public Transform manoJugador;
    public GameObject conoPrefab;

    private GameObject conoActual;
    private string saborActual = "";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Recoger cono si no tiene uno
            if (conoActual == null)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("DespensaCono"))
                    {
                        RecogerCono();
                        break;
                    }
                }
            }
            else
            {
                // Servir sabor si está cerca de un contenedor
                Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("SaborFresa")) ServirHelado("fresa");
                    else if (hit.CompareTag("SaborVainilla")) ServirHelado("vainilla");
                    else if (hit.CompareTag("SaborChocolate")) ServirHelado("chocolate");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && conoActual != null)
        {
            Debug.Log("Intentando entregar cono...");
            Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (var hit in hits)
            {
                // Debug.Log($"Hit con: {hit.name}");
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
                        Debug.Log("Helado entregado con éxito.");
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
        if (conoPrefab == null)
        {
            Debug.LogWarning("Cono Prefab no asignado.");
            return;
        }

        if (manoJugador == null)
        {
            Debug.LogWarning("Mano del jugador no asignada.");
            return;
        }

        conoActual = Instantiate(conoPrefab, manoJugador);
        conoActual.transform.SetParent(manoJugador, false);
        conoActual.transform.localPosition = Vector3.zero;
        conoActual.transform.localRotation = Quaternion.identity;
        conoActual.transform.localScale = Vector3.one;
        saborActual = "";
        Debug.Log("¡Cono recogido exitosamente!");
    }

    void ServirHelado(string sabor)
    {
        if (saborActual != "") return; // ya servido

        saborActual = sabor;

        // Pintar bola de helado
        Transform bola = conoActual.transform.Find("BolaHelado");
        if (bola != null)
        {
            bola.gameObject.SetActive(true);
            Renderer rend = bola.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = ObtenerColor(sabor);
        }
    }

    Color ObtenerColor(string sabor)
    {
        switch (sabor)
        {
            case "fresa": return Color.magenta;
            case "chocolate": return new Color(0.4f, 0.2f, 0f);
            case "vainilla": return Color.yellow;
            default: return Color.white;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}