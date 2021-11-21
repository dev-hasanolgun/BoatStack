using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IPoolable
{
    public Rigidbody Rb;
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("boatPool", this);
    }
}
