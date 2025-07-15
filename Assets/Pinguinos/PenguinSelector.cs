using UnityEngine;
using System.Collections.Generic;

public class PenguinSelector : MonoBehaviour
{
    public LayerMask penguinLayer;
    public LayerMask mesaLayer;
    public AudioClip sonidoSeleccion;
    private List<PenguinNPCController> selectedPenguins = new();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, penguinLayer))
            {
                PenguinNPCController penguin = hit.collider.GetComponentInParent<PenguinNPCController>();
                if (penguin != null)
                {
                    DeselectAll();

                    penguin.SetSelected(true);
                    selectedPenguins.Add(penguin);

                    if (sonidoSeleccion != null)
                        audioSource.PlayOneShot(sonidoSeleccion);
                }
            }
            else
            {
                DeselectAll();
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedPenguins.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mesaLayer))
            {
                Transform mesaDestino = hit.collider.transform.Find("MesaDestino");

                if (mesaDestino == null)
                {
                    Debug.LogWarning("La mesa no tiene 'MesaDestino'.");
                    return;
                }

                foreach (var penguin in selectedPenguins)
                {
                    penguin.MoveToMesa(mesaDestino.position, hit.collider.transform);
                }

                DeselectAll();
            }
        }
    }

    void DeselectAll()
    {
        foreach (var p in selectedPenguins)
            p.SetSelected(false);
        selectedPenguins.Clear();
    }
}