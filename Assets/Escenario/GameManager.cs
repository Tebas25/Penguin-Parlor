using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tiempo de juego")]
    public float duracionJuego = 90f;
    private float tiempoRestante;
    private bool juegoActivo = true;

    [Header("UI y fin del juego")]
    public Text dineroText;
    private int dinero = 25;
    public Text textoTimer;
    public GameObject panelResultados;
    public GameObject panelUI;
    public Text textoDineroFinal;
    public AudioClip sonidoFinDia;
    public AudioSource audioSource;
    public Button botonVolverMenu;
    public Image imagenMensajeExito;
    public Image imagenMensajeFracaso;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        tiempoRestante = duracionJuego;
        ActualizarUI();

        if (botonVolverMenu != null)
            botonVolverMenu.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        if (imagenMensajeExito != null) imagenMensajeExito.gameObject.SetActive(false);
        if (imagenMensajeFracaso != null) imagenMensajeFracaso.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante < 0)
        {
            tiempoRestante = 0;
            FinDelDia();
        }

        if (textoTimer != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
            textoTimer.text = $"{minutos:00}:{segundos:00}";
        }
    }

    public void AñadirDinero(int cantidad)
    {
        dinero += cantidad;
        ActualizarUI();
    }

    public bool GastarDinero(int cantidad)
    {
        if (dinero >= cantidad)
        {
            dinero -= cantidad;
            ActualizarUI();
            return true;
        }
        return false;
    }

    void ActualizarUI()
    {
        if (dineroText != null)
            dineroText.text = "$" + dinero.ToString();
    }

    void FinDelDia()
    {
        juegoActivo = false;

        if (audioSource != null && sonidoFinDia != null)
            audioSource.PlayOneShot(sonidoFinDia);

        if (panelUI != null)
            panelUI.SetActive(false);

        if (panelResultados != null)
            panelResultados.SetActive(true);

        if (textoDineroFinal != null)
            textoDineroFinal.text = "Dinero final: $" + dinero;

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        Debug.Log("NPCs encontrados al final del juego: " + npcs.Length);
        bool hayNPCs = npcs.Length > 0;

        if (imagenMensajeExito != null)
            imagenMensajeExito.gameObject.SetActive(!hayNPCs);
        if (imagenMensajeFracaso != null)
            imagenMensajeFracaso.gameObject.SetActive(hayNPCs);
    }
}