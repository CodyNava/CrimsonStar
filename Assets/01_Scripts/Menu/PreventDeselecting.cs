using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreventDeselecting : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject lastSelected;
    private void Reset()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
        lastSelected = eventSystem.firstSelectedGameObject;
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject && lastSelected != eventSystem.currentSelectedGameObject)
            lastSelected = eventSystem.currentSelectedGameObject;
        
        if (!eventSystem.currentSelectedGameObject && lastSelected)
            eventSystem.SetSelectedGameObject(lastSelected);
    }
}

