using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzleOrdinadorUSB : MonoBehaviour
{
    public TMP_Text textoMostrar;
    public GameObject usbGris;
    public GameObject usbDorada;
    public GameObject ProjeccioODS;

    // Evento que se ejecuta cuando se inserta un objeto en el socket
    public void OnUSBInserted(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.gameObject == usbGris)
        {
            textoMostrar.text = "Projecció Enviada";
            ProjeccioODS.SetActive(true); 
        }
        else if (args.interactableObject.transform.gameObject == usbDorada)
        {
            textoMostrar.text = "USB Dorada Conectada";
        }
        else
        {
            textoMostrar.text = "USB Desconocida Conectada";
        }
    }

    // Evento que se ejecuta cuando se retira un objeto del socket
    public void OnUSBRemoved(SelectExitEventArgs args)
    {
        textoMostrar.text = "Insertar USB";

        // Si el objeto retirado es la USB Gris, se oculta el objeto
        if (args.interactableObject.transform.gameObject == usbGris)
        {
            ProjeccioODS.SetActive(false);
        }
    }
}
