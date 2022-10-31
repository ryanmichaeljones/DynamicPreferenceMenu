using System;
using UnityEditor;
using UnityEngine;
using static MenuManager;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(MenuPref))]
    public class MenuPrefDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty property1 = property.FindPropertyRelative("type");
            PrefType type = (PrefType)property1.enumValueIndex;
            switch (type)
            {
                case PrefType.Toggle:
                    return EditorGUIUtility.singleLineHeight * 4 + 6;
                case PrefType.InputField:
                    return EditorGUIUtility.singleLineHeight * 4 + 6;
                case PrefType.Slider:
                    return EditorGUIUtility.singleLineHeight * 6 + 10;
                default:
                    throw new ArgumentException();
            }

            // The 6 comes from extra spacing between the fields (2px each)
            return EditorGUIUtility.singleLineHeight * 4 + 6;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //EditorGUI.LabelField(position, label);

            var nameRect = new Rect(position.x, position.y + 0, position.width, 18);
            var typeRect = new Rect(position.x, position.y + 20, position.width, 18);
            var propertyRect1 = new Rect(position.x, position.y + 40, position.width, 18);
            var propertyRect2 = new Rect(position.x, position.y + 60, position.width, 18);
            var propertyRect3 = new Rect(position.x, position.y + 80, position.width, 18);

            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"));
            SerializedProperty property1 = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(typeRect, property1);

            PrefType type = (PrefType)property1.enumValueIndex;
            switch (type)
            {
                case PrefType.Toggle:
                    EditorGUI.PropertyField(propertyRect1, property.FindPropertyRelative("defaultValueToggle"), new GUIContent("Default Value"));
                    break;
                case PrefType.InputField:
                    EditorGUI.PropertyField(propertyRect1, property.FindPropertyRelative("defaultValueInputField"), new GUIContent("Default Value"));
                    break;
                case PrefType.Slider:
                    var minValue = property.FindPropertyRelative("minValue");
                    var maxValue = property.FindPropertyRelative("maxValue");

                    EditorGUI.DelayedIntField(propertyRect1, minValue);
                    EditorGUI.DelayedIntField(propertyRect2, maxValue);

                    var currentValue = property.FindPropertyRelative("defaultValueSlider");

                    if (currentValue.intValue < minValue.intValue)
                    {
                        currentValue.intValue = minValue.intValue;
                    }

                    if (currentValue.intValue > maxValue.intValue)
                    {
                        currentValue.intValue = maxValue.intValue;
                    }

                    EditorGUI.IntSlider(propertyRect3, currentValue, minValue.intValue, maxValue.intValue, new GUIContent("Default Value"));
                    break;
                default:
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
}
