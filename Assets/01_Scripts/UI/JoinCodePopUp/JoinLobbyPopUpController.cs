using System;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyPopUpController : MonoBehaviour
{
    [SerializeField] private Button _closePopUpBtn;

    [SerializeField] private GameObject _backgroundObject;
    [SerializeField] private JoinLobbyController _popupContainerObject;

    private void OnEnable()
    {
        _closePopUpBtn.onClick.AddListener(OnCloseBtnClicked);
        _popupContainerObject.ClearInputField();
    }

    private void OnDisable()
    {
        _closePopUpBtn.onClick.RemoveListener(OnCloseBtnClicked);
    }

    private void OnCloseBtnClicked()
    {
        _backgroundObject.SetActive(false);
        _popupContainerObject.gameObject.SetActive(false);
    }
}
