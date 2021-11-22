using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Vector3 OffsetPos;
    public Quaternion CamRotation;
    
    private float _angle = 0;

    public void FocusPlayer()
    {
        var rotation = transform.rotation;
        
        var rot = Quaternion.LookRotation(Player.transform.forward);
        transform.rotation = Quaternion.Slerp(rotation, rot*CamRotation, Time.deltaTime * 3f);
        transform.position = Vector3.Lerp(transform.position,Player.transform.position + Player.transform.right * OffsetPos.z + Player.transform.up * OffsetPos.y + Player.transform.forward * OffsetPos.x, Time.deltaTime * 5f);
    }

    public void VictoryCameraRotation()
    {
        if (Mathf.Abs(transform.eulerAngles.y + Player.transform.eulerAngles.y - 360f) > 1f)
        {
            _angle += Time.deltaTime;
            var pos = transform.position;
            var runnerPos = Player.transform.position;
            var dir = runnerPos - pos;
            var rot = Quaternion.LookRotation(dir);
            transform.RotateAround(runnerPos, Vector3.up, _angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 3f);
            transform.position += dir* (Time.deltaTime / 1.5f);
        }
        else
        {
            _angle = 0;
        }
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