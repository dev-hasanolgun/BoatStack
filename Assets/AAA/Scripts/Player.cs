using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WaterSlideData WaterSlideData;
    public CapsuleCollider Collider;
    public Transform Child;
    public Transform CharacterVisual;
    public Boat Boat;
    public List<Boat> BoatList = new List<Boat>();
    public Vector3 EndPoint;
    public int BoatAmount = 1;
    public int TargetIndex;
    public float CurrentScore;
    public float Speed = 1f;
    public float HorizontalSpeed = 5f;
    public float Offset;
    public bool IsSliding;

    private List<Boat> _tempBoatList = new List<Boat>();
    private float _finalTimer;
    private float _mousePosX;

    public void ResetStats()
    {
        for (int i = 0; i < BoatList.Count; i++)
        {
            BoatList[i].gameObject.SetActive(false);
        }
        BoatList.Clear();
        
        var boat = PoolManager.Instance.GetObjectFromPool("boatPool", Boat);
        boat.transform.position = Child.transform.position;
        boat.transform.localRotation = Quaternion.identity;
        boat.transform.SetParent(Child);
        
        BoatList.Add(boat);
        
        Collider.height = Boat.transform.localScale.y;
        Collider.center = Vector3.zero;
        Child.transform.localRotation = Quaternion.identity;
        CharacterVisual.transform.localPosition = Vector3.zero + Vector3.up*0.15f;
        CharacterVisual.transform.localRotation = Quaternion.identity;

        BoatAmount = 1;
        TargetIndex = 0;
        Speed = 1f;
        Offset = 0f;
        IsSliding = false;
    }
    public void CharacterMovement()
    {
        var waypoints = WaterSlideData.LocalPoints;
        var density = WaterSlideData.Density;
        var depth = WaterSlideData.Depth;
        var width = WaterSlideData.Width;
        var targetIndex = TargetIndex;
        
        var vel = Mathf.SmoothStep(0, 1, Mathf.Abs(Offset) / ((density - 1) / 2f + density/2f));
        var height = Vector3.up * (vel * depth * 2f);
        var radius = (Mathf.Pow(width+1f, 2f) + 4f * Mathf.Pow(depth, 2f)) / (8f * depth);
        var dir = waypoints[targetIndex] + Vector3.up * radius - Child.transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            _mousePosX = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < _mousePosX)
            {
                _mousePosX = Input.mousePosition.x;
                Offset += Time.deltaTime*HorizontalSpeed*5f;
                Child.transform.position -= Child.transform.right * (Time.deltaTime * HorizontalSpeed);
            }

            if (Input.mousePosition.x > _mousePosX)
            {
                _mousePosX = Input.mousePosition.x;
                Offset -= Time.deltaTime*HorizontalSpeed*5f;
                Child.transform.position += Child.transform.right * (Time.deltaTime * HorizontalSpeed);
            }
        }
        // if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     Offset += Time.deltaTime*HorizontalSpeed;
        //     Child.transform.position -= Child.transform.right * Time.deltaTime;
        // }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     Offset -= Time.deltaTime*HorizontalSpeed;
        //     Child.transform.position += Child.transform.right * Time.deltaTime;
        // }
        Child.transform.position = new Vector3(Child.transform.position.x, height.y+0.1f, Child.transform.position.z);
        Child.transform.rotation = Quaternion.LookRotation(Child.transform.forward,dir.normalized);
    }
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

    public void EndPlatformMovement()
    {
        if (BoatAmount > 0)
        {
            transform.position += transform.forward * (Time.deltaTime * Speed);
        }
        else
        {
            var pos = transform.position;
            var targetPos = new Vector3(EndPoint.x, pos.y, EndPoint.z);
            transform.position = Vector3.MoveTowards(pos, targetPos, Time.deltaTime * Speed);
            if (transform.position == targetPos)
            {
                for (int i = 0; i < _tempBoatList.Count; i++)
                {
                    _tempBoatList[i].gameObject.SetActive(false);
                }
                _tempBoatList.Clear();
            }
        }
    }
    private void TakeDamage(Dictionary<string,object> message)
    {
        var obstacle = (Obstacle) message["obstacle"];
        if (BoatAmount > 0)
        {
            for (int j = 0; j < obstacle.Damage; j++)
            {
                var scale = Boat.transform.localScale.y;
                BoatList[^1].gameObject.SetActive(false);
                BoatList.RemoveAt(BoatList.Count-1);
                CharacterVisual.transform.localPosition -= Vector3.up * scale;
                for (int i = 0; i < BoatList.Count; i++)
                {
                    BoatList[i].transform.localPosition -= Vector3.up * scale;
                }
                Collider.height -= Boat.transform.localScale.y;
                Collider.center -= Vector3.up * scale/2f;
                BoatAmount--;
            }
        }
    }

    private void CountBoat(Dictionary<string,object> message)
    {
        EndPoint = (Vector3) message["endPoint"];
        if (BoatAmount > 0)
        {
            var scale = Boat.transform.localScale.y;
            BoatList[^1].gameObject.transform.SetParent(null);
            _tempBoatList.Add(BoatList[^1]);
            BoatList.RemoveAt(BoatList.Count-1);
            BoatAmount--;
        }
    }

    private void CountRemainingBoats(Dictionary<string, object> message)
    {
        EndPoint = (Vector3) message["endPoint"];
        if (BoatAmount > 0)
        {
            _finalTimer += Time.deltaTime;
            Speed = 0f;
            
            if (_finalTimer > 0.3f)
            {
                var scale = Boat.transform.localScale.y;
                BoatList[^1].gameObject.SetActive(false);
                BoatList.RemoveAt(BoatList.Count-1);
                for (int i = 0; i < BoatList.Count; i++)
                {
                    BoatList[i].transform.localPosition -= Vector3.up * scale;
                }
                CharacterVisual.transform.localPosition -= Vector3.up * scale;
                BoatAmount--;
                _finalTimer = 0f;
            }
        }
        else
        {
            Speed = 1;
        }
    }
    private void CollectItem(Dictionary<string, object> message)
    {
        var item = (ICollectable) message["item"];
        item.Collect(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            var obj = Instantiate(Boat, BoatList[^1].transform.position, Quaternion.identity,Child.transform);
            for (int i = 0; i < BoatList.Count; i++)
            {
                BoatList[i].transform.localPosition += Vector3.up * Boat.transform.localScale.y;
            }
            //obj.Joint.connectedBody = BoatList[^1].Rb;
            BoatList.Add(obj);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Destroy(BoatList[^1].gameObject);
            BoatList.RemoveAt(BoatList.Count-1);
            for (int i = 0; i < BoatList.Count; i++)
            {
                BoatList[i].transform.localPosition -= Vector3.up * Boat.transform.localScale.y;
            }
        }
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("OnLevelStart", StartSliding);
        EventManager.StartListening("OnBlocked", TakeDamage);
        EventManager.StartListening("OnCollect", CollectItem);
        EventManager.StartListening("OnEndPlatform", CountBoat);
        EventManager.StartListening("OnLastPlatform", CountRemainingBoats);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnLevelStart", StartSliding);
        EventManager.StopListening("OnBlocked", TakeDamage);
        EventManager.StopListening("OnCollect", CollectItem);
        EventManager.StopListening("OnEndPlatform", CountBoat);
        EventManager.StopListening("OnLastPlatform", CountRemainingBoats);
    }
}
