using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Button backButton;
    
    private void Update()
    {
        if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            backButton.onClick.Invoke();
        }
    }
}
