using UnityEngine;
using System.Collections.Generic;

public class PenguinGroupSelector : MonoBehaviour
{
    public LayerMask penguinLayer;
    public LayerMask mesaLayer;
    private List<PenguinNPCController> selectedPenguins = new();

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
                    if (!Input.GetKey(KeyCode.LeftShift)) DeselectAll();

                    bool selected = !penguin.IsSelected();
                    penguin.SetSelected(selected);

                    if (selected) selectedPenguins.Add(penguin);
                    else selectedPenguins.Remove(penguin);
                }
            }
            else DeselectAll();
        }

        if (Input.GetMouseButtonDown(1) && selectedPenguins.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mesaLayer))
            {
                Debug.Log("Objeto clickeado: " + hit.collider.name);

                SitPointMarker[] puntos = hit.collider.GetComponentsInChildren<SitPointMarker>();
                int asignados = 0;

                foreach (var punto in puntos)
                {
                    if (!punto.EstaDisponible()) continue;
                    if (asignados >= selectedPenguins.Count) break;

                    bool ok = selectedPenguins[asignados].AsignarSitPoint(punto.transform);
                    if (ok) asignados++;
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