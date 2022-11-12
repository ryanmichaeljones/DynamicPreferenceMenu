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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => property.isExpanded ? GetPreferenceType(property) switch
        {
            PreferenceType.Toggle => EditorGUIUtility.singleLineHeight * 4 + 6,
            PreferenceType.InputField => EditorGUIUtility.singleLineHeight * 4 + 6,
            PreferenceType.Slider => EditorGUIUtility.singleLineHeight * 6 + 10,
            PreferenceType.Dropdown => property.FindPropertyRelative("dropdownOptions").isExpanded ? GetDropdownOptionsHeight(property) : EditorGUIUtility.singleLineHeight * 5 + 6,
            _ => throw new ArgumentException("Preference type is invalid or not implemented"),
        } : EditorGUIUtility.singleLineHeight;

        private static float GetDropdownOptionsHeight(SerializedProperty property)
        {
            SerializedProperty dropdownOptions = property.FindPropertyRelative("dropdownOptions");

            if (dropdownOptions.arraySize == 0)
            {
                return (EditorGUIUtility.singleLineHeight * 7 + 10) + EditorGUIUtility.singleLineHeight;
            }

            return (EditorGUIUtility.singleLineHeight * 7 + 10) + EditorGUIUtility.singleLineHeight * dropdownOptions.arraySize;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int propertyFieldCount = 0;

            property.isExpanded = EditorGUI.Foldout(GetPropertyRect(position, propertyFieldCount++), property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

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
                    case PreferenceType.Dropdown:
                        string[] array = GetSerializedPropertyArray(property, "dropdownOptions");
                        int currentIndex = Array.IndexOf(array, property.FindPropertyRelative("defaultValueDropdown").stringValue);
                        int newIndex = EditorGUI.Popup(GetPropertyRect(position, propertyFieldCount++), "Default Value", currentIndex, array);
                        property.FindPropertyRelative("defaultValueDropdown").stringValue = array[newIndex != -1 ? newIndex : 0];

                        EditorGUI.PropertyField(GetPropertyRect(position, propertyFieldCount++), property.FindPropertyRelative("dropdownOptions"), new GUIContent("Dropdown Options"));
                        break;
                    default:
                        throw new ArgumentException("Preference type is invalid or not implemented");
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private static string[] GetSerializedPropertyArray(SerializedProperty property, string relativePropertyPath)
        {
            SerializedProperty sp = property.FindPropertyRelative(relativePropertyPath);
            var array = new string[sp.arraySize];

            for (int i = 0; i < sp.arraySize; i++)
            {
                array[i] = sp.GetArrayElementAtIndex(i).stringValue;
            }

            return array;
        }

        private static Rect GetPropertyRect(Rect position, int count) => new(position.x, position.y + (count * PropertyFieldSpacing), position.width, PropertyFieldHeight);

        private static PreferenceType GetPreferenceType(SerializedProperty property) => (PreferenceType)property.FindPropertyRelative("type").enumValueIndex;
    }
}
