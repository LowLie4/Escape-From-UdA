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

    [Header("Configuración del parpadeo")]
    [Tooltip("Duración de la unidad de tiempo (dot).")]
    public float unitDuration = 0.2f;

    [Tooltip("Color cuando está \"encendido\" (blanco).")]
    public Color onColor = Color.white;

    [Tooltip("Color cuando está \"apagado\" (negro).")]
    public Color offColor = Color.black;

 


    public AudioSource _audioSource;
    public AudioClip soClic;         // assign in Inspector
    public AudioClip soError;       // assign in Inspector
    public AudioClip soCorrecte;    // assign in Inspector

    [Header("Morse en múltiples objetos")]
    [Tooltip("Asignar un Renderer por cada letra de la palabra")]
    public Renderer[] morseRenderers;  // ← Nuevo: arrastra aquí 5 Renderers

    private readonly Dictionary<char, string> morseCode = new Dictionary<char, string>()
    {
        { 'X', "-..-" },
        { 'A', ".-"   },
        { 'R', ".-."  }
        // si tuvieras más letras, las agregarías aquí
    };



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

        // Si no has arrastrado el Renderer en el Inspector, lo buscamos
        // Validación opcional por si no asignaste bien en el inspector:
        if (morseRenderers == null || morseRenderers.Length < 5)
            Debug.LogWarning("Debes asignar 5 Renderers en morseRenderers para cada letra.");

        StartCoroutine(FlashMorseLoop());
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
        _audioSource.PlayOneShot(soClic);
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

            if (sequencia == "RPGBf") //RPBfG
            {
                foreach (GameObject led in leds)
                {
                    renderer = led.GetComponent<Renderer>();
                    renderer.material.color = Color.green;
                }
                // Iniciar la rotación
                rotating = true;
                rotatedAngle = 0f;

                _audioSource.PlayOneShot(soCorrecte);

            }
            else
            {
                foreach (GameObject led in leds)
                {
                    renderer = led.GetComponent<Renderer>();
                    renderer.material.color = Color.red;
                    _audioSource.volume= 0.5f; // Ajustar el volumen
                    _audioSource.PlayOneShot(soError);
                    _audioSource.volume = 1f; // Volver al volumen original
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

    private IEnumerator FlashMorseLoop()
    {
        string palabra = "XARXA";

        while (true)
        {
            for (int i = 0; i < palabra.Length; i++)
            {
                char letra = palabra[i];

                // Asegurarnos de no salirnos del array:
                if (i < morseRenderers.Length && morseRenderers[i] != null)
                {
                    yield return StartCoroutine(FlashLetter(morseRenderers[i], letra));
                }
                else
                {
                    Debug.LogError($"No hay Renderer asignado en posición {i} para la letra '{letra}'");
                }

                // pausa entre letras = 3 unidades
                yield return new WaitForSeconds(unitDuration * 3f);
            }

            // tras enviar toda la palabra, espera antes de repetir
            yield return new WaitForSeconds(unitDuration * 7f);
        }
    }

    // Ahora recibe el Renderer por parámetro
    private IEnumerator FlashLetter(Renderer rend, char letra)
    {
        string code;
        if (!morseCode.TryGetValue(char.ToUpper(letra), out code))
            yield break;  // si la letra no está en tu diccionario, salimos

        foreach (char signal in code)
        {
            // Enciende
            rend.material.color = onColor;
            // Duración: dot = 1 unidad, dash = 3 unidades
            float duration = (signal == '.') ? unitDuration : unitDuration * 3f;
            yield return new WaitForSeconds(duration);

            // Apaga (pausa entre signals = 1 unidad)
            rend.material.color = offColor;
            yield return new WaitForSeconds(unitDuration);
        }
    }
}
