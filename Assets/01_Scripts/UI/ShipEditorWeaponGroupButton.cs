using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShipEditorWeaponGroupButton : MonoBehaviour
{
    private float SizeIncrease => shipWeaponGroupManager.buttonSelectedSizeIncrease;

    [SerializeField] private ShipEditorWeaponGroups shipWeaponGroupManager;
    [SerializeField] public bool buttonToggle;
    [SerializeField] public int buttonID;
    [SerializeField] private Vector2 rectVector;
    [SerializeField] private RectTransform rectTransform;
    private int _indexGp;

    public void Awake()
    {
        shipWeaponGroupManager.weaponGroupButtons.Add(this);
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectVector = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        _indexGp = 1;
        if (buttonToggle) ChangeToSelected();
    }

    public void OnEnable()
    {
        Keybinds.Actions.ShipEditor.WeaponGroupSelect.performed += OnKeyPress;
        Keybinds.Actions.ShipEditor.WeaponGroupSelectGP.performed += OnTriggerPress;
    }

    public void OnDisable()
    {
        Keybinds.Actions.ShipEditor.WeaponGroupSelect.performed -= OnKeyPress;
        Keybinds.Actions.ShipEditor.WeaponGroupSelectGP.performed -= OnTriggerPress;
    }

    private void OnTriggerPress(InputAction.CallbackContext input)
    {
        if (input.performed && _indexGp >= 1 || _indexGp <= 3)
        {
            switch (input.control.name)
            {
                case "leftTrigger":
                    _indexGp--;
                    break;
                case "rightTrigger":
                    _indexGp++;
                    break;
            }
            if (_indexGp > 3) _indexGp = 3;
            if (_indexGp < 1) _indexGp = 1;
            if (buttonID == _indexGp)
            {
                ChangeToSelected();
            }
        }
    }

    private void OnKeyPress(InputAction.CallbackContext input)
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
        buttonToggle = true;
        shipWeaponGroupManager.DeselectButtonsExcept(this);
        this.GetComponent<Button>().interactable = false;
        rectTransform.sizeDelta = rectVector * SizeIncrease;
        shipWeaponGroupManager.SetWeaponGroup(buttonID);
    }

    public void ChangeToUnselected()
    {
        if (!buttonToggle) return;
        buttonToggle = false;
        this.GetComponent<Button>().interactable = true;
        rectTransform.sizeDelta = rectVector;
    }
}