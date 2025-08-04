using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvButton : Button
{
    [Serializable] public class ButtonPressedEvent : UnityEvent {}
    [Serializable] public class ButtonReleasedEvent : UnityEvent {}
    
    [SerializeField] private ButtonPressedEvent m_OnPressed = new ButtonPressedEvent();
    [SerializeField] private ButtonReleasedEvent m_OnReleased = new ButtonReleasedEvent();

    public ButtonPressedEvent onPressed
    {
        get => m_OnPressed;
        set => m_OnPressed = value;
    }

    public ButtonReleasedEvent onReleased
    {
        get => m_OnReleased;
        set => m_OnReleased = value;
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!IsActive() || !IsInteractable()) return;
        UISystemProfilerApi.AddMarker("ToggleButton.onPressed", this);
        m_OnPressed?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!IsActive() || !IsInteractable()) return;
        UISystemProfilerApi.AddMarker("ToggleButton.onReleased", this);
        m_OnReleased?.Invoke();
    }
}