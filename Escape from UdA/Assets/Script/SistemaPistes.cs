using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SistemaPistes : MonoBehaviour
{
    public XRSimpleInteractable boto;
    public TMP_Text textoMostrar;
    public GameObject panelText;

    public AudioClip[] pistes;

    public AudioSource src;
    private int index = 0;

    public Animator animacioMontse;

    void Awake()
    {
       
        if (src == null)
            Debug.LogError("CyclePistas: no hi ha cap AudioSource al GameObject!");
    }


    void Start()
    {
        panelText.SetActive(false);
        boto.selectEntered.AddListener((arg) => BotoPulsat(boto.gameObject));
    }

    public void BotoPulsat(GameObject boto)
    {
        panelText.SetActive(true);

        ReproduirPista();
    }

    public void ReproduirPista()
    {
        animacioMontse.SetTrigger("EstaParlant");
        if (pistes == null || pistes.Length == 0)
            return;

        // Assigna i reprodueix el clip actual
        src.clip = pistes[index];
        src.Play();
        textoMostrar.text = "Potser el que busques està més a prop d'on s'ubiquen els estudiants que del professor.";

        // Passa a la següent, amb wrap-around
        index = (index + 1) % pistes.Length;

    }
}

