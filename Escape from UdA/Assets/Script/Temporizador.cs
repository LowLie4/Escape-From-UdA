using UnityEngine;
using TMPro;

public class Temporizador : MonoBehaviour
{
    [Header("Configuració")]
    public TextMeshPro textTemporizador;
    
    private float tempsInici;
    private bool temporizadorActiu = true;

    void Start()
    {
        // Guardar el temps quan comença l'escena
        tempsInici = Time.time;
        Debug.Log("Temporizador iniciat!");
    }

    void Update()
    {
        if (temporizadorActiu && textTemporizador != null)
        {
            // Calcular el temps transcorregut
            float tempsTranscorregut = Time.time - tempsInici;
            
            // Convertir a minuts i segons
            int minuts = Mathf.FloorToInt(tempsTranscorregut / 60);
            int segons = Mathf.FloorToInt(tempsTranscorregut % 60);
            
            // Formatar com MM:SS
            string tempsFormatat = string.Format("{0:00}:{1:00}", minuts, segons);
            
            // Mostrar al TextMeshPro
            textTemporizador.text = tempsFormatat;
        }
    }

    // Mètode per aturar el temporizador
    public void AturarTemporizador()
    {
        temporizadorActiu = false;
        Debug.Log("Temporizador aturat!");
    }

    // Mètode per obtenir el temps actual
    public float GetTempsActual()
    {
        return Time.time - tempsInici;
    }
} 