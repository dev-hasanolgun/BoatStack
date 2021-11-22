using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Vector3 OffsetPos;
    public Quaternion CamRotation;
    
    private float _angle = 0;

    public void FocusPlayer() // Focus camera to the player with offset and rotation set from inspector
    {
        var rotation = transform.rotation;
        
        var rot = Quaternion.LookRotation(Player.transform.forward);
        transform.rotation = Quaternion.Slerp(rotation, rot*CamRotation, Time.deltaTime * 3f);
        transform.position = Vector3.Lerp(transform.position,Player.transform.position + Player.transform.right * OffsetPos.z + Player.transform.up * OffsetPos.y + Player.transform.forward * OffsetPos.x, Time.deltaTime * 5f);
    }

    private void Start()
    {
        transform.position = Player.transform.position;
    }

    private void Update()
    {
        FocusPlayer();
    }
}