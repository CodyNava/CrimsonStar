
using UnityEngine;

public class ShipEditorButton : MonoBehaviour
{
    [SerializeField] private NetModuleID netModuleID;
    [SerializeField] private ShipEditor shipEditor;

    public void ButtonClick()
    {
        shipEditor.SpawnPart(netModuleID);
    }
}

