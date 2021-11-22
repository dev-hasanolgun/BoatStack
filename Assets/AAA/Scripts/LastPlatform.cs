using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPlatform : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnLastPlatform", new Dictionary<string, object>{{"endPoint", transform.position}});
        }
    }
}
