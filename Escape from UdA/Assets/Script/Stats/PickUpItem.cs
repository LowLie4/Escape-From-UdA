using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string objectID; // ex. "USB1"

    private void Start()
    {
        // Assegura’t que està actiu al començament
        enabled = true;
    }

    public void OnPick()
    {
        // 1️⃣ Registra la recollida només la primera vegada
        StatsManager.Instance.RecordPickup(objectID);

        // 2️⃣ Deshabilita aquest script perquè no torni a enviar la notificació
        enabled = false;

        
    }
}
