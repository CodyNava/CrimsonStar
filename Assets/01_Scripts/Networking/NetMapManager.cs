using System;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Object;
using UnityEngine;

namespace _01_Scripts.Networking
{
    public class NetMapManager : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<NetTeamModeID, GameObject> _teamModeMaps = new();
        // private NetLobbyConductor _lobbyConductor;
        // private NetGameplayConductor _gameplayConductor;
        
        
        private void Start()
        {
            var teamModeID = InstanceFinder.GetInstance<NetLobbyConductor>().SelectedTeamMode;
            var mapPrefab = _teamModeMaps[teamModeID];
            Instantiate(mapPrefab);
        }
    }
}