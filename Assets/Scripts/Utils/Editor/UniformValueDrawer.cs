using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UniformValue))]
public class UniformValueDrawer : PropertyDrawer
{
    private const float PlusMinusLabelWidth = 20;
    private const float FieldSpacing = 10f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty valueProp = property.FindPropertyRelative("Value");
        SerializedProperty variationProp = property.FindPropertyRelative("Variation");

        float totalFieldWidth = position.width - PlusMinusLabelWidth - FieldSpacing;
        float valueFieldWidth = totalFieldWidth / 3f;
        float variationFieldWidth = totalFieldWidth / 3f;

        var valueRect = new Rect(position.x, position.y, valueFieldWidth, position.height);
        var labelRect = new Rect(valueRect.xMax + FieldSpacing / 2f, position.y, PlusMinusLabelWidth, position.height);
        var variationRect = new Rect(labelRect.xMax + FieldSpacing / 2f, position.y, variationFieldWidth, position.height);

        EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
        EditorGUI.LabelField(labelRect, "±");

        EditorGUI.PropertyField(variationRect, variationProp, GUIContent.none);
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}