using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfReferenceAttribute))]
public class ShowIfReferenceDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool show = ShouldShow(property);
        if (!show)
            return 0f; // не занимает места

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return;

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndProperty();
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var attr = (ShowIfReferenceAttribute)attribute;
        if (string.IsNullOrEmpty(attr.ConditionField))
            return true;

        SerializedProperty conditionProp = property.serializedObject.FindProperty(attr.ConditionField);

        if (conditionProp == null)
        {
            string propertyPath = property.propertyPath; 
            int lastDot = propertyPath.LastIndexOf('.');
            string prefix = lastDot == -1 ? "" : propertyPath.Substring(0, lastDot);
            string relativePath = string.IsNullOrEmpty(prefix) ? attr.ConditionField : prefix + "." + attr.ConditionField;
            conditionProp = property.serializedObject.FindProperty(relativePath);
        }

        if (conditionProp == null)
        {
            Debug.LogWarning($"ShowIfReference: условное поле '{attr.ConditionField}' не найдено для свойства '{property.propertyPath}' в объекте '{property.serializedObject.targetObject}'. Поле будет показано по умолчанию.");
            return true;
        }

        switch (conditionProp.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return conditionProp.boolValue;
            case SerializedPropertyType.Enum:
                return conditionProp.enumValueIndex != 0;
            case SerializedPropertyType.Integer:
                return conditionProp.intValue != 0;
            default:
                Debug.LogWarning($"ShowIfReference: Unsupported condition property type '{conditionProp.propertyType}' for '{attr.ConditionField}'. Показываем по умолчанию.");
                return true;
        }
    }
}
