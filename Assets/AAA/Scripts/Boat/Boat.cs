using UnityEngine;

public class Boat : MonoBehaviour, IPoolable
{
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("boatPool", this);
    }
}
