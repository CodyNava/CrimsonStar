using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HowToPlayInputs : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayContainer;
    [SerializeField] private Button howToPlay;
    [SerializeField] private GameObject htpContainer;
    [SerializeField] private Button editor;
    [SerializeField] private GameObject editorContainer;
    [SerializeField] private Button hazards;
    [SerializeField] private GameObject hazardsContainer;
    [SerializeField] private Button icons;
    [SerializeField] private GameObject iconsContainer;
    [SerializeField] private Button htpBack;

    [SerializeField] private List<GameObject> buttonPrompts;
    
    private void Update()
    {
        foreach (var prompt in buttonPrompts)
        {
            prompt.SetActive(InputManager.Instance.IsGamepadUsed);
        }
        if (Keybinds.Actions.UI.SwapTabLeft.WasPressedThisFrame())
        {
            
            if (htpContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                icons.onClick.Invoke();
            }
            
            else if (editorContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                howToPlay.onClick.Invoke();
            }

            else if (hazardsContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                editor.onClick.Invoke();
            }

            else if (iconsContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                hazards.onClick.Invoke();
            }
        }

        if (Keybinds.Actions.UI.SwapTabRight.WasPressedThisFrame())
        {
            if (htpContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                editor.onClick.Invoke();
            }
            
            else if (editorContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
               hazards.onClick.Invoke();
            }


            else if (hazardsContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                icons.onClick.Invoke();
            }

            else if (iconsContainer.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                howToPlay.onClick.Invoke();
            }
        }

        if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            htpBack.onClick.Invoke();
        }
    }
}
