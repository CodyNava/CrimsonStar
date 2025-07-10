using System;
using UnityEngine;
using UnityEngine.UI;

public class Ready : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    private void Update()
    {
        if (Keybinds.Actions.UI.PauseGame.WasPressedThisFrame() && !PlayerData.IsLobbyHost)
        {
            readyButton.onClick.Invoke();
        }

        if (Keybinds.Actions.UI.PauseGame.WasPressedThisFrame() && PlayerData.IsLobbyHost)
        {
            startButton.onClick.Invoke();
        }
    }
}

