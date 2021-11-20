using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Wall : Obstacle
{
    [OnValueChanged("SetHeight")]
    public int HeigthLevel = 1;
    public float HeightScale = 0.3f;

    private void SetHeight()
    {
        var scale = transform.localScale;
        transform.localScale = new Vector3(scale.x,HeightScale*HeigthLevel,scale.z);
    }
}
