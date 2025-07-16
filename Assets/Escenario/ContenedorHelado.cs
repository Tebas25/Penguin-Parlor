using UnityEngine;

public class ContenedorHelado : MonoBehaviour
{
    public string sabor;
    public Transform heladoMesh;
    public int maxUsos = 3;
    private int usosRestantes;
    public AudioClip sonidoRelleno;
    private AudioSource audioSource;
    public GameObject panelRellenoUI;

    void Start()
    {
        usosRestantes = maxUsos;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (EstaVacio() && panelRellenoUI != null && !panelRellenoUI.activeSelf)
            panelRellenoUI.SetActive(true);
    }

    public bool UsarHelado()
    {
        if (usosRestantes <= 0)
        {
            Debug.Log("Contenedor vacío.");
            return false;
        }

        usosRestantes--;
        if (heladoMesh != null)
            heladoMesh.localPosition -= new Vector3(0, 0.2f, 0);

        return true;
    }

    //public void Rellenar()
    //{
    //    if (GameManager.Instance.GastarDinero(100))
    //    {
    //        usosRestantes = maxUsos;
    //        if (heladoMesh != null)
    //            heladoMesh.localPosition += new Vector3(0, 0.2f * (maxUsos - usosRestantes), 0);
    //        audioSource.PlayOneShot(sonidoRelleno);
    //        if (panelRellenoUI != null)
    //            panelRellenoUI.SetActive(false);
    //    }
    //}
    public void Rellenar()
    {
        if (GameManager.Instance.GastarDinero(100))
        {
            int usosAntes = usosRestantes;
            usosRestantes = maxUsos;

            if (heladoMesh != null)
            {
                int usosRellenados = maxUsos - usosAntes;
                heladoMesh.localPosition += new Vector3(0, 0.2f * usosRellenados, 0);
            }

            audioSource.PlayOneShot(sonidoRelleno);

            if (panelRellenoUI != null)
                panelRellenoUI.SetActive(false);
        }
    }

    public bool EstaVacio() => usosRestantes <= 0;
}