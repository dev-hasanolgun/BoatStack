using System.Collections.Generic;
using UnityEngine;

public class PointBonusItem : MonoBehaviour, ICollectable, IPoolable
{
    public float BonusPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnCollect", new Dictionary<string, object>{{"item", this}});
        }
    }

    public void Collect(Player player)
    {
        player.CurrentScore += BonusPoints;
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("pointItemPool", this);
    }
}
