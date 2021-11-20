using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController Child;
    public WaterSlideData WaterSlideData;
    public int BoatAmount = 1;
    public int TargetIndex;
    public float Speed = 1f;
    public bool IsSliding;
    
    private void StartSliding(Dictionary<string,object> message)
    {
        IsSliding = true;
        StartCoroutine(Move());
    }
    
    private IEnumerator Move()
    {
        var waypoints = WaterSlideData.LocalPoints;
        var rotations = WaterSlideData.LocalTangents;
        TargetIndex = 0;
        
        while (true)
        {
            var pos = transform.position;
            var vec = waypoints[TargetIndex] - pos;
            var nor = Vector3.up;
            var cross = Vector3.Cross(vec.normalized, nor);
            cross = Quaternion.AngleAxis(90, Vector3.up) * cross;
            var plane = new Plane(cross, waypoints[TargetIndex]);
            
            if (Vector3.Distance(pos, plane.ClosestPointOnPlane(pos)) < 0.1f)
            {
                TargetIndex++;
                if (TargetIndex >= waypoints.Length)
                {
                    EventManager.TriggerEvent("OnLevelFinish", null);
                    yield break;
                }
            }
            
            if (!IsSliding) yield break;
            
            transform.position = Vector3.MoveTowards(pos, waypoints[TargetIndex], Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotations[TargetIndex]), Time.deltaTime * 5f);
            yield return null;
        }
    }
    private void TakeDamage(Dictionary<string,object> message)
    {
        var obstacle = (Obstacle) message["obstacle"];
        BoatAmount -= obstacle.Damage;
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("OnLevelStart", StartSliding);
        EventManager.StartListening("OnBlocked", StartSliding);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnLevelStart", StartSliding);
        EventManager.StopListening("OnBlocked", StartSliding);
    }
}
