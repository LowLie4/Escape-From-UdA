using System.Numerics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class CaixaFusibles : MonoBehaviour


{
    [Tooltip("LED que se enciende cuando se inserta el fusible en este socket.")]
    public GameObject LEDAsignat;
    public PortaSortida portaSortida;

    public AudioSource _audioSource;
    public AudioClip soEngancha;         // assign in Inspector
    public AudioClip soDesengancha;       // assign in Inspector
    public AudioClip soCorrecte;    // assign in Inspector
    public AudioClip soElectricitatOn;
    public AudioClip soElectricitatOff;


    public void OnFusibleInserted(SelectEnterEventArgs args)
    {
        GameObject objetoInsertado = args.interactableObject.transform.gameObject;

        if (objetoInsertado.CompareTag("Fusible Fos"))
        {
            SetLEDColor(Color.red);
        }
        else if (objetoInsertado.CompareTag("Fusible Bo"))
        {
            SetLEDColor(Color.green);
        }
        else
        {
            Debug.LogWarning("[CaixaFusibles] El objeto insertado no tiene una tag válida.");
        }
    }

    public void OnFusbleRemoved(SelectExitEventArgs args)
    {
        GameObject objetoRetirado = args.interactableObject.transform.gameObject;

        SetLEDColor(Color.black);
    }

    private void SetLEDColor(Color color)
    {
        if (LEDAsignat != null)
        {
            Renderer renderer = LEDAsignat.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            else
            {
                Debug.LogError("[CaixaFusibles] No se encontró Renderer en el LED asignado.");
            }
        }
        else
        {
            Debug.LogError("[CaixaFusibles] LEDAsignat no está asignado en el Inspector.");
        }

        portaSortida?.VerificarResistencies();

    }
}
 