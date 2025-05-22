using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FMODButtonAudio : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference buttonHover;
    [SerializeField] private FMODUnity.EventReference buttonPress;
    [SerializeField] private FMODUnity.EventReference buttonDisabled;

    [SerializeField] private Button button;

    public void OnPress()
    {
        if (button.interactable)
        {
            FMODUnity.RuntimeManager.PlayOneShot(buttonPress, transform.position);
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(buttonDisabled, transform.position);
        }
    }

    public void OnHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonHover, transform.position);
    }
}