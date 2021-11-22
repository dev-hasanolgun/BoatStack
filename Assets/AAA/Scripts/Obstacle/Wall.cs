using Sirenix.OdinInspector;
using UnityEngine;

public class Wall : Obstacle
{
    [OnValueChanged("SetHeight")]
    public int HeigthLevel = 1;
    public float HeightScale = 0.3f;

    private void SetHeight() // Set height of the wall from inspector (why not)
    {
        var scale = transform.localScale;
        transform.localScale = new Vector3(scale.x,HeightScale*HeigthLevel,scale.z);
    }
}
