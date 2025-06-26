using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class FMODButtonAudio : MonoBehaviour
{
    [SerializeField] private EventReference buttonHover;
    [SerializeField] private EventReference buttonPress;
    [SerializeField] private EventReference buttonDisabled;
    [SerializeField] private string parameterName;
    [SerializeField] private float value;
    private EventInstance eventInstance;

    [SerializeField] private Button button;

    public void OnPress()
    {
        if (button.interactable)
        {
            RuntimeManager.PlayOneShot(buttonPress, transform.position);
            eventInstance.setParameterByName(parameterName, value);
        }
        else
        {
            RuntimeManager.PlayOneShot(buttonDisabled, transform.position);
            eventInstance.setParameterByName(parameterName, value);
        }
    }

    public void OnHover()
    {
        RuntimeManager.PlayOneShot(buttonHover, transform.position);
        eventInstance.setParameterByName(parameterName, value);
    }
}