using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public WaterSlideData WaterSlideData; // Slide data which includes local points, normals, tangents with width, depth etc. sizes
    public CapsuleCollider Collider; // Collider of the player
    public Transform Child; // Child object to control rotations and horizontal movement along the curve
    public Transform CharacterVisual; // Character visual up on the boat
    public Boat Boat; // Boat Prefab
    public List<Boat> BoatList = new List<Boat>(); // Current boat list that player has
    public Vector3 EndPoint; // End point on the final bonus point platform
    public int CurrentBoatAmount = 1;
    public int TargetIndex; // Target waypoint index
    public float TotalScore;
    public float CurrentScore;
    public float Speed = 1.5f;
    public float DefaulSpeed = 1.5f;
    public float HorizontalSpeed = 5f;
    public float Offset;
    public bool IsSliding;

    private List<Boat> _tempBoatList = new List<Boat>(); // Temporary boat list to pool boats at the final bonus platform
    private float _finalTimer;
    private float _mousePosX;

    public void ResetStats() // Reset player data to default along with the positions and rotations
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

        CurrentBoatAmount = 1;
        CurrentScore = 0;
        TargetIndex = 0;
        Speed = DefaulSpeed;
        Offset = 0f;
        IsSliding = false;
    }
    public void CharacterMovement() // Basically player's horizontal movement along the curve
    {
        var waypoints = WaterSlideData.LocalPoints;
        var density = WaterSlideData.Density;
        var depth = WaterSlideData.Depth;
        var width = WaterSlideData.Width;
        var targetIndex = TargetIndex;

        // Get position along the curve corresponding to the offset
        var vel = Mathf.SmoothStep(0, 1, Mathf.Abs(Offset) / ((density - 1) / 2f + density/2f));
        var height = Vector3.up * (vel * depth);
        var radius = (Mathf.Pow(width+1f, 2f) + 4f * Mathf.Pow(depth, 2f)) / (8f * depth);
        var dir = waypoints[targetIndex] + Vector3.up * radius - Child.transform.position;
        
        if (Input.GetMouseButtonDown(0))
        {
            _mousePosX = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < _mousePosX && Offset < width*2f)
            {
                _mousePosX = Input.mousePosition.x;
                Offset += Time.deltaTime*HorizontalSpeed*5f;
                Child.transform.position -= Child.transform.right * (Time.deltaTime * HorizontalSpeed);
            }

            if (Input.mousePosition.x > _mousePosX && Offset > -width*2f)
            {
                _mousePosX = Input.mousePosition.x;
                Offset -= Time.deltaTime*HorizontalSpeed*5f;
                Child.transform.position += Child.transform.right * (Time.deltaTime * HorizontalSpeed);
            }
        }
        Child.transform.position = new Vector3(Child.transform.position.x, transform.position.y+height.y+0.1f, Child.transform.position.z);
        Child.transform.rotation = Quaternion.LookRotation(Child.transform.forward,dir.normalized);
    }
    private void StartSliding(Dictionary<string,object> message)
    {
        IsSliding = true;
        StartCoroutine(Move());
    }
    private IEnumerator Move() // Move player along the waypoints
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
                if (TargetIndex >= waypoints.Length-1)
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
    public void EndPlatformMovement() // Movement after player finished the level and moving along the bonus point platforms
    {
        if (CurrentBoatAmount > 0)
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
    private void TakeDamage(Dictionary<string,object> message) // Remove boat according to taken damage and position remaining boats that player has
    {
        var obstacle = (Obstacle) message["obstacle"];
        if (CurrentBoatAmount > 0)
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
                CurrentBoatAmount--;
            }
        }
    }

    private void CountBoat(Dictionary<string,object> message) // Deparent one boat and remove it from the boat list
    {
        EndPoint = (Vector3) message["endPoint"];
        if (CurrentBoatAmount > 0)
        {
            var scale = Boat.transform.localScale.y;
            BoatList[^1].gameObject.transform.SetParent(null);
            _tempBoatList.Add(BoatList[^1]);
            BoatList.RemoveAt(BoatList.Count-1);
            CurrentBoatAmount--;
        }
    }

    private void CountRemainingBoats(Dictionary<string, object> message) // Pool remaining boats and remove them from the boat list
    {
        EndPoint = (Vector3) message["endPoint"];
        if (CurrentBoatAmount > 0)
        {
            _finalTimer += Time.deltaTime;
            Speed = 0f; // Stop movement while pooling remaining boats
            
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
                CurrentBoatAmount--;
                _finalTimer = 0f;
            }
        }
        else
        {
            Speed = DefaulSpeed; // Change speed back to default value after counting is done
        }
    }
    private void CollectItem(Dictionary<string, object> message) // Use collected power up
    {
        var item = (ICollectable) message["item"];
        item.Collect(this);
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
