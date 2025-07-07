using UnityEngine;
using UnityEngine.UI;

public class CreditsButton : MonoBehaviour
{
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private Button creditsBackButton;

    private void Update()
    {
        if (creditsCanvas.activeSelf)
        {
            if (Keybinds.Actions.UI.Cancel.WasPerformedThisFrame())
            {
                creditsBackButton.onClick.Invoke();
            }
        }
    }
}
