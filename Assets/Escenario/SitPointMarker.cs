using UnityEngine;

public class SitPointMarker : MonoBehaviour
{
    public bool Ocupado { get; private set; } = false;

    public bool EstaDisponible() => !Ocupado;

    public void MarcarOcupado(bool estado)
    {
        Ocupado = estado;
        Debug.Log($"{name} marcado como ocupado = {estado} (frame {Time.frameCount})");
    }
}