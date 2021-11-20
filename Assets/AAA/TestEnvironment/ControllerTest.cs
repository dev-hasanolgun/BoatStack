using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{
    public Transform Object;

    private float _offset;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Object.transform.position -= transform.right * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Object.transform.position += transform.right * Time.deltaTime;
        }
    }
}
