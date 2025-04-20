using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RangeValue))]
public class RangeValueDrawer : PropertyDrawer
{
    private const float PlusMinusLabelWidth = 20;
    private const float FieldSpacing = 10f;

public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    EditorGUI.BeginProperty(position, label, property);

    // Draw the label
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;

    SerializedProperty minProp = property.FindPropertyRelative("Min");
    SerializedProperty maxProp = property.FindPropertyRelative("Max");

    float spacing = 8f;
    float labelWidth = 30f;
    float fieldWidth = (position.width - 2 * (labelWidth + spacing) - spacing) / 4f;

    Rect minLabelRect = new Rect(position.x, position.y, labelWidth, position.height);
    Rect minFieldRect = new Rect(minLabelRect.xMax + spacing, position.y, fieldWidth, position.height);

    Rect maxLabelRect = new Rect(minFieldRect.xMax + spacing, position.y, labelWidth, position.height);
    Rect maxFieldRect = new Rect(maxLabelRect.xMax + spacing, position.y, fieldWidth, position.height);

    EditorGUI.LabelField(minLabelRect, "Min:");
    EditorGUI.PropertyField(minFieldRect, minProp, GUIContent.none);

    EditorGUI.LabelField(maxLabelRect, "Max:");
    EditorGUI.PropertyField(maxFieldRect, maxProp, GUIContent.none);

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
}

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}