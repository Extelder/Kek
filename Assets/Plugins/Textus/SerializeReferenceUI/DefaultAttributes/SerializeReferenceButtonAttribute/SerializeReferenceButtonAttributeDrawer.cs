#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(SerializeReferenceButtonAttribute))]
public class SerializeReferenceButtonAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = 2;

        float propHeight = EditorGUI.GetPropertyHeight(property, true);

        // одна строка кнопок + само поле
        return line + spacing + propHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = 2;

        EditorGUI.BeginProperty(position, label, property);

        // --- ЛИНИЯ С КНОПКАМИ ---
        Rect row = new Rect(position.x, position.y, position.width, line);

        // 1) маленькая кнопка поиска слева
        Rect searchBtn = new Rect(row.x, row.y, 32, line);

        if (GUI.Button(searchBtn, "Find", EditorStyles.miniButtonLeft))
        {
            FindScriptOfManagedReference(property);
        }

        // 2) кнопка смены класса (остальная ширина)
        Rect changeBtn = new Rect(row.x + 24, row.y, row.width - 24, line);

        var typeRestrictions = SerializedReferenceUIDefaultTypeRestrictions.GetAllBuiltInTypeRestrictions(fieldInfo);
        property.DrawSelectionButtonForManagedReference(changeBtn, typeRestrictions);

        // --- ПОЛЕ СВОЙСТВА ---
        Rect propRect = new Rect(position.x,
                                 position.y + line + spacing,
                                 position.width,
                                 position.height - line - spacing);

        EditorGUI.PropertyField(propRect, property, true);

        EditorGUI.EndProperty();
    }

    private void FindScriptOfManagedReference(SerializedProperty property)
    {
        if (property.managedReferenceValue == null)
        {
            Debug.LogWarning("Сначала назначьте класс в SerializeReference.");
            return;
        }

        Type type = property.managedReferenceValue.GetType();
        string[] guids = AssetDatabase.FindAssets(type.Name + " t:MonoScript");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            if (script != null && script.GetClass() == type)
            {
                Selection.activeObject = script;
                EditorGUIUtility.PingObject(script);
                return;
            }
        }

        Debug.LogWarning("Скрипт не найден: " + type.FullName);
    }
}
#endif
