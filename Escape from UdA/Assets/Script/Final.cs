using UnityEngine;
using UnityEngine.SceneManagement;

public class Final : MonoBehaviour
{
    [Header("Configuració")]
    public Transform zonaFinal; // Posició de la zona final
    public float distanciaActivacio = 2f; // Distància per activar el final
    public Temporizador temporizador;
    
    [Header("Detecció Jugador")]
    public Transform jugador; // Assignar manualment o deixar buit per auto-detecció
    
    [Header("Canvi d'Escena")]
    public string nomEscenaSeguent = ""; // Nom de l'escena a carregar després de finalitzar
    public float tempsEsperaCanviEscena = 2f; // Temps d'espera abans de canviar d'escena
    
    [Header("Ranking")]
    public RankingManager rankingManager; // Referència al RankingManager per mostrar resultats
    
    private bool jocAcabat = false;

    void Start()
    {
        // Buscar el temporizador automàticament si no està assignat
        if (temporizador == null)
        {
            temporizador = FindObjectOfType<Temporizador>();
        }
        
        // Buscar el RankingManager automàticament si no està assignat
        if (rankingManager == null)
        {
            rankingManager = FindObjectOfType<RankingManager>();
        }
        
        // Buscar el jugador automàticament si no està assignat
        if (jugador == null)
        {
            // Intentar trobar per tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
            else
            {
                // Si no troba per tag, buscar per nom comú de VR
                GameObject vrRig = GameObject.Find("XR Rig");
                if (vrRig == null) vrRig = GameObject.Find("VR Rig");
                if (vrRig == null) vrRig = GameObject.Find("Player");
                
                if (vrRig != null)
                {
                    jugador = vrRig.transform;
                }
            }
        }
        
        // Verificar configuració
        if (jugador == null)
        {
            Debug.LogWarning("Final: No s'ha trobat el jugador! Assigna'l manualment al inspector.");
        }
        
        if (zonaFinal == null)
        {
            // Usar la posició d'aquest GameObject com a zona final
            zonaFinal = this.transform;
        }
    }

    void Update()
    {
        // Verificar distància cada frame si el joc no ha acabat
        if (!jocAcabat && jugador != null && zonaFinal != null)
        {
            float distancia = Vector3.Distance(jugador.position, zonaFinal.position);
            
            // Debug.Log("Distancia: " + distancia);
            
            if (distancia <= distanciaActivacio)
            {
                FinalizarJoc();
            }
        }
    }

    private void FinalizarJoc()
    {
        jocAcabat = true;
        
        // Aturar el temporizador i obtenir el temps final
        if (temporizador != null)
        {
            float tempsTotal = temporizador.GetTempsActual();
            temporizador.AturarTemporizador();
            
            Debug.Log($"Joc finalitzat! Temps total: {tempsTotal:F2} segons");
            
            // Guardar el temps a PlayerPrefs per passar-lo a la següent escena
            PlayerPrefs.SetFloat("UltimaPartidaTemps", tempsTotal);
            PlayerPrefs.SetString("UltimaPartidaTempsFormatat", FormatarTemps(tempsTotal));
            PlayerPrefs.Save();
            Debug.Log($"💾 Temps guardat a PlayerPrefs: {tempsTotal:F2}");
            
            // També guardar a GameDataManager si està disponible
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.GuardarTempsPartida(tempsTotal);
                Debug.Log($"🎮 Temps guardat a GameDataManager: {tempsTotal:F2}");
            }
            
            // Passar el temps al RankingManager per calcular la posició (si està en la mateixa escena)
            if (rankingManager != null)
            {
                rankingManager.SetTempsJugadorActual(tempsTotal);
                Debug.Log("📊 Temps passat al RankingManager");
            }
            else
            {
                Debug.LogWarning("⚠️ RankingManager no trobat!");
            }
            
            // Pujar la puntuació a Firebase i després canviar d'escena
            PujarPuntuacioFirebase(tempsTotal);
        }
        else
        {
            Debug.LogError("Final: No s'ha trobat el temporizador!");
        }
    }

    private string FormatarTemps(float tempsSegons)
    {
        int minuts = Mathf.FloorToInt(tempsSegons / 60);
        int segons = Mathf.FloorToInt(tempsSegons % 60);
        return $"{minuts:00}:{segons:00}";
    }

    private void PujarPuntuacioFirebase(float tempsSegons)
    {
        // Verificar que el ScoreUploader està disponible
        if (ScoreUploader.Instance != null)
        {
            ScoreUploader.Instance.PujarPuntuacio(tempsSegons);
            Debug.Log("📊 Puntuació enviada a Firebase!");
            
            // Canviar d'escena després d'un petit retard
            if (!string.IsNullOrEmpty(nomEscenaSeguent))
            {
                Invoke(nameof(CanviarEscena), tempsEsperaCanviEscena);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ ScoreUploader no trobat! Assegura't que hi ha un GameObject amb el script ScoreUploader a l'escena.");
            
            // Canviar d'escena igualment si està configurat
            if (!string.IsNullOrEmpty(nomEscenaSeguent))
            {
                Invoke(nameof(CanviarEscena), tempsEsperaCanviEscena);
            }
        }
    }

    private void CanviarEscena()
    {
        if (!string.IsNullOrEmpty(nomEscenaSeguent))
        {
            Debug.Log($"🎬 Canviant a l'escena: {nomEscenaSeguent}");
            SceneManager.LoadScene(nomEscenaSeguent);
        }
    }

    // Mètode per finalitzar manualment (per testing)
    [ContextMenu("Finalitzar Joc (Test)")]
    public void FinalizarJocManual()
    {
        FinalizarJoc();
    }

    // Mètode per canviar d'escena manualment (per testing)
    [ContextMenu("Canviar Escena (Test)")]
    public void CanviarEscenaManual()
    {
        CanviarEscena();
    }

    // Mostrar la zona de detecció a l'editor
    void OnDrawGizmosSelected()
    {
        if (zonaFinal != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(zonaFinal.position, distanciaActivacio);
        }
    }
} 