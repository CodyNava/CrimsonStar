using UnityEngine;
using UnityEngine.UI;

public class HowToPlayInputs : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayContainer;
    [SerializeField] private Button rules;
    [SerializeField] private GameObject rulesContainer;
    [SerializeField] private Button modules;
    [SerializeField] private GameObject modulesContainer;
    [SerializeField] private Button hazards;
    [SerializeField] private GameObject hazardsContainer;
    [SerializeField] private Button icons;
    [SerializeField] private GameObject iconsContainer;
    [SerializeField] private Button htpBack;
    
    private void Update()
    {
        if (Keybinds.Actions.UI.SwapTabLeft.WasPressedThisFrame())
        {
            if (rulesContainer.activeSelf)
            {
                icons.onClick.Invoke();
            }

            else if (modulesContainer.activeSelf)
            {
                rules.onClick.Invoke();
            }

            else if (hazardsContainer.activeSelf)
            {
                modules.onClick.Invoke();
            }

            else if (iconsContainer.activeSelf)
            {
                hazards.onClick.Invoke();
            }
        }

        if (Keybinds.Actions.UI.SwapTabRight.WasPressedThisFrame())
        {
            if (rulesContainer.activeSelf)
            {
                modules.onClick.Invoke();
            }

            else if (modulesContainer.activeSelf)
            {
                hazards.onClick.Invoke();
            }

            else if (hazardsContainer.activeSelf)
            {
                icons.onClick.Invoke();
            }

            else if (iconsContainer.activeSelf)
            {
                rules.onClick.Invoke();
            }
        }

        if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            htpBack.onClick.Invoke();
        }
    }
}
