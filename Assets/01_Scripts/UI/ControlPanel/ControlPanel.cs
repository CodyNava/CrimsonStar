using System;
using FishNet;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace _01_Scripts.UI.ControlPanel
{
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] private Button _shipEditorBtn;

        public void Awake()
        {
            Assert.IsNotNull(_shipEditorBtn, "_shipEditorBtn != null");
            _shipEditorBtn.onClick.AddListener(OnShipEditorBtnClicked);
        }

        private void OnShipEditorBtnClicked()
        {
            var lobbyConductor = InstanceFinder.GetInstance<NetLobbyConductor>();
            var gameplayConductor = InstanceFinder.GetInstance<NetGameplayConductor>();
            InstanceFinder.GetInstance<NetShipEditorConductor>().MoveToScene(gameplayConductor, lobbyConductor.Players);
        }
    }
}