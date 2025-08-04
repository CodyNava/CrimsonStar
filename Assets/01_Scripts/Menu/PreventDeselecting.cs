using UnityEngine;
using UnityEngine.EventSystems;

public class PreventDeselecting : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private InputManager inputManager;
    private void Reset()
    {
        eventSystem = EventSystem.current;
        lastSelected = eventSystem.firstSelectedGameObject;
    }

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
    }

    private void Update()
    {
        if (inputManager.IsGamepadUsed)
        {
            SetSelected();
        }
        else
        {
            // eventSystem.SetSelectedGameObject(null);
        }
    }

    private void SetSelected()
    {
        if (eventSystem.currentSelectedGameObject && lastSelected != eventSystem.currentSelectedGameObject)
            lastSelected = eventSystem.currentSelectedGameObject;
            
        if (!eventSystem.currentSelectedGameObject && lastSelected)
            eventSystem.SetSelectedGameObject(lastSelected);
    }
}

