using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CajaPuzzleColores : MonoBehaviour
{
    public XRSimpleInteractable[] botons; // Asignación de los 4 botones de la caja
    public GameObject[] leds; // Asignación de las 4 "LEDs" de la caja

    private List<Color> colorsSeleccionats = new List<Color>();
    private int numLEDs = 0;
    private Array seleccioFinal;
    private string sequencia = ""; // Inicializar la secuencia

    public GameObject tapa; // Asigna tu objeto en el inspector
    private GameObject newPivot;

    // Variables para controlar la rotación
    private float rotatedAngle = 0f;       // Ángulo acumulado
    public float targetAngle = 120f;       // Ángulo total deseado
    public float rotationSpeed = 20f;      // Velocidad de rotación (grados por segundo)
    private bool rotating = false;         // Indica si se está rotando

    void Start()
    {
        foreach (XRSimpleInteractable boto in botons)
        {
            boto.selectEntered.AddListener((arg) => BotoPulsat(boto.gameObject));
        }

        if (tapa == null) return;

        newPivot = new GameObject("NewPivot");
        newPivot.transform.position = tapa.transform.position;
        newPivot.transform.rotation = tapa.transform.rotation;

        // Hacer que "tapa" sea hijo del nuevo pivote
        tapa.transform.SetParent(newPivot.transform);
    }

    void Update()
    {
        // Rotar gradualmente la tapa si se activó la rotación
        if (rotating && newPivot != null && rotatedAngle < targetAngle)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;

            if (rotatedAngle + rotationThisFrame > targetAngle)
            {
                rotationThisFrame = targetAngle - rotatedAngle;
            }

            // Rotación en sentido contrario (puedes cambiar -Vector3.up a Vector3.up según necesites)
            newPivot.transform.Rotate(-Vector3.right * rotationThisFrame);
            rotatedAngle += rotationThisFrame;

            if (rotatedAngle >= targetAngle)
            {
                rotating = false;
            }
        }
    }

    public void BotoPulsat(GameObject boto)
    {
        if (numLEDs >= 4)
        {
            return;
        }

        Color colorBoto = ObtindreColorBoto(boto);
        colorsSeleccionats.Add(colorBoto);

        Renderer renderer = leds[numLEDs].GetComponent<Renderer>();
        if (renderer)
        {
            renderer.material.color = colorBoto;
        }
        numLEDs++;

        if (numLEDs >= 4)
        {
            seleccioFinal = colorsSeleccionats.ToArray();
            sequencia = ""; // Reiniciar la secuencia para evitar problemas

            foreach (Color selecció in seleccioFinal)
            {
                if (selecció == new Color(0.6037736f, 0.0f, 0.0f)) sequencia += "R";
                if (selecció == new Color(1.0f, 0.2122642f, 0.9745069f)) sequencia += "P";
                if (selecció == new Color(0.0291019f, 0.002224996f, 0.4716981f)) sequencia += "Bf";
                if (selecció == new Color(0.3490196f, 0.7294118f, 0.2784314f)) sequencia += "G";
            }

            if (sequencia == "RPBfG")
            {
                foreach (GameObject led in leds)
                {
                    renderer = led.GetComponent<Renderer>();
                    renderer.material.color = Color.green;
                }
                // Iniciar la rotación
                rotating = true;
                rotatedAngle = 0f;
            }
            else
            {
                foreach (GameObject led in leds)
                {
                    renderer = led.GetComponent<Renderer>();
                    renderer.material.color = Color.red;
                }
            }

            Invoke("restablirPuzzle", 3f);
        }
    }

    void restablirPuzzle()
    {
        numLEDs = 0;
        colorsSeleccionats = new List<Color>();
        seleccioFinal = null;
        sequencia = "";

        foreach (GameObject led in leds)
        {
            Renderer renderer = led.GetComponent<Renderer>();
            renderer.material.color = Color.white;
        }
    }

    Color ObtindreColorBoto(GameObject boto)
    {
        if (boto.name.Equals("Boto Verd")) return new Color(0.3490196f, 0.7294118f, 0.2784314f);
        if (boto.name.Equals("Boto Taronja")) return new Color(1.0f, 0.5f, 0.0f);
        if (boto.name.Equals("Boto Vermell")) return new Color(0.6037736f, 0.0f, 0.0f);
        if (boto.name.Equals("Boto Groc")) return new Color(1.0f, 0.9798178f, 0.0f);
        if (boto.name.Equals("Boto Blau Fosc")) return new Color(0.0291019f, 0.002224996f, 0.4716981f);
        if (boto.name.Equals("Boto Blau Clar")) return new Color(0.2971698f, 0.7495989f, 1.0f);
        if (boto.name.Equals("Boto Rosa")) return new Color(1.0f, 0.2122642f, 0.9745069f);
        if (boto.name.Equals("Boto Verd Fosc")) return new Color(0.2588235f, 0.4980392f, 0.2745098f);
        if (boto.name.Equals("Boto Marro")) return new Color(0.7490196f, 0.5529411f, 0.172549f);
        return Color.white;
    }
}
