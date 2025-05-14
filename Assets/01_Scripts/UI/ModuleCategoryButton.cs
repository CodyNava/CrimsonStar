using UnityEngine;

public class ModuleCategoryButton : MonoBehaviour
{
    [SerializeField] private NetModuleCategory moduleCategory;
    [SerializeField] private ModuleCategoryContainer categoryContainer;


    public void ButtonClick()
    {
        categoryContainer.SetModuleCategory(moduleCategory);
    }
}
