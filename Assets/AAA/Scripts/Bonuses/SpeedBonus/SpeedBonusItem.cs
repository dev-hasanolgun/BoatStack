using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBonusItem : MonoBehaviour, ICollectable, IPoolable
{
    public MeshRenderer MeshRenderer;
    public Collider Collider;
    public float BoostAmount;
    public float Duration;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnCollect", new Dictionary<string, object>{{"item", this}});
        }
    }
    public void Collect(Player player)
    {
        StartCoroutine(BoostSpeed(player, BoostAmount, Duration));
    }
    private IEnumerator BoostSpeed(Player player, float boostAmount, float duration) // Boost player's speed for a certain duration
    {
        player.Speed += boostAmount;
        MeshRenderer.enabled = false;
        Collider.enabled = false;
        yield return new WaitForSeconds(duration);
        player.Speed = player.DefaulSpeed;
        MeshRenderer.enabled = true;
        Collider.enabled = true;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("speedItemPool", this);
    }
}
