using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WindowReveal : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la cabeza / cámara del jugador (VR rig).")]
    public Transform cameraTransform;
    [Tooltip("Collider que define la ventana")]
    public Collider windowCollider;
    [Tooltip("LayerMask que incluya los colliders de los pilares")]
    public LayerMask pillarsLayer;
    [Tooltip("Material completamente negro")]
    public Material blackMaterial;
    [Tooltip("Material con la imagen que quieres revelar")]
    public Material imageMaterial;

    MeshRenderer _renderer;
    float _windowDistance;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        // calculamos la distancia inicial a la ventana (por si la ventana no está siempre fija)
        _windowDistance = Vector3.Distance(cameraTransform.position, windowCollider.bounds.center);
        // arrancamos con negro
        _renderer.material = blackMaterial;
    }

    void Update()
    {
        // lanzamos un rayo desde la cámara hacia el centro de la ventana
        Vector3 origin = cameraTransform.position;
        Vector3 dir = (windowCollider.bounds.center - origin).normalized;

        // recogemos TODOS los hits hasta la distancia de la ventana
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, _windowDistance, ~0, QueryTriggerInteraction.Ignore);
        // ordenamos por distancia
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool blockedByPillar = false;
        bool hitWindow = false;

        foreach (var h in hits)
        {
            // si es la ventana, marcamos que la hemos alcanzado
            if (h.collider == windowCollider)
            {
                hitWindow = true;
                break;
            }
            // si antes aparece un pilar, está bloqueada
            if (((1 << h.collider.gameObject.layer) & pillarsLayer) != 0)
            {
                blockedByPillar = true;
                break;
            }
        }

        // si alcanzamos la ventana y NO estaba bloqueada por ningún pilar → revelamos la imagen
        if (hitWindow && !blockedByPillar)
            _renderer.material = imageMaterial;
        else
            _renderer.material = blackMaterial;
    }
}
