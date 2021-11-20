using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Test : MonoBehaviour
{
    public GameObject Sphere;
    
    private float _distance;
    private List<GameObject> list = new List<GameObject>();

    private void Start()
    {
        list.Add(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Physics.Raycast(list[^1].transform.position, Vector3.down, out var hit, 20f, 1 << 6))
            {
                _distance = Vector3.Distance(list[^1].transform.position,hit.point);
                var obj = Instantiate(Sphere, list[^1].transform.position, Quaternion.identity);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].transform.position += Vector3.up * (_distance * 3f);
                }
                obj.GetComponent<FixedJoint>().connectedBody = list[^1].GetComponent<Rigidbody>();

                list.Add(obj);
            }
        }
    }
}
