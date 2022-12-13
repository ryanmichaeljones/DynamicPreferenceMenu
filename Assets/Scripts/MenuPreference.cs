using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuPreference
{
    public string name;
    public PreferenceType type;
    public string id;
    private GameObject _preference;

    public bool defaultValueToggle;
    public string defaultValueInputField;
    public int defaultValueSlider;
    public string defaultValueDropdown;

    public int minValue = 0;
    public int maxValue = 100;

    public List<string> dropdownOptions;

    public void Create(GameObject prefab, Transform parent, float offset)
    {
        _preference = GameObject.Instantiate(prefab, parent);
        _preference.name = $"Preference: {name}";
        TMP_Text nameText = _preference.GetComponentsInChildren<TMP_Text>().Single(t => t.name == "NameText");
        float width = GameObject.Find("MenuPanel").GetComponent<RectTransform>().sizeDelta.x / 2;
        nameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        nameText.text = name;

        SetPreferencePosition(offset);
    }

    private void SetPreferencePosition(float offset)
    {
        Vector3 position = _preference.GetComponent<RectTransform>().anchoredPosition;
        _preference.GetComponent<RectTransform>().anchoredPosition = new Vector3(position.x, position.y - offset);
    }

    public void AddPreferencePrefab(GameObject prefab)
    {
        Transform parent = _preference.transform.Find("Pref");
        GameObject.Instantiate(prefab, parent);
        float width = GameObject.Find("MenuPanel").GetComponent<RectTransform>().sizeDelta.x / 2;
        parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        LoadDefaultValues();
    }

    private void LoadDefaultValues()
    {
        switch (type)
        {
            case PreferenceType.Toggle:
                Toggle toggle = _preference.GetComponentInChildren<Toggle>();
                toggle.isOn = defaultValueToggle;
                break;
            case PreferenceType.InputField:
                TMP_InputField inputField = _preference.GetComponentInChildren<TMP_InputField>();
                inputField.text = defaultValueInputField;
                break;
            case PreferenceType.Slider:
                Slider slider = _preference.GetComponentInChildren<Slider>();
                slider.value = defaultValueSlider;
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                break;
            case PreferenceType.Dropdown:
                TMP_Dropdown dropdown = _preference.GetComponentInChildren<TMP_Dropdown>();
                dropdown.AddOptions(dropdownOptions);
                break;
            default:
                throw new ArgumentException("Preference type is invalid or not implemented");
        }
    }

    public void LoadPreferenceValue()
    {
        if (PlayerPrefs.HasKey(id))
        {
            switch (type)
            {
                case PreferenceType.Toggle:
                    _preference.GetComponentInChildren<Toggle>().isOn = bool.TryParse(PlayerPrefs.GetString(id), out bool isOn) && isOn;
                    break;
                case PreferenceType.InputField:
                    _preference.GetComponentInChildren<TMP_InputField>().text = PlayerPrefs.GetString(id);
                    break;
                case PreferenceType.Slider:
                    _preference.GetComponentInChildren<Slider>().value = float.TryParse(PlayerPrefs.GetString(id), out float value) ? value : default;
                    break;
                case PreferenceType.Dropdown:
                    //none
                    break;
                default:
                    throw new ArgumentException("Preference type is invalid or not implemented");
            }
        }
    }

    public string GetPreferenceValue() => type switch
    {
        PreferenceType.Toggle => _preference.GetComponentInChildren<Toggle>().isOn.ToString(),
        PreferenceType.InputField => _preference.GetComponentInChildren<TMP_InputField>().text,
        PreferenceType.Slider => _preference.GetComponentInChildren<Slider>().value.ToString(),
        PreferenceType.Dropdown => _preference.GetComponentInChildren<TMP_Dropdown>().value.ToString(),
        _ => throw new ArgumentException("Preference type is invalid or not implemented"),
    };
}
