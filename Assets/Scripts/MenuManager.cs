using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//add menu groupings to seperate preferences based on usage, possibly with a border and label

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<MenuPreference> _preferences;
    [SerializeField] private GameObject _preferencePrefab;
    [SerializeField] private GameObject _togglePrefab;
    [SerializeField] private GameObject _inputFieldPrefab;
    [SerializeField] private GameObject _sliderPrefab;
    [SerializeField] private GameObject _dropdownPrefab;

    private void Start()
    {
        InitEventListeners();
        InitPreferences();
    }

    private void InitEventListeners()
    {
        transform.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(SavePreferences);
        transform.Find("ClearButton").GetComponent<Button>().onClick.AddListener(ClearPreferences);
    }

    private void InitPreferences()
    {
        RectTransform content = transform.Find("Scroll View").GetComponent<ScrollRect>().content;
        float offset = _preferencePrefab.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < _preferences.Count; i++)
        {
            MenuPreference preference = _preferences[i];
            GameObject prefab = GetPrefabByType(preference.type);
            preference.Create(_preferencePrefab, content, (i * offset) - 75);
            preference.AddPreferencePrefab(prefab);
        }

        SetContentSize(content, offset);
        LoadPreferenceValues();
    }

    private GameObject GetPrefabByType(PreferenceType type) => type switch
    {
        PreferenceType.Toggle => _togglePrefab,
        PreferenceType.InputField => _inputFieldPrefab,
        PreferenceType.Slider => _sliderPrefab,
        PreferenceType.Dropdown => _dropdownPrefab,
        _ => throw new ArgumentException("Preference type is invalid or not implemented"),
    };

    private void SetContentSize(RectTransform content, float offset) => content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, offset * _preferences.Count);

    private void LoadPreferenceValues()
    {
        foreach (MenuPreference preference in _preferences)
        {
            preference.LoadPreferenceValue();
        }
    }

    private void SavePreferences()
    {
        foreach (MenuPreference preference in _preferences)
        {
            PlayerPrefs.SetString(preference.id, preference.GetPreferenceValue());
        }

        ReloadScene();
    }

    private void ClearPreferences()
    {
        foreach (MenuPreference preference in _preferences)
        {
            PlayerPrefs.DeleteKey(preference.id);
        }

        ReloadScene();
    }

    private static void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void OnValidate()
    {
        foreach (string id in GetDuplicatePreferenceIds())
        {
            _preferences.Last(p => p.id == id).id = string.Empty;
        }
    }

    private IEnumerable<string> GetDuplicatePreferenceIds() => _preferences
        .Select(p => p.id)
        .Distinct()
        .Where(id => _preferences
        .Count(p => p.id == id) > 1);
}
