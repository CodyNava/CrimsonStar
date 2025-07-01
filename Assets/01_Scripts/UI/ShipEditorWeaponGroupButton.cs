using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShipEditorWeaponGroupButton : MonoBehaviour
{
    private float SizeIncrease => shipWeaponGroupManager.buttonSelectedSizeIncrease;
    private Color SelectedColor => shipWeaponGroupManager.selectedColor;
    private Color DeselectedColor => shipWeaponGroupManager.deSelectedColor;

    [SerializeField] private ShipEditorWeaponGroups shipWeaponGroupManager;
    [SerializeField] public bool buttonToggle;
    [SerializeField] public int buttonID;
    [SerializeField] private Image buttonImageColorRef;
    [SerializeField] private Vector2 rectVector;
    [SerializeField] private RectTransform rectTransform;

    public void Awake()
    {
        shipWeaponGroupManager.weaponGroupButtons.Add(this);
        buttonImageColorRef.color = DeselectedColor;
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectVector = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        if (buttonToggle) ChangeToSelected();
    }
    public void OnEnable()
    {
        Keybinds.Actions.ShipEditor.WeaponGroupSelect.performed += OnKeyPress;
    }

    public void OnDisable()
    {
        Keybinds.Actions.ShipEditor.WeaponGroupSelect.performed -= OnKeyPress;
    }

    public void OnKeyPress(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            int keyValue = 0;
            switch (input.control.name)
            {
                case "1":
                    keyValue = 1;
                    break;
                case "2":
                    keyValue = 2;
                    break;
                case "3":
                    keyValue = 3;
                    break;
            }

            if (buttonID == keyValue)
            {
                ChangeToSelected();
                
            }
        }
    }

    private void ChangeToSelected()
    {
        shipWeaponGroupManager.DeselectButtonsExcept(this);
        buttonToggle = true;
        rectTransform.sizeDelta = rectVector * SizeIncrease;
        buttonImageColorRef.color = SelectedColor;
        shipWeaponGroupManager.SetWeaponGroup(buttonID);
        shipWeaponGroupManager.ChangeMaskForEachGroup(buttonID);
    }

    public void ChangeToUnselected()
    {
        if (!buttonToggle) return;
        buttonToggle = false;
        rectTransform.sizeDelta = rectVector / SizeIncrease;
        buttonImageColorRef.color = DeselectedColor;
    }
}