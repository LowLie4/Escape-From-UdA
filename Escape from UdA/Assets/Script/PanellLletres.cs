using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PanellLletres : MonoBehaviour
{
    public XRSimpleInteractable[] botons;
    private string sequencia = "";
    private string lletraSeleccionada = "";
    private bool botonesActivos = true;


    public GameObject porta;
    private GameObject newPivot;

    public TMP_Text textoMostrar;

    public float targetAngle = 120f;
    public float rotationSpeed = 20f;

    public AudioSource _audioSource;
    public AudioClip soClic;         // assign in Inspector
    public AudioClip soError;       // assign in Inspector
    public AudioClip soCorrecte;    // assign in Inspector




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
        porta.transform.SetParent(newPivot.transform);



    }

    public void BotoPulsat(GameObject boto)
    {
        if (!botonesActivos) return; // Ignorar si están desactivados

        


        lletraSeleccionada = ObtindreLletraBoto(boto);

        if (lletraSeleccionada == "Enter")
        {
            if (sequencia == "XARXA")//XARXA
            {
                botonesActivos = false; // Desactivar funcionalidad

                _audioSource.PlayOneShot(soCorrecte);


                StartCoroutine(ParpadejarText(5, 0.3f));
                StartCoroutine(RotarPorta());
            }
            else
            {
                textoMostrar.text = "Error";
                _audioSource.volume = 0.5f;
                _audioSource.PlayOneShot(soError);
                _audioSource.volume = 1f;
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

                _audioSource.PlayOneShot(soClic);
            }
        }
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

    private void restablirPuzzle()
    {
        textoMostrar.text = "";
    }

    private string ObtindreLletraBoto(GameObject boto)
    {
        string[] parts = boto.name.Split(' ');
        return parts.Length >= 2 ? parts[1] : "";
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
