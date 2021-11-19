using UnityEngine;
using UnityEditor;

public class OddEvenRangeAttribute : PropertyAttribute 
{
    public readonly int min, max;
    public readonly bool isEven;
    public OddEvenRangeAttribute(int min, int max, bool isEven)
    {
        this.min = min;
        this.max = max;
        this.isEven = isEven;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(OddEvenRangeAttribute))]
public class OddEvenRangeAttributeDrawer : PropertyDrawer
{
    OddEvenRangeAttribute rangeAttribute => (OddEvenRangeAttribute)attribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect textFieldPosition = position;
        textFieldPosition.width = position.width;
        textFieldPosition.height = position.height;
        
        int minVal, maxVal;
        if (rangeAttribute.isEven)
        {
            minVal = rangeAttribute.min % 2 == 0 ? rangeAttribute.min : rangeAttribute.min + 1;
            maxVal = rangeAttribute.max % 2 == 0 ? rangeAttribute.max : rangeAttribute.max - 1;
        }
        else
        {
            minVal = rangeAttribute.min % 2 == 0 ? rangeAttribute.min + 1 : rangeAttribute.min;
            maxVal = rangeAttribute.max % 2 == 0 ? rangeAttribute.max - 1 : rangeAttribute.max;
        }

        EditorGUI.IntSlider(position, property, minVal, maxVal, label);
        
        var val = property.intValue;
        if (rangeAttribute.isEven)
        {
            val = val % 2 == 0 ? val : val - 1;
        }
        else
        {
            val = val % 2 == 0 ? val - 1 : val;
        }
        property.intValue = val;
    }
}
#endif
