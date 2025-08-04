using System.Collections;
using UnityEngine;

public class NetOutOfBounds : MonoBehaviour
{
    private NetGameplayModule _module;
    private Coroutine _destruction;
    private bool _destructionTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetGameplayModule>().ModuleID != NetModuleID.Bridge)
        {
            return;
        }
        _destruction = StartCoroutine(DestructionCoroutine(other));
    }

    private IEnumerator DestructionCoroutine(Collider2D other)
    {
        Debug.Log("Destruction Started");
        _module = other.gameObject.GetComponent<NetGameplayModule>();
        var i = 10f;
        while (i > 0f)
        {
            yield return new WaitForSeconds(1f);
            i--;
        }
        _destructionTriggered = true;
        _module.S_InflictDamage(9999, _module.Bridge.PlayerID);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_destructionTriggered) return;
        if (_destruction != null)
        {
            StopCoroutine(_destruction);
            _destruction = null;
        }
        Debug.Log("Stopped");
        _destructionTriggered = false;
    }
}   