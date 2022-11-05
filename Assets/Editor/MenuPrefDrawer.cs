using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(MenuPreference))]
    public class MenuPrefDrawer : PropertyDrawer
    {
        private const int PropertyFieldSpacing = 20;
        private const int PropertyFieldHeight = 18;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => GetPreferenceType(property) switch
        {
            PreferenceType.Toggle => EditorGUIUtility.singleLineHeight * 4 + 6,
            PreferenceType.InputField => EditorGUIUtility.singleLineHeight * 4 + 6,
            PreferenceType.Slider => EditorGUIUtility.singleLineHeight * 6 + 10,
            _ => throw new ArgumentException("Preference type is invalid or not implemented"),
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            int propertyFieldCount = 0;

            EditorGUI.PropertyField(GetPropertyRect(position, propertyFieldCount++), property.FindPropertyRelative("name"));
            EditorGUI.PropertyField(GetPropertyRect(position, propertyFieldCount++), property.FindPropertyRelative("type"));

            switch (GetPreferenceType(property))
            {
                case PreferenceType.Toggle:
                    EditorGUI.PropertyField(GetPropertyRect(position, propertyFieldCount++), property.FindPropertyRelative("defaultValueToggle"), new GUIContent("Default Value"));
                    break;
                case PreferenceType.InputField:
                    EditorGUI.PropertyField(GetPropertyRect(position, propertyFieldCount++), property.FindPropertyRelative("defaultValueInputField"), new GUIContent("Default Value"));
                    break;
                case PreferenceType.Slider:
                    SerializedProperty minValue = property.FindPropertyRelative("minValue");
                    SerializedProperty maxValue = property.FindPropertyRelative("maxValue");
                    SerializedProperty currentValue = property.FindPropertyRelative("defaultValueSlider");

                    if (minValue.intValue < 0)
                    {
                        minValue.intValue = 0;
                    }

                    if (maxValue.intValue < 0)
                    {
                        maxValue.intValue = 0;
                    }

                    if (currentValue.intValue < 0)
                    {
                        currentValue.intValue = 0;
                    }

                    EditorGUI.DelayedIntField(GetPropertyRect(position, propertyFieldCount++), minValue);
                    EditorGUI.DelayedIntField(GetPropertyRect(position, propertyFieldCount++), maxValue);

                    if (currentValue.intValue < minValue.intValue)
                    {
                        currentValue.intValue = minValue.intValue;
                    }

                    if (currentValue.intValue > maxValue.intValue)
                    {
                        currentValue.intValue = maxValue.intValue;
                    }

                    EditorGUI.IntSlider(GetPropertyRect(position, propertyFieldCount++), currentValue, minValue.intValue, maxValue.intValue, new GUIContent("Default Value"));
                    break;
                default:
                    throw new ArgumentException("Preference type is invalid or not implemented");
            }

            EditorGUI.EndProperty();
        }

        private static Rect GetPropertyRect(Rect position, int count) => new(position.x, position.y + (count * PropertyFieldSpacing), position.width, PropertyFieldHeight);

        private static PreferenceType GetPreferenceType(SerializedProperty property) => (PreferenceType)property.FindPropertyRelative("type").enumValueIndex;
    }
}
