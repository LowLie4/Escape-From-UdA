using System.Collections;
using UnityEngine;

public class PortaSortida : MonoBehaviour
{
    public CaixaFusibles[] sockets;

    public GameObject cilindro1;
    public GameObject cilindro2;

    public GameObject porta; // Asigna tu objeto en el inspector
    private GameObject newPivot;

    // Variables para controlar la rotación
    public float targetAngle = 120f;       // Ángulo total deseado
    public float rotationSpeed = 20f;      // Velocidad de rotación (grados por segundo)


    private void Start()
    {
        newPivot = new GameObject("NewPivot");
        newPivot.transform.position = porta.transform.position;
        newPivot.transform.rotation = porta.transform.rotation;
        porta.transform.SetParent(newPivot.transform);
    }

    public void VerificarResistencies()
    {
        StartCoroutine(AccionCompleta()); // Iniciar toda la secuencia

        int verdes = 0;
        foreach (var s in sockets)
        {
            if (s.LEDAsignat.GetComponent<Renderer>().material.color == Color.green)
                verdes++;
        }

        if (verdes == 4)
        {
            Debug.Log("[GestorResistencias] ¡4 LEDs verdes detectados!");
            StartCoroutine(AccionCompleta()); // Iniciar toda la secuencia
        }
    }

    private IEnumerator AccionCompleta()
    {
        yield return StartCoroutine(MoverSuavementeLocal(cilindro1.transform, new Vector3(1.591f, -0.125f, -0.087f), 2f));
        yield return StartCoroutine(MoverSuavementeLocal(cilindro2.transform, new Vector3(1.591f, -0.125f, -0.087f), 2f));
        yield return StartCoroutine(RotarPorta());
    }



    private IEnumerator MoverSuavementeLocal(Transform objeto, Vector3 posicionFinal, float duracion)
        {
            Vector3 posicionInicial = objeto.localPosition;
            float tiempo = 0f;

            while (tiempo < duracion)
            {
                objeto.localPosition = Vector3.Lerp(posicionInicial, posicionFinal, tiempo / duracion);
                tiempo += Time.deltaTime;
                yield return null;
            }

            objeto.localPosition = posicionFinal;
    }


    private IEnumerator RotarPorta()
    {
        float rotatedAngle = 0f;

        while (rotatedAngle < targetAngle)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            if (rotatedAngle + rotationThisFrame > targetAngle)
                rotationThisFrame = targetAngle - rotatedAngle;

            newPivot.transform.Rotate(-Vector3.down * rotationThisFrame);
            rotatedAngle += rotationThisFrame;
            yield return null;
        }
    }


}
