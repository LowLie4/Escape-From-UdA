using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public AudioSource audioSource;         // Fuente de audio (�nica)
    public AudioClip Intro;                // Primer audio
    public AudioClip Moviments;                // Segundo audio
    public AudioClip FicarClau;                // Tercer audio (con animaci�n)
    public AudioClip IntroButons;
    public AudioClip MovimentsButons;
    public AudioClip FiTutorial;

    public Animator animator;               // Referencia al Animator
    public Animator animatorMontse;
    public Animator animatorBotons;

    public TMP_Text textoMostrar;


    private void Start()
    {
        StartCoroutine(ReproducirAudiosConAnimacion());
    }

    private System.Collections.IEnumerator ReproducirAudiosConAnimacion()
    {
        // Reproducir primer audio
        audioSource.clip = Intro;
        audioSource.Play();
        textoMostrar.text = "No se com hem acabat aqui! Abans de comen�ar, farem un petit tutorial per que t'adaptis als controls.";
        animatorMontse.SetTrigger("AgafaCap");
        yield return new WaitForSeconds(Intro.length);
        animatorMontse.SetTrigger("StopParlar");

        // Reproducir segundo audio
        audioSource.clip = Moviments;
        audioSource.Play();
        textoMostrar.text = "Per despla�ar-te, utilitza la palanca de control de la m� esquerra. ATENCIO! Aquest moviment pot provocar mareig. Tamb� pots despla�ar-te per teletransportaci�, utilitzant la palanca de control de la m� dreta cap al davant, apuntant a terra.";
        animatorMontse.SetTrigger("StartParlar");
        yield return new WaitForSeconds(Moviments.length);
        animatorMontse.SetTrigger("StopParlar");

        // Reproducir tercer audio y lanzar animaci�n al mismo tiempo
        audioSource.clip = FicarClau;
        audioSource.Play();
        textoMostrar.text = "Aprendrem a com funcionen les interaccions. Agafa la clau de la meva dreta i introdueix-la en el pany.";
        animator.SetTrigger("MostrarCaixa");
        animatorMontse.SetTrigger("StartParlar");
        yield return new WaitForSeconds(FicarClau.length);
        animatorMontse.SetTrigger("StopParlar");


    }

    public void Clauposada(SelectEnterEventArgs args)
    {
        StartCoroutine(AudiosBotons());
    }

    private System.Collections.IEnumerator AudiosBotons()
    {
        audioSource.clip = IntroButons;
        audioSource.Play();
        textoMostrar.text = "Perfecte! Ara mirarem l'interacci� amb els botons.";
        animator.SetTrigger("OcultaCaixa");
        animatorMontse.SetTrigger("StartParlar");
        yield return new WaitForSeconds(IntroButons.length);
        animatorMontse.SetTrigger("StopParlar");
        animatorBotons.SetTrigger("MostrarCaixa");
    }

    public void BotoPulsat(SelectEnterEventArgs args)
    {
        StartCoroutine(FinalitzaTutorial());
    }

    private System.Collections.IEnumerator FinalitzaTutorial()
    {
        audioSource.clip = FiTutorial;
        audioSource.Play();
        textoMostrar.text = "Hem acabat amb el tutorial! Bona Sort!";
        animatorBotons.SetTrigger("OcultaCaixa");
        animatorMontse.SetTrigger("Ballar");
        yield return new WaitForSeconds(FiTutorial.length);

        SceneManager.LoadScene("Joc"); // Cargar la escena principal
    }


}
