using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    
    [Header("Dades de la Partida")]
    public float tempsUltimaPartida = 0f;
    public string tempsUltimaPartidaFormatat = "";
    public bool hiHaDadesDisponibles = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🎮 GameDataManager inicialitzat");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GuardarTempsPartida(float temps)
    {
        tempsUltimaPartida = temps;
        tempsUltimaPartidaFormatat = FormatarTemps(temps);
        hiHaDadesDisponibles = true;
        
        Debug.Log($"💾 Dades de partida guardades: {temps:F2} segons ({tempsUltimaPartidaFormatat})");
    }
    
    public float GetTempsUltimaPartida()
    {
        return tempsUltimaPartida;
    }
    
    public string GetTempsUltimaPartidaFormatat()
    {
        return tempsUltimaPartidaFormatat;
    }
    
    public bool TeDadesDisponibles()
    {
        return hiHaDadesDisponibles;
    }
    
    public void EsborrarDades()
    {
        tempsUltimaPartida = 0f;
        tempsUltimaPartidaFormatat = "";
        hiHaDadesDisponibles = false;
        Debug.Log("🗑️ Dades de partida esborrades");
    }
    
    private string FormatarTemps(float tempsSegons)
    {
        int minuts = Mathf.FloorToInt(tempsSegons / 60);
        int segons = Mathf.FloorToInt(tempsSegons % 60);
        return $"{minuts:00}:{segons:00}";
    }
    
    // Mètodes per testing
    [ContextMenu("Mostrar Dades")]
    public void MostrarDades()
    {
        Debug.Log($"📊 Temps: {tempsUltimaPartida:F2}s | Formatat: {tempsUltimaPartidaFormatat} | Disponible: {hiHaDadesDisponibles}");
    }
    
    [ContextMenu("Esborrar Dades")]
    public void EsborrarDadesManual()
    {
        EsborrarDades();
    }
} 