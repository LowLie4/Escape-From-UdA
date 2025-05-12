using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TPFinal : MonoBehaviour
{
    [Header("Configuraci�n de la carga de escena")]
    [Tooltip("Nombre de la escena que se cargar� al detectar el XR Ring")]
    public string sceneName = "NombreDeTuEscena";

    [Tooltip("Tag que identifica al XR Ring")]
    public string ringTag = "XRRing";

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos que el objeto entrante tenga el tag correcto
        if (other.CompareTag(ringTag))
        {
            // Opcional: puedes a�adir aqu� efectos de sonido, animaciones, etc.
            LoadScene();
        }
    }

    private void LoadScene()
    {
        // Verificamos que la escena est� incluida en el Build Settings
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"La escena '{sceneName}' no est� incluida en Build Settings.");
        }
    }
}
