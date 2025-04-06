using UnityEngine;

public class PivotChanger : MonoBehaviour
{
    public GameObject porta; // Asigna tu objeto en el inspector
    private GameObject newPivot;

    // Variables para controlar la rotación
    private float rotatedAngle = 0f;       // Ángulo acumulado
    public float targetAngle = -120f;       // Ángulo total deseado
    public float rotationSpeed = 20f;      // Velocidad de rotación (grados por segundo)

    void Start()
    {
        if (porta == null) return;

        // Crear un nuevo objeto vacío en la posición deseada
        newPivot = new GameObject("NewPivot");
        newPivot.transform.position = porta.transform.position;
        newPivot.transform.rotation = porta.transform.rotation;

        // Hacer que "porta" sea hijo del nuevo pivote
        porta.transform.SetParent(newPivot.transform);
    }

    void Update()
    {
        // Rotar solo si no se ha alcanzado el ángulo objetivo
        if (newPivot != null && rotatedAngle < targetAngle)
        {
            // Calcular la rotación de este frame
            float rotationThisFrame = rotationSpeed * Time.deltaTime;

            // Evitar que se exceda el ángulo objetivo
            if (rotatedAngle + rotationThisFrame > targetAngle)
            {
                rotationThisFrame = targetAngle - rotatedAngle;
            }

            // Aplicar la rotación
            newPivot.transform.Rotate(-Vector3.up * rotationThisFrame);
            rotatedAngle += rotationThisFrame;
        }
    }
}
