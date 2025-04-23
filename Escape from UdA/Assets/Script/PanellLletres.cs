using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PanellLletres : MonoBehaviour
{
    public XRSimpleInteractable[] botons; // Asignación de los 4 botones de la caja

    private List<Color> colorsSeleccionats = new List<Color>();
    private int numLletres = 0;
    private Array seleccioFinal;
    private string sequencia = ""; // Inicializar la secuencia
    private string lletraSeleccionada = "";

    public GameObject porta; // Asigna tu objeto en el inspector
    private GameObject newPivot;

    public TMP_Text textoMostrar;

    // Variables para controlar la rotación
    private float rotatedAngle = 0f;       // Ángulo acumulado
    public float targetAngle = 120f;       // Ángulo total deseado
    public float rotationSpeed = 20f;      // Velocidad de rotación (grados por segundo)
    private bool rotating = false;         // Indica si se está rotando

    void Start()
    {
        textoMostrar.text = "";

        foreach (XRSimpleInteractable boto in botons)
        {
            boto.selectEntered.AddListener((arg) => BotoPulsat(boto.gameObject));
        }

        if (porta == null) return;

        newPivot = new GameObject("NewPivot");
        newPivot.transform.position = porta.transform.position;
        newPivot.transform.rotation = porta.transform.rotation;

        // Hacer que "porta" sea hijo del nuevo pivote
        porta.transform.SetParent(newPivot.transform);
    }

    void Update()
    {
        //Rotar gradualmente la porta si se activó la rotación
        if (rotating && newPivot != null && rotatedAngle < targetAngle)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;

            if (rotatedAngle + rotationThisFrame > targetAngle)
            {
                rotationThisFrame = targetAngle - rotatedAngle;
            }

            // Rotación en sentido contrario (puedes cambiar -Vector3.up a Vector3.up según necesites)
            newPivot.transform.Rotate(-Vector3.down * rotationThisFrame);
            rotatedAngle += rotationThisFrame;

            if (rotatedAngle >= targetAngle)
            {
                rotating = false;
            }
        }
    }

    public void BotoPulsat(GameObject boto)
    {
      
        
        lletraSeleccionada = ObtindreLletraBoto(boto);

        if (lletraSeleccionada == "Enter")
        {
            if (sequencia == "TETAS") 
            {
                StartCoroutine(ParpadejarText(5, 0.3f)); // 5 veces, con 0.3s entre parpadeos
                rotating = true;
                rotatedAngle = 0f;
            }
            else
            {
                textoMostrar.text = "";
                textoMostrar.text = "Error";
                sequencia = "";
                Invoke("restablirPuzzle", 3f);
            }

        }
        else
        {
            if (textoMostrar.text.Length < 5)
            {
                sequencia += lletraSeleccionada;
                textoMostrar.text = sequencia;
            }
            
        }

            
        return;
        

    }

    private void restablirPuzzle()
    {
        textoMostrar.text = "";
    }

    private string ObtindreLletraBoto(GameObject boto)
    {
        string nomBoto = boto.name; // Ejemplo: "Boto a"

        // Validación: debe tener al menos dos palabras
        string[] parts = nomBoto.Split(' ');
        if (parts.Length >= 2)
        {
            return parts[1]; // Devolver la segunda parte, que sería la letra
        }

        // Si no se encuentra una letra válida, puedes devolver una cadena vacía o un valor por defecto
        return "";
    }

    private IEnumerator ParpadejarText(int vegades, float interval)
    {
        for (int i = 0; i < vegades; i++)
        {
            textoMostrar.enabled = false;
            yield return new WaitForSeconds(interval);
            textoMostrar.enabled = true;
            yield return new WaitForSeconds(interval);
        }
    }
}
