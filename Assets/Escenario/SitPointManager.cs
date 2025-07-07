using UnityEngine;

public class SitPointManager : MonoBehaviour
{
    void Start()
    {
        var puntos = FindObjectsOfType<SitPointMarker>();
        foreach (var punto in puntos)
        {
            punto.MarcarOcupado(false);
        }
    }
}