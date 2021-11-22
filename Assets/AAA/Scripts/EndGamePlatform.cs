using System.Collections.Generic;
using UnityEngine;

public class EndGamePlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnEndPlatform", new Dictionary<string, object>{{"endPoint", transform.position}});
        }
    }
}
