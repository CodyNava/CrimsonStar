using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleCategoryButton : MonoBehaviour
{
    [SerializeField] private NetModuleCategory moduleCategory;
    [SerializeField] private ModuleCategoryContainer categoryContainer;
    [SerializeField] private List<Button> otherCategoryButton = new List<Button>();
    public void ButtonClick()
    {
        categoryContainer.SetModuleCategory(moduleCategory);
        if (InputManager.Instance.IsGamepadUsed) return;
        this.GetComponent<Button>().interactable = true;
        foreach (var button in otherCategoryButton)
        {
            button.interactable = true;
        }
        // das disablen für den disabled sprite ist nicht mit keyboard + gamepad kompatible 
        // todo die visualisierung für den disabled state anders darstellen
    }
}
