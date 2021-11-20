using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public int Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent("OnBlocked", new Dictionary<string, object>{{"obstacle", this}});
        }
    }
}
