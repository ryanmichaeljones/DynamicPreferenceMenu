using Assets;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuPreference)), CanEditMultipleObjects]
public class MenuPrefEditor : Editor
{
    public SerializedProperty typeProperty;
    public SerializedProperty minValueProperty;
    public SerializedProperty maxValueProperty;
    public SerializedProperty defaultValueToggleProperty;
    public SerializedProperty defaultValueInputFieldProperty;
    public SerializedProperty defaultValueSliderProperty;

    public SerializedProperty groupProperty;

    private void OnEnable()
    {
        typeProperty = serializedObject.FindProperty("type");
        minValueProperty = serializedObject.FindProperty("minValue");
        maxValueProperty = serializedObject.FindProperty("maxValue");
        defaultValueToggleProperty = serializedObject.FindProperty("DefaultValueToggle");
        defaultValueInputFieldProperty = serializedObject.FindProperty("DefaultValueInputField");
        defaultValueSliderProperty = serializedObject.FindProperty("DefaultValueSlider");

        groupProperty = serializedObject.FindProperty("group");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProperty);

        switch ((PreferenceType)typeProperty.enumValueIndex)
        {
            case PreferenceType.Toggle:
                EditorGUILayout.PropertyField(defaultValueToggleProperty, new GUIContent("defaultValueToggle"));
                break;
            case PreferenceType.InputField:
                EditorGUILayout.PropertyField(defaultValueInputFieldProperty, new GUIContent("defaultValueInputField"));
                break;
            case PreferenceType.Slider:
                EditorGUILayout.PropertyField(defaultValueSliderProperty, new GUIContent("defaultValueSlider"));
                EditorGUILayout.PropertyField(minValueProperty, new GUIContent("minValue"));
                EditorGUILayout.PropertyField(maxValueProperty, new GUIContent("maxValue"));
                break;
            default:
                throw new ArgumentException("Preference type is invalid or not implemented");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
