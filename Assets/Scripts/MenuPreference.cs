using Assets;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MenuPreference
{
    public string name;
    public PreferenceType type;
    private GameObject _preference;

    public bool defaultValueToggle;
    public string defaultValueInputField;
    public int defaultValueSlider;

    public int minValue = 0;
    public int maxValue = 100;

    public void Create(GameObject prefab, Transform parent, float offset)
    {
        _preference = GameObject.Instantiate(prefab, parent);
        _preference.name = $"Preference: {name}";
        _preference.GetComponentsInChildren<TMP_Text>().Single(t => t.name == "NameText").text = name;

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
            default:
                throw new ArgumentException("Preference type is invalid or not implemented");
        }
    }

    public void LoadPreferenceValue()
    {
        if (PlayerPrefs.HasKey(name))
        {
            switch (type)
            {
                case PreferenceType.Toggle:
                    _preference.GetComponentInChildren<Toggle>().isOn = bool.TryParse(PlayerPrefs.GetString(name), out bool isOn) && isOn;
                    break;
                case PreferenceType.InputField:
                    _preference.GetComponentInChildren<TMP_InputField>().text = PlayerPrefs.GetString(name);
                    break;
                case PreferenceType.Slider:
                    _preference.GetComponentInChildren<Slider>().value = float.TryParse(PlayerPrefs.GetString(name), out float value) ? value : default;
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
        _ => throw new ArgumentException("Preference type is invalid or not implemented"),
    };
}
