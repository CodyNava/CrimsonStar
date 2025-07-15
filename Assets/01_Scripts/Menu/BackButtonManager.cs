using System;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonManager : MonoBehaviour
{
   [SerializeField] private Button backButton;

   private void Update()
   {
      if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame() && gameObject.activeSelf)
      {
         backButton.onClick.Invoke();
      }
   }
}
