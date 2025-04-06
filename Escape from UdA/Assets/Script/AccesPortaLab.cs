using UnityEngine;

public class PivotChanger : MonoBehaviour
{
    public GameObject porta; // Asigna tu objeto en el inspector
    private GameObject newPivot;

    // Variables para controlar la rotaci�n
    private float rotatedAngle = 0f;       // �ngulo acumulado
    public float targetAngle = -120f;       // �ngulo total deseado
    public float rotationSpeed = 20f;      // Velocidad de rotaci�n (grados por segundo)

    void Start()
    {
        if (porta == null) return;

        // Crear un nuevo objeto vac�o en la posici�n deseada
        newPivot = new GameObject("NewPivot");
        newPivot.transform.position = porta.transform.position;
        newPivot.transform.rotation = porta.transform.rotation;

        // Hacer que "porta" sea hijo del nuevo pivote
        porta.transform.SetParent(newPivot.transform);
    }

    void Update()
    {
        // Rotar solo si no se ha alcanzado el �ngulo objetivo
        if (newPivot != null && rotatedAngle < targetAngle)
        {
            // Calcular la rotaci�n de este frame
            float rotationThisFrame = rotationSpeed * Time.deltaTime;

            // Evitar que se exceda el �ngulo objetivo
            if (rotatedAngle + rotationThisFrame > targetAngle)
            {
                rotationThisFrame = targetAngle - rotatedAngle;
            }

            // Aplicar la rotaci�n
            newPivot.transform.Rotate(-Vector3.up * rotationThisFrame);
            rotatedAngle += rotationThisFrame;
        }
    }
}
