using _01_Scripts.Networking;
using UnityEngine;

namespace _01_Scripts.Ship
{
    public class DetachedModuleSpawner : SceneSingleton<DetachedModuleSpawner>
    {
        [SerializeField] private NetDetachedModule _detachedModulePrefab;

        public NetDetachedModule SpawnDetachedModule(NetModuleID moduleID, Transform transform, Vector2 velocity)
        {
            NetDetachedModule detachedModule =
                Instantiate(_detachedModulePrefab, transform.position, transform.rotation);
            detachedModule.Initialize(moduleID, velocity);
            return detachedModule;
        }

        public NetDetachedModule SpawnDetachedModule(NetModuleID moduleID, Transform transform)
        {
            return SpawnDetachedModule(moduleID, transform, Vector2.zero);
        }
        
// #if UNITY_EDITOR
//         [SerializeField] private Vector2 _debugVelocity;
//         [SerializeField] private NetModuleID _debugModuleID;
//
//         private void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.O))
//             {
//                 for (int i = 0; i < 10; ++i)
//                 {
//                     SpawnDetachedModule(_debugModuleID, transform, Random.onUnitSphere);
//                 }
//             }
//         }
// #endif
    }
}