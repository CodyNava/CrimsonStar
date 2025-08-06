using System;
using FishNet;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace _01_Scripts.UI.ControlPanel
{
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] private Button _shipEditorBtn;
        [SerializeField] private Button _reloadBtn;

        private NetLobbyConductor _lobbyConductor;
        private NetShipEditorConductor _shipEditorConductor;
        private NetGameplayConductor _gameplayConductor;

        private void Start()
        {
            Assert.IsNotNull(_shipEditorBtn, "_shipEditorBtn != null");
            _shipEditorBtn.onClick.AddListener(OnShipEditorBtnClicked);
            
            Assert.IsNotNull(_reloadBtn, "_reloadBtn != null");
            _reloadBtn.onClick.AddListener(OnReloadBtnClicked);

            _lobbyConductor = InstanceFinder.GetInstance<NetLobbyConductor>();
            _shipEditorConductor = InstanceFinder.GetInstance<NetShipEditorConductor>();
            _gameplayConductor = InstanceFinder.GetInstance<NetGameplayConductor>();
        }

        private void OnDestroy()
        {
            _shipEditorBtn.onClick.RemoveListener(OnShipEditorBtnClicked);
            _reloadBtn.onClick.RemoveListener(OnReloadBtnClicked);
        }

        private void OnShipEditorBtnClicked()
        {
            _shipEditorConductor.MoveToScene(_gameplayConductor, _lobbyConductor.Players);
        }

        private void OnReloadBtnClicked()
        {
            _gameplayConductor.ReloadScene(_lobbyConductor.Players);
        }
    }
}