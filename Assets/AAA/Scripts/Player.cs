using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3[] Waypoints;
    public Vector3[] Rotations;
    public int BoatAmount;
    public bool IsSliding;
    
    private void StartSliding(Dictionary<string,object> message)
    {
        IsSliding = true;
        StartCoroutine(Move());
    }
    
    private IEnumerator Move()
    {
        var targetIndex = 0;
        
        while (true)
        {
            var pos = transform.position;
            var rot = transform.rotation;
            
            if (Vector3.Distance(pos, Waypoints[targetIndex]) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= Waypoints.Length)
                {
                    EventManager.TriggerEvent("OnLevelFinish", null);
                    yield break;
                }
            }
            
            if (!IsSliding) yield break;
            
            transform.position = Vector3.MoveTowards(pos, Waypoints[targetIndex], Time.deltaTime);
            transform.rotation = Quaternion.Lerp(rot, Quaternion.LookRotation(Rotations[targetIndex]), Time.deltaTime * 10f);
            yield return null;
        }
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("OnLevelStart", StartSliding);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnLevelStart", StartSliding);
    }
}
