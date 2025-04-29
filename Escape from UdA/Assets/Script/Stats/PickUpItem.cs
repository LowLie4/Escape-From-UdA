using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// 1️⃣ Definir aquí los tipos disponibles:
public enum ItemType
{
    USB,
    Resistencia,
    Battery,
    Cable,
    // …añade los que necesites
}

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class PickupItem : MonoBehaviour
{
    [Header("Analytics ID")]
    [Tooltip("Identificador único para Stats (ej. “USB1”, “ResistorA”).")]
    public string objectID;

    [Header("Tipo de objeto")]
    [Tooltip("Selecciona el tipo para controlar notificaciones únicas.")]
    public ItemType objectType;

    // Lleva el registro de tipos ya notificados
    private static HashSet<ItemType> pickedTypes = new HashSet<ItemType>();

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        // Usar el nuevo evento selectEntered en vez de onSelectEntered
        grabInteractable.selectEntered.AddListener(OnPickEvent);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnPickEvent);
    }

    private void OnPickEvent(SelectEnterEventArgs args)
    {
        // Si ya registramos este tipo, salimos
        if (pickedTypes.Contains(objectType))
            return;

        // Primera vez: registramos y deshabilitamos
        pickedTypes.Add(objectType);
        StatsManager.Instance.RecordPickup(objectID);
        enabled = false;
    }
}
