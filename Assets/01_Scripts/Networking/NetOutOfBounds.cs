using System;
using System.Collections;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class NetOutOfBounds : NetworkBehaviour
{
    private Coroutine _destruction;
    [SerializeField] private float outOfBoundsTimer = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!InstanceFinder.IsServerStarted) return;
        if (!other.TryGetComponent(out NetGameplayModule gameplayModule)) return;
        if (gameplayModule.ModuleID != NetModuleID.Bridge) return;
        if (_destruction != null) return;
        _destruction = StartCoroutine(DestructionCoroutine(gameplayModule));
    }

    private IEnumerator DestructionCoroutine(NetGameplayModule gameplayModule)
    {
        Debug.Log("Destruction Started");
        yield return new WaitForSeconds(outOfBoundsTimer);
        gameplayModule.S_InflictDamage(9999, gameplayModule.Bridge.PlayerID);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!InstanceFinder.IsServerStarted) return;
        if (_destruction != null)
        {
            StopCoroutine(_destruction);
            _destruction = null;
        }

        Debug.Log("Stopped");
    }
}