using Firebase.Database; // o Firebase.Firestore
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Auth;
using Firebase;

public class ScoreUploader : MonoBehaviour
{
    private DatabaseReference _dbRef;
    private System.Random _random; // Instancia de Random

    // Singleton per accedir des d'altres scripts
    public static ScoreUploader Instance { get; private set; }

    private void Awake()
    {
        // Implementar singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        // 1️⃣ Comprova dependències i inicialitza Firebase
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase no disponible: {dependencyStatus}");
            return;
        }

        // 2️⃣ Anonim login
        var authTask = FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        await authTask;
        if (authTask.Exception != null)
        {
            Debug.LogError($"Error sign-in: {authTask.Exception}");
            return;
        }
        Debug.Log("Sign-in anònim satisfactori.");

        // 3️⃣ Prepara la referència a Realtime Database
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Inicializa la instancia de Random
        _random = new System.Random();

        // 4️⃣ Envia dades de prova
        UploadTestData();
    }

    private void UploadTestData()
    {
        // Exemple de node de prova "testRun" amb un timestamp i un valor aleatori
        string key = _dbRef.Child("testRun").Push().Key;
        var testData = new Dictionary<string, object>
        {
            ["valor"] = _random.Next(1, 101),
            ["timestamp"] = ServerValue.Timestamp
        };

        _dbRef.Child("testRun").Child(key)
            .SetValueAsync(testData)
            .ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                    Debug.Log("✅ Dades de prova pujat correctament!");
                else
                    Debug.LogError($"❌ Error pujant dades de prova: {task.Exception}");
            });
    }

    // ===== SISTEMA DE PUNTUACIONS =====

    /// <summary>
    /// Puja una puntuació a la base de dades
    /// </summary>
    /// <param name="tempsSegons">Temps en segons que ha trigat el jugador</param>
    public void PujarPuntuacio(float tempsSegons)
    {
        if (_dbRef == null)
        {
            Debug.LogError("❌ Firebase no està inicialitzat!");
            return;
        }

        // Generar nom de jugador aleatori
        string nomJugador = GenerarNomJugador();
        
        // Formatar el temps
        string tempsFormatat = FormatarTemps(tempsSegons);

        // Crear les dades de la puntuació amb Dictionary
        string key = _dbRef.Child("puntuacions").Push().Key;
        var puntuacioData = new Dictionary<string, object>
        {
            ["nom"] = nomJugador,
            ["temps"] = tempsSegons,
            ["tempsFormatat"] = tempsFormatat,
            ["timestamp"] = ServerValue.Timestamp
        };

        // Pujar a Firebase
        _dbRef.Child("puntuacions").Child(key)
            .SetValueAsync(puntuacioData)
            .ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log($"✅ Puntuació pujada correctament: {nomJugador} - {tempsFormatat}");
                }
                else
                {
                    Debug.LogError($"❌ Error pujant puntuació: {task.Exception}");
                }
            });
    }

    /// <summary>
    /// Genera un nom de jugador aleatori del tipus "Jugador01", "Jugador02", etc.
    /// </summary>
    private string GenerarNomJugador()
    {
        int numeroAleatori = _random.Next(1, 100); // Del 1 al 99
        return $"Jugador{numeroAleatori:00}"; // Format amb zeros: Jugador01, Jugador02, etc.
    }

    /// <summary>
    /// Formata el temps en segons a format MM:SS
    /// </summary>
    private string FormatarTemps(float tempsSegons)
    {
        int minuts = Mathf.FloorToInt(tempsSegons / 60);
        int segons = Mathf.FloorToInt(tempsSegons % 60);
        return $"{minuts:00}:{segons:00}";
    }

    // Mètode per testing - pujar puntuació de prova
    [ContextMenu("Pujar Puntuació de Prova")]
    public void PujarPuntuacioProva()
    {
        float tempsAleatori = _random.Next(60, 600); // Entre 1 i 10 minuts
        PujarPuntuacio(tempsAleatori);
    }
}
