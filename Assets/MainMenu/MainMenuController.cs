using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelControles;
    public AudioSource clickAudio;
    public Animator fadeAnimator;

    public void Jugar()
    {
        clickAudio.Play();
        StartCoroutine(CargarEscena("SampleScene"));
    }

    public void MostrarControles()
    {
        clickAudio.Play();
        StartCoroutine(TransicionControles(true));
    }

    public void VolverMenu()
    {
        clickAudio.Play();
        StartCoroutine(TransicionControles(false));
    }

    IEnumerator CargarEscena(string nombre)
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f); // Duración del fade out
        SceneManager.LoadScene(nombre);
    }

    IEnumerator TransicionControles(bool aControles)
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f); // Espera a que termine fade out

        panelMenu.SetActive(!aControles);
        panelControles.SetActive(aControles);

        fadeAnimator.SetTrigger("FadeIn"); // Realiza fade in
    }
}