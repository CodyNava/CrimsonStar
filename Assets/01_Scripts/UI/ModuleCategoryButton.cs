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
        this.GetComponent<Button>().interactable = false;
        foreach (var button in otherCategoryButton)
        {
            button.interactable = true;
        }
    }
}
