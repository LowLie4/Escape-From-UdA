using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CajaPuzzleColores : MonoBehaviour
{

    public XRSimpleInteractable[] botons; // Assignació dels 4 botons que hi han a la caixa

    public GameObject[] leds; // Assignació de les 4 "LEDs" que hi han a la caixa

    private List<Color> colorsSeleccionats = new List<Color>();
    private int numLEDs = 0;
    private Array seleccióFinal;
    private String sequencia;


    //public float anguloRotacion = 90f; // Ángulo específico en grados a rotar
    //public float velocidadRotacion = 5f; // Velocidad a la que se rotará (en grados por segundo)
    //public Transform ejeRotacion; // El punto sobre el cual gira (por lo general, uno de los lados de la puerta)

    //private bool obrirCaixa = false; // Indica si debe rotar
    //private float anguloActual = 0f; // El ángulo actual de la puerta

    // Start is called before the first frame update
    void Start()
    {

        foreach (XRSimpleInteractable boto in botons)
        {
            
            boto.activated.AddListener((arg) => BotoPulsat(boto.gameObject));
            
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
            seleccióFinal = colorsSeleccionats.ToArray();

            foreach (Color selecció in seleccióFinal)
            {
                if (selecció == Color.green) sequencia += "g"; 
                if (selecció == Color.blue) sequencia += "b";
                if (selecció == Color.red) sequencia += "r";
                if (selecció == new Color(1.0f, 0.5f, 0.0f)) sequencia += "o";

            }

            if (sequencia == "rrrr")
            {
                foreach (GameObject led in leds)
                {
                    renderer = led.GetComponent<Renderer>();
                    renderer.material.color = Color.green;
                }
                //obrirCaixa = true;
                //while (obrirCaixa)
                //{
                //    RotarTapaCaixa();
                //}
            }else
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
        seleccióFinal = null;
        sequencia = null;

        foreach (GameObject led in leds)
        {
            Renderer renderer = led.GetComponent<Renderer>();
            renderer.material.color = Color.white;
        }

    }

    Color ObtindreColorBoto(GameObject boto)
    {
        if (boto.name.Contains("Verd")) return Color.green;
        if (boto.name.Contains("Blau")) return Color.blue;
        if (boto.name.Contains("Vermell")) return Color.red;
        if (boto.name.Contains("Taronja")) return new Color(1.0f, 0.5f, 0.0f);
        return Color.white;

    }


   


}
