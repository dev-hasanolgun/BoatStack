using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Player Player;
    public float HorizontalSpeed = 5f;
    public float Offset;
    
    public void MoveCharacter()
    {
        var waypoints = Player.WaterSlideData.LocalPoints;
        var density = Player.WaterSlideData.Density;
        var depth = Player.WaterSlideData.Depth;
        var width = Player.WaterSlideData.Width;
        var targetIndex = Player.TargetIndex;
        
        var vel = Mathf.SmoothStep(0, 1, Mathf.Abs(Offset) / ((density - 1) / 2f + density/2f));
        var height = Vector3.up * (vel * depth);
        var radius = (Mathf.Pow(width+1f, 2f) + 4f * Mathf.Pow(depth, 2f)) / (8f * depth);
        var dir = waypoints[targetIndex] + Vector3.up * radius - transform.position;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Offset += Time.deltaTime*HorizontalSpeed;
            transform.position -= transform.right * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Offset -= Time.deltaTime*HorizontalSpeed;
            transform.position += transform.right * Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, height.y+0.25f, transform.position.z);
        transform.rotation = Quaternion.LookRotation(transform.forward,dir.normalized);
    }
}
