using System.Collections;
using UnityEngine;

public class NetOutOfBounds : MonoBehaviour
{
    private NetGameplayModule _module;

    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetGameplayModule>().ModuleID != NetModuleID.Bridge) yield break;
        _module = other.gameObject.GetComponent<NetGameplayModule>();
        var i = 10f;
        while (i > 0f)
        {
            yield return new WaitForSeconds(1f);
            i--;
        }
        _module.S_InflictDamage(9999, PlayerData.PlayerID);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        StopCoroutine(OnTriggerEnter2D(other));
    }
}