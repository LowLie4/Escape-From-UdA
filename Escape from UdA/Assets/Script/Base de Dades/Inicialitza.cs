using Firebase;
using Firebase.Database;      // o Firebase.Firestore
using Firebase.Auth;          // si fas auth
using UnityEngine;
using System.Collections.Generic;

public class Inicialitza : MonoBehaviour
{
    private DatabaseReference _dbRef;
    private System.Random _random; // Instancia de Random

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
            { "valor", Random.Range(1, 100) },
            { "timestamp", ServerValue.Timestamp }
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
}
