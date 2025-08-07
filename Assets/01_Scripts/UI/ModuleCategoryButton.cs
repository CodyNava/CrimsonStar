using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleCategoryButton : MonoBehaviour
{
    [SerializeField] private NetModuleCategory moduleCategory;
    [SerializeField] private ModuleCategoryContainer categoryContainer;
    [SerializeField] private List<Button> otherCategoryButton = new List<Button>();
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite originalButtonSprite, selectedButtonSprite, highlightedButtonSprite;

    private bool _selected;

    private void Start()
    {
        buttonImage = GetComponent<Button>().image;
        _selected = false;
    }

    public void ButtonClick()
    {
        categoryContainer.SetModuleCategory(moduleCategory);
        buttonImage.sprite = selectedButtonSprite;
        _selected = true;
        foreach (var button in otherCategoryButton)
        {
            var otherButtons = button.GetComponent<ModuleCategoryButton>();
            button.image.sprite = otherButtons.originalButtonSprite;
            otherButtons._selected = false;
        }
        
    }

    public void EnableButtonHighLight()
    {
        if (_selected) return;
        buttonImage.sprite = highlightedButtonSprite;
    }
    
    public void DisableButtonHighLight()
    {
        if (_selected) return;
        buttonImage.sprite = originalButtonSprite;
    }
}
