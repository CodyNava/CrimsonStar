using UnityEngine;

public class ModuleCategoryButton : MonoBehaviour
{
    [SerializeField] private NetModuleCategory moduleCategory;
    [SerializeField] private ModuleCategoryContainer categoryContainer;


    public async void ButtonClick()
    {

        await categoryContainer.SetModuleCategory(moduleCategory);
    }
}
