using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Auth;
using Firebase;
using TMPro;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class PuntuacioJugador
{
    public string nom;
    public float temps;
    public string tempsFormatat;
    public long timestamp;

    public PuntuacioJugador(string nom, float temps, string tempsFormatat, long timestamp)
    {
        this.nom = nom;
        this.temps = temps;
        this.tempsFormatat = tempsFormatat;
        this.timestamp = timestamp;
    }
}

public class RankingManager : MonoBehaviour
{
    [Header("Referències UI")]
    public TextMeshPro textRankingTop3;
    public TextMeshPro textPosicioJugador;
    public TextMeshPro textIntroduirNom; // Per mostrar missatge d'introduir nom
    
    [Header("Input Nom Personalitzat")]
    public TMP_InputField inputFieldNom; // Camp d'input per introduir el nom (opcional, per UI normal)
    public Button botoGuardarNom; // Botó per guardar el nom
    public Button botoMostrarTeclat; // Botó per mostrar el teclat VR
    
    [Header("Teclat VR (MRTK)")]
    public bool usarTeclatVR = true; // Si usar el teclat VR o InputField normal
    public Transform posicioTeclat; // On posicionar el teclat VR
    
    [Header("Configuració")]
    public float tempsJugadorActual = 0f; // Temps del jugador actual (assignar des d'altre script)
    
    private DatabaseReference _dbRef;
    private List<PuntuacioJugador> totesPuntuacions = new List<PuntuacioJugador>();
    private bool estaEnPodio = false;
    private int posicioJugador = 0;
    private string clauPuntuacioActual = ""; // Clau de la puntuació actual a Firebase
    private string nomTemporalIntroduit = ""; // Nom introduït pel jugador

    async void Start()
    {
        await InicialitzarFirebase();
        await CarregarPuntuacions();
        
        // Carregar el temps del jugador des de PlayerPrefs
        CarregarTempsJugadorDesdePlayerPrefs();
        
        ProcessarRanking();
        
        // Configurar els botons
        if (botoGuardarNom != null)
        {
            botoGuardarNom.onClick.AddListener(GuardarNomPersonalitzat);
        }
        
        if (botoMostrarTeclat != null)
        {
            botoMostrarTeclat.onClick.AddListener(MostrarTeclatVR);
        }
        
        // Subscriure's als events del teclat VR si està disponible
        ConfigurarTeclatVR();
    }

    private async System.Threading.Tasks.Task InicialitzarFirebase()
    {
        try
        {
            // Comprova dependències i inicialitza Firebase
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus != DependencyStatus.Available)
            {
                Debug.LogError($"Firebase no disponible: {dependencyStatus}");
                return;
            }

            // Anonim login
            var authTask = FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
            await authTask;
            
            if (authTask.Exception != null)
            {
                Debug.LogError($"Error sign-in: {authTask.Exception}");
                return;
            }

            _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("🔥 Firebase inicialitzat per Ranking");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inicialitzant Firebase: {e.Message}");
        }
    }

    private async System.Threading.Tasks.Task CarregarPuntuacions()
    {
        if (_dbRef == null)
        {
            Debug.LogError("❌ Firebase no està inicialitzat!");
            return;
        }

        try
        {
            Debug.Log("📊 Carregant puntuacions...");
            
            var snapshot = await _dbRef.Child("puntuacions").GetValueAsync();
            
            totesPuntuacions.Clear();
            
            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    var data = child.Value as Dictionary<string, object>;
                    if (data != null)
                    {
                        string nom = data["nom"].ToString();
                        float temps = Convert.ToSingle(data["temps"]);
                        string tempsFormatat = data["tempsFormatat"].ToString();
                        long timestamp = Convert.ToInt64(data["timestamp"]);
                        
                        totesPuntuacions.Add(new PuntuacioJugador(nom, temps, tempsFormatat, timestamp));
                    }
                }
                
                Debug.Log($"✅ Carregades {totesPuntuacions.Count} puntuacions");
            }
            else
            {
                Debug.Log("📊 No hi ha puntuacions a la base de dades");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error carregant puntuacions: {e.Message}");
        }
    }

    private void ProcessarRanking()
    {
        // Ordenar puntuacions de menor a major temps (millor temps primer)
        var puntuacionsOrdenades = totesPuntuacions.OrderBy(p => p.temps).ToList();
        
        Debug.Log($"📊 Processant ranking amb {puntuacionsOrdenades.Count} puntuacions:");
        for (int i = 0; i < puntuacionsOrdenades.Count && i < 5; i++)
        {
            Debug.Log($"   {i + 1}. {puntuacionsOrdenades[i].nom} - {puntuacionsOrdenades[i].temps:F2}s ({puntuacionsOrdenades[i].tempsFormatat})");
        }
        
        // Mostrar TOP 3
        MostrarTop3(puntuacionsOrdenades);
        
        // Verificar posició del jugador actual
        VerificarPosicioJugador(puntuacionsOrdenades);
    }

    private void MostrarTop3(List<PuntuacioJugador> puntuacionsOrdenades)
    {
        if (textRankingTop3 == null) return;
        
        string rankingText = "TOP 3 MILLORS TEMPS \n\n";
        
        for (int i = 0; i < Mathf.Min(3, puntuacionsOrdenades.Count); i++)
        {
            var jugador = puntuacionsOrdenades[i];
            string medalles = GetMedalla(i + 1);
            rankingText += $"{jugador.nom} - {jugador.tempsFormatat}\n";
        }
        
        if (puntuacionsOrdenades.Count == 0)
        {
            rankingText = " No hi ha puntuacions encara\n¡Sigues el primer!";
        }
        
        textRankingTop3.text = rankingText;
        Debug.Log("🏆 TOP 3 mostrat");
    }

    private void VerificarPosicioJugador(List<PuntuacioJugador> puntuacionsOrdenades)
    {
        if (tempsJugadorActual <= 0)
        {
            Debug.LogWarning("⚠️ Temps del jugador actual no assignat");
            return;
        }
        
        // Calcular la posició directament comparant amb els temps existents
        posicioJugador = 1; // Començar assumint que és el primer
        int jugadorsMesRapids = 0;
        
        Debug.Log($"🎯 Calculant posició per temps jugador: {tempsJugadorActual:F2}s");
        
        foreach (var puntuacio in puntuacionsOrdenades)
        {
            if (puntuacio.temps < tempsJugadorActual)
            {
                jugadorsMesRapids++;
                posicioJugador++; // Hi ha algú més ràpid, baixar una posició
                Debug.Log($"   👆 {puntuacio.nom} és més ràpid: {puntuacio.temps:F2}s < {tempsJugadorActual:F2}s");
            }
            else
            {
                Debug.Log($"   👇 {puntuacio.nom} és més lent: {puntuacio.temps:F2}s >= {tempsJugadorActual:F2}s");
            }
        }
        
        // Verificar si està en el podio (TOP 3)
        estaEnPodio = posicioJugador <= 3;
        
        Debug.Log($" RESULTAT: Posició {posicioJugador} (hi ha {jugadorsMesRapids} jugadors més ràpids) - Podio: {estaEnPodio}");
        
        // Mostrar resultat
        MostrarPosicioJugador();
    }

    private void MostrarPosicioJugador()
    {
        if (textPosicioJugador != null)
        {
            string tempsFormatat = FormatarTemps(tempsJugadorActual);
            string missatge = $" POSICIÓ: {posicioJugador}\n TEMPS: {tempsFormatat}";
            
            if (estaEnPodio)
            {
                string medalles = GetMedalla(posicioJugador);
                missatge = $"POSICIÓ: {posicioJugador}\n TEMPS: {tempsFormatat}\n HAS ENTRAT AL PODIO!";
            }
            
            textPosicioJugador.text = missatge;
        }
        
        // Mostrar/amagar elements segons si està en podio
        if (estaEnPodio)
        {
            // Mostrar missatge per introduir nom
            if (textIntroduirNom != null)
            {
                if (usarTeclatVR)
                {
                    textIntroduirNom.text = "PREM EL BOTÓ PER INTRODUIR\nEL TEU NOM";
                }
                else
                {
                    textIntroduirNom.text = "INTRODUEIX EL TEU NOM\nPER APARÈIXER AL RANKING";
                }
            }
            
            // Mostrar elements UI segons el tipus de teclat
            if (usarTeclatVR)
            {
                // Mostrar botó per obrir teclat VR
                if (botoMostrarTeclat != null) botoMostrarTeclat.gameObject.SetActive(true);
                if (botoGuardarNom != null) botoGuardarNom.gameObject.SetActive(false); // No necessari amb VR
                if (inputFieldNom != null) inputFieldNom.gameObject.SetActive(false); // No necessari amb VR
            }
            else
            {
                // Mostrar input field i botó tradicionals
                if (inputFieldNom != null) 
                {
                    inputFieldNom.gameObject.SetActive(true);
                    inputFieldNom.text = ""; // Netejar el camp
                }
                if (botoGuardarNom != null) botoGuardarNom.gameObject.SetActive(true);
                if (botoMostrarTeclat != null) botoMostrarTeclat.gameObject.SetActive(false);
            }
        }
        else
        {
            // Amagar tot si no està en podio
            if (textIntroduirNom != null) textIntroduirNom.text = "";
            if (inputFieldNom != null) inputFieldNom.gameObject.SetActive(false);
            if (botoGuardarNom != null) botoGuardarNom.gameObject.SetActive(false);
            if (botoMostrarTeclat != null) botoMostrarTeclat.gameObject.SetActive(false);
        }
        
        Debug.Log($" RESULTAT: Posició {posicioJugador} - Podio: {estaEnPodio}");
    }

    private string GetMedalla(int posicio)
    {
        switch (posicio)
        {
            case 1: return "🥇";
            case 2: return "🥈";
            case 3: return "🥉";
            default: return $"{posicio}.";
        }
    }

    private string FormatarTemps(float tempsSegons)
    {
        int minuts = Mathf.FloorToInt(tempsSegons / 60);
        int segons = Mathf.FloorToInt(tempsSegons % 60);
        return $"{minuts:00}:{segons:00}";
    }

    // Mètodes públics per usar des d'altres scripts
    public void SetTempsJugadorActual(float temps)
    {
        tempsJugadorActual = temps;
        Debug.Log($"⏱️ Temps jugador actual assignat: {temps:F2} segons");
    }

    public bool EstaEnPodio()
    {
        return estaEnPodio;
    }

    public int GetPosicioJugador()
    {
        return posicioJugador;
    }

    // Mètodes per testing
    [ContextMenu("Recarregar Ranking")]
    public async void RecarregarRanking()
    {
        await CarregarPuntuacions();
        ProcessarRanking();
    }

    [ContextMenu("Test amb Temps Aleatori")]
    public void TestAmbTempsAleatori()
    {
        float tempsAleatori = UnityEngine.Random.Range(60f, 300f);
        SetTempsJugadorActual(tempsAleatori);
        ProcessarRanking();
    }

    private void CarregarTempsJugadorDesdePlayerPrefs()
    {
        // Primer intentar carregar des de GameDataManager (si està disponible)
        if (GameDataManager.Instance != null && GameDataManager.Instance.TeDadesDisponibles())
        {
            float temps = GameDataManager.Instance.GetTempsUltimaPartida();
            SetTempsJugadorActual(temps);
            Debug.Log($"🎮 Temps carregat des de GameDataManager: {temps:F2} segons");
            return;
        }
        
        // Si no, intentar carregar des de PlayerPrefs
        if (PlayerPrefs.HasKey("UltimaPartidaTemps"))
        {
            float temps = PlayerPrefs.GetFloat("UltimaPartidaTemps");
            SetTempsJugadorActual(temps);
            Debug.Log($"💾 Temps carregat des de PlayerPrefs: {temps:F2} segons");
            
            // Opcional: Esborrar després de carregar per evitar confusions
            // PlayerPrefs.DeleteKey("UltimaPartidaTemps");
            // PlayerPrefs.DeleteKey("UltimaPartidaTempsFormatat");
        }
        else
        {
            Debug.LogWarning("⚠️ No s'ha trobat temps de l'última partida en cap sistema");
        }
    }

    private void ConfigurarTeclatVR()
    {
        if (!usarTeclatVR) return;
        
        // Intentar trobar el component NonNativeKeyboard
        var keyboard = FindObjectOfType<Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard>();
        if (keyboard != null)
        {
            try
            {
                // OnTextUpdated usa Action<string>
                keyboard.OnTextUpdated += OnTeclatTextActualitzat;
                Debug.Log("🎹 OnTextUpdated configurat com Action<string>");
                
                // OnTextSubmitted usa EventHandler
                keyboard.OnTextSubmitted += OnTeclatTextEnviatEventHandler;
                Debug.Log("🎹 OnTextSubmitted configurat com EventHandler");
                
                Debug.Log("🎹 Teclat VR configurat correctament amb tipus mixtos");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Error configurant events: {e.Message}");
                Debug.LogWarning("⚠️ Activant fallback a InputField tradicional...");
                usarTeclatVR = false; // Fallback si no funciona
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No s'ha trobat el component NonNativeKeyboard. Assegura't que el prefab està a l'escena.");
            usarTeclatVR = false; // Fallback a InputField normal
        }
    }

    // Funció per EventHandler (OnTextSubmitted)
    private async void OnTeclatTextEnviatEventHandler(object sender, System.EventArgs e)
    {
        // Obtenir el text final del teclat
        if (Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance != null)
        {
            string text = Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance.InputField.text.Trim();
            await OnTeclatTextEnviat(text);
        }
    }

    // Funció per Action<string> (OnTextUpdated)
    private void OnTeclatTextActualitzat(string text)
    {
        // Actualitzar el text temporal mentre l'usuari escriu
        nomTemporalIntroduit = text;
        Debug.Log($"📝 Text actualitzat: '{text}'");
        
        // Opcional: Mostrar el text en temps real en algun lloc de la UI
        if (textIntroduirNom != null)
        {
            textIntroduirNom.text = $"INTRODUEIX EL TEU NOM:\n'{text}'";
        }
    }

    // Funció interna per processar text enviat
    private async System.Threading.Tasks.Task OnTeclatTextEnviat(string text)
    {
        // Aquest event es dispara quan l'usuari prem Enter
        nomTemporalIntroduit = text.Trim();
        Debug.Log($"✅ Text enviat des del teclat VR: '{nomTemporalIntroduit}'");
        
        // Amagar el teclat
        if (Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance != null)
        {
            Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance.Close();
        }
        
        // Guardar automàticament el nom
        await GuardarNomPersonalitzatAmbText(nomTemporalIntroduit);
    }

    public async void GuardarNomPersonalitzat()
    {
        string nomPersonalitzat = "";
        
        if (usarTeclatVR)
        {
            // Usar el nom del teclat VR
            nomPersonalitzat = nomTemporalIntroduit;
        }
        else if (inputFieldNom != null)
        {
            // Usar el InputField normal
            nomPersonalitzat = inputFieldNom.text.Trim();
        }
        
        await GuardarNomPersonalitzatAmbText(nomPersonalitzat);
    }

    private async System.Threading.Tasks.Task GuardarNomPersonalitzatAmbText(string nomPersonalitzat)
    {
        // Validar el nom
        if (string.IsNullOrEmpty(nomPersonalitzat))
        {
            Debug.LogWarning("⚠️ El nom no pot estar buit!");
            if (textIntroduirNom != null)
            {
                textIntroduirNom.text = "❌ EL NOM NO POT ESTAR BUIT!";
            }
            return;
        }
        
        if (nomPersonalitzat.Length > 20)
        {
            Debug.LogWarning("⚠️ El nom és massa llarg (màxim 20 caràcters)!");
            if (textIntroduirNom != null)
            {
                textIntroduirNom.text = "❌ NOM MASSA LLARG (MÀX 20)!";
            }
            return;
        }
        
        // Només permetre si està en el podio
        if (!estaEnPodio)
        {
            Debug.LogWarning("⚠️ Només els jugadors del podio poden canviar el nom!");
            return;
        }
        
        Debug.Log($"💾 Guardant nom personalitzat: '{nomPersonalitzat}'");
        
        await ActualitzarNomAFirebase(nomPersonalitzat);
    }
    
    private async System.Threading.Tasks.Task ActualitzarNomAFirebase(string nouNom)
    {
        if (_dbRef == null)
        {
            Debug.LogError("❌ Firebase no està inicialitzat!");
            return;
        }
        
        try
        {
            // Buscar la puntuació més recent amb el temps del jugador actual
            string clauPuntuacio = await TrobarPuntuacioActual();
            
            if (string.IsNullOrEmpty(clauPuntuacio))
            {
                Debug.LogError("❌ No s'ha trobat la puntuació actual a Firebase!");
                return;
            }
            
            // Actualitzar només el nom
            var actualitzacio = new Dictionary<string, object>
            {
                ["nom"] = nouNom
            };
            
            await _dbRef.Child("puntuacions").Child(clauPuntuacio).UpdateChildrenAsync(actualitzacio);
            
            Debug.Log($"✅ Nom actualitzat a Firebase: {nouNom}");
            
            // Recarregar el ranking per mostrar els canvis
            await CarregarPuntuacions();
            ProcessarRanking();
            
            // Amagar el camp d'input i el botó
            if (inputFieldNom != null) inputFieldNom.gameObject.SetActive(false);
            if (botoGuardarNom != null) botoGuardarNom.gameObject.SetActive(false);
            if (textIntroduirNom != null) textIntroduirNom.text = "✅ NOM GUARDAT CORRECTAMENT!";
            
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error actualitzant nom a Firebase: {e.Message}");
        }
    }
    
    private async System.Threading.Tasks.Task<string> TrobarPuntuacioActual()
    {
        try
        {
            var snapshot = await _dbRef.Child("puntuacions").GetValueAsync();
            
            if (!snapshot.Exists) return "";
            
            string clauTrobada = "";
            long timestampMesRecent = 0;
            
            // Buscar la puntuació més recent amb el temps del jugador actual
            foreach (var child in snapshot.Children)
            {
                var data = child.Value as Dictionary<string, object>;
                if (data != null)
                {
                    float temps = Convert.ToSingle(data["temps"]);
                    long timestamp = Convert.ToInt64(data["timestamp"]);
                    
                    // Comparar temps amb una petita tolerància (0.1 segons)
                    if (Mathf.Abs(temps - tempsJugadorActual) < 0.1f && timestamp > timestampMesRecent)
                    {
                        clauTrobada = child.Key;
                        timestampMesRecent = timestamp;
                    }
                }
            }
            
            Debug.Log($"🔍 Puntuació trobada: {clauTrobada} (timestamp: {timestampMesRecent})");
            return clauTrobada;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error buscant puntuació actual: {e.Message}");
            return "";
        }
    }

    private void MostrarTeclatVR()
    {
        if (!usarTeclatVR)
        {
            Debug.LogWarning("⚠️ Teclat VR no està activat");
            return;
        }
        
        // Usar el singleton del teclat MRTK
        if (Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance != null)
        {
            // Posicionar el teclat si tenim una posició definida
            if (posicioTeclat != null)
            {
                Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance.transform.position = posicioTeclat.position;
                Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance.transform.rotation = posicioTeclat.rotation;
            }
            
            // Mostrar el teclat amb un placeholder
            Microsoft.MixedReality.Toolkit.Experimental.UI.NonNativeKeyboard.Instance.PresentKeyboard("Introdueix el teu nom...");
            Debug.Log("🎹 Teclat VR mostrat");
        }
        else
        {
            Debug.LogError("❌ NonNativeKeyboard.Instance és null!");
        }
    }
} 