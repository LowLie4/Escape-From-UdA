using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VRExposureFade : MonoBehaviour
{
    [Tooltip("Transform de la cámara o XR Rig")]
    public Transform player;
    [Tooltip("Punto donde comienza el fade")]
    public Transform startPoint;
    [Tooltip("Punto donde termina (negro completo)")]
    public Transform endPoint;
    [Tooltip("Global Volume con Color Adjustments")]
    public Volume volume;
    [Tooltip("Exposición al inicio (normalmente 0)")]
    public float maxExposure = 0f;
    [Tooltip("Exposición al final (e.g. -10)")]
    public float minExposure = -10f;

    private ColorAdjustments colorAdjustments;
    private float corridorLength;

    void Start()
    {
        // Calcula la distancia total del pasillo
        corridorLength = Vector3.Distance(startPoint.position, endPoint.position);

        // Obtiene el override de Color Adjustments
        if (!volume.profile.TryGet(out colorAdjustments))
            Debug.LogError("¡No se encontró Color Adjustments en el Volume!");
    }

    void Update()
    {
        // Distancia recorrida desde el inicio
        float dist = Vector3.Distance(player.position, startPoint.position);
        float t = Mathf.Clamp01(dist / corridorLength);

        // Interpola la exposición
        colorAdjustments.postExposure.value = Mathf.Lerp(maxExposure, minExposure, t);
    }
}
