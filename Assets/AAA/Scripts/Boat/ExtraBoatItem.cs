using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBoatItem : MonoBehaviour, ICollectable, IPoolable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnCollect", new Dictionary<string, object>{{"item", this}});
        }
    }

    public void Collect(Player player)
    {
        player.BoatAmount++;
        var boat = PoolManager.Instance.GetObjectFromPool("boatPool", player.Boat);
        boat.transform.position = player.BoatList[^1].transform.position;
        boat.transform.rotation = Quaternion.identity;
        boat.transform.SetParent(player.Child);
        
        for (int i = 0; i < player.BoatList.Count; i++)
        {
            player.BoatList[i].transform.localPosition += Vector3.up * player.Boat.transform.localScale.y;
        }

        player.Collider.height += player.Boat.transform.localScale.y;
        player.Collider.center += Vector3.up * player.Boat.transform.localScale.y/2f;
        //obj.Joint.connectedBody = BoatList[^1].Rb;
        player.BoatList.Add(boat);
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("extraBoatPool", this);
    }
}
