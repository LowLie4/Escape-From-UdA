using System.Collections;
using UnityEngine;

public class PortaSortida : MonoBehaviour
{
    public CaixaFusibles[] sockets;

    public GameObject cilindro1;
    public GameObject cilindro2;


    public void VerificarResistencies()
    {
        int verdes = 0;
        foreach (var s in sockets)
        {
            if (s.LEDAsignat.GetComponent<Renderer>().material.color == Color.green)
                verdes++;
        }

        if (verdes == 4)
        {
            Debug.Log("[GestorResistencias] ¡4 LEDs verdes detectados!");
            // Ejecuta tu acción aquí
            MoverCilindros();
        }
    }

    public void MoverCilindros()
    {
        StartCoroutine(MoverSuavementeLocal(cilindro1.transform, new Vector3(1.591f, -0.125f, -0.087f), 2f));
        StartCoroutine(MoverSuavementeLocal(cilindro2.transform, new Vector3(1.591f, -0.125f, -0.087f), 2f));


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


}
