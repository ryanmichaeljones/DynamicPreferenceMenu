using Assets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static MenuManager;

[CustomEditor(typeof(MenuPref)), CanEditMultipleObjects]
public class MenuPrefEditor : Editor
{
    public SerializedProperty typeProperty;
    public SerializedProperty minValueProperty;
    public SerializedProperty maxValueProperty;
    public SerializedProperty defaultValueToggleProperty;
    public SerializedProperty defaultValueInputFieldProperty;
    public SerializedProperty defaultValueSliderProperty;

    private void OnEnable()
    {
        typeProperty = serializedObject.FindProperty("type");
        minValueProperty = serializedObject.FindProperty("minValue");
        maxValueProperty = serializedObject.FindProperty("maxValue");
        defaultValueToggleProperty = serializedObject.FindProperty("DefaultValueToggle");
        defaultValueInputFieldProperty = serializedObject.FindProperty("DefaultValueInputField");
        defaultValueSliderProperty = serializedObject.FindProperty("DefaultValueSlider");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProperty);

        PrefType type = (PrefType)typeProperty.enumValueIndex;

        switch (type)
        {
            case PrefType.Toggle:
                EditorGUILayout.PropertyField(defaultValueToggleProperty, new GUIContent("defaultValueToggle"));
                break;
            case PrefType.InputField:
                EditorGUILayout.PropertyField(defaultValueInputFieldProperty, new GUIContent("defaultValueInputField"));
                break;
            case PrefType.Slider:
                EditorGUILayout.PropertyField(defaultValueSliderProperty, new GUIContent("defaultValueSlider"));
                EditorGUILayout.PropertyField(minValueProperty, new GUIContent("minValue"));
                EditorGUILayout.PropertyField(maxValueProperty, new GUIContent("maxValue"));
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
