using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //add scrollview
    //add reset to default pop up - are you sure?

    [Serializable]
    public class MenuPref
    {
        public string name;
        public PrefType type;
        private GameObject _pref;

        public bool defaultValueToggle;
        public string defaultValueInputField;
        public int defaultValueSlider;

        public int minValue = 0;
        public int maxValue = 100;

        public void Create(GameObject prefab, Transform parent, float offset)
        {
            _pref = Instantiate(prefab, parent);
            Vector3 position = _pref.GetComponent<RectTransform>().anchoredPosition;
            _pref.GetComponent<RectTransform>().anchoredPosition = new Vector3(position.x, position.y - offset);

            _pref.name = $"MenuPref: {name}";
            _pref.GetComponentsInChildren<TMP_Text>().Single(t => t.name == "NameText").text = name;
        }

        public void AddPref(GameObject prefab)
        {
            Transform parent = _pref.GetComponentsInChildren<RectTransform>().Single(rt => rt.name == "Pref").transform;
            Instantiate(prefab, parent);

            //Init
            switch (type)
            {
                case PrefType.Toggle:
                    Toggle toggle = _pref.GetComponentInChildren<Toggle>();
                    toggle.isOn = defaultValueToggle;
                    break;
                case PrefType.InputField:
                    TMP_InputField inputField = _pref.GetComponentInChildren<TMP_InputField>();
                    inputField.text = defaultValueInputField;
                    break;
                case PrefType.Slider:
                    Slider slider = _pref.GetComponentInChildren<Slider>();
                    slider.value = defaultValueSlider;
                    slider.minValue = minValue;
                    slider.maxValue = maxValue;
                    break;
                default:
                    throw new ArgumentException("Pref type is invalid or not implemented");
            }
        }

        public string GetPrefValue() => type switch
        {
            PrefType.Toggle => _pref.GetComponentInChildren<Toggle>().isOn.ToString(),
            PrefType.InputField => _pref.GetComponentInChildren<TMP_InputField>().text,
            PrefType.Slider => _pref.GetComponentInChildren<Slider>().value.ToString(),
            _ => throw new ArgumentException("Pref type is invalid or not implemented"),
        };

        public void LoadPrefValue()
        {
            if (PlayerPrefs.HasKey(name))
            {
                switch (type)
                {
                    case PrefType.Toggle:
                        _pref.GetComponentInChildren<Toggle>().isOn = bool.TryParse(PlayerPrefs.GetString(name), out bool isOn) && isOn;
                        break;
                    case PrefType.InputField:
                        _pref.GetComponentInChildren<TMP_InputField>().text = PlayerPrefs.GetString(name);
                        break;
                    case PrefType.Slider:
                        _pref.GetComponentInChildren<Slider>().value = float.TryParse(PlayerPrefs.GetString(name), out float value) ? value : default;
                        break;
                    default:
                        throw new ArgumentException("Pref type is invalid or not implemented");
                }
            }
            //else 
            //{
            //    switch (type)
            //    {
            //        case PrefType.Toggle:
            //            _pref.GetComponentInChildren<Toggle>().isOn = defaultValueToggle;
            //            break;
            //        case PrefType.InputField:
            //            _pref.GetComponentInChildren<TMP_InputField>().text = defaultValueInputField;
            //            break;
            //        case PrefType.Slider:
            //            _pref.GetComponentInChildren<Slider>().value = defaultValueSlider;
            //            break;
            //        default:
            //            throw new ArgumentException("Pref type is invalid or not implemented");
            //    }
            //}
        }
    }

    [SerializeField] private List<MenuPref> _preferences;
    [SerializeField] private GameObject _preferencePrefab;
    [SerializeField] private GameObject _togglePrefab;
    [SerializeField] private GameObject _inputFieldPrefab;
    [SerializeField] private GameObject _sliderPrefab;

    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _closeButton;

    private const int MaxPreferencesHeight = 360;

    private void Start()
    {
        InitPreferences();
        InitEventListeners();
    }

    private void InitEventListeners()
    {
        _confirmButton.onClick.AddListener(SavePreferences);
        _clearButton.onClick.AddListener(ClearPreferences);
        //_closeButton.onClick.AddListener(Close);
    }

    private void LoadPreferences()
    {
        foreach (MenuPref pref in _preferences)
        {
            pref.LoadPrefValue();
        }
    }

    private void SavePreferences()
    {
        foreach (MenuPref pref in _preferences)
        {
            PlayerPrefs.SetString(pref.name, pref.GetPrefValue());
        }

        ReloadScene();
    }

    private void ClearPreferences()
    {
        foreach (MenuPref pref in _preferences)
        {
            PlayerPrefs.DeleteKey(pref.name);
        }

        ReloadScene();
    }

    private static void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void InitPreferences()
    {
        Transform contentTransform = transform.Find("Scroll View").Find("Viewport").Find("Content");

        float offset = _preferencePrefab.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < _preferences.Count; i++)
        {
            MenuPref pref = _preferences[i];
            pref.Create(_preferencePrefab, contentTransform, (i * offset) - 70);

            GameObject prefab = GetPrefabByPrefType(_preferences[i].type);
            pref.AddPref(prefab);
        }

        contentTransform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, offset * _preferences.Count);

        LoadPreferences();
    }

    private GameObject GetPrefabByPrefType(PrefType type) => type switch
    {
        PrefType.Toggle => _togglePrefab,
        PrefType.InputField => _inputFieldPrefab,
        PrefType.Slider => _sliderPrefab,
        _ => throw new ArgumentException("Pref type is invalid or not implemented"),
    };

    private void Update()
    {
        
    }

    //private void OnValidate()
    //{
    //    float height = _preferencePrefab.GetComponent<RectTransform>().rect.height;
    //    float totalHeight = 0;

    //    for (int i = 0; i < _preferences.Count; i++)
    //    {
    //        totalHeight += height;
    //        if (totalHeight > MaxPreferencesHeight)
    //        {
    //            _preferences.RemoveRange(i, _preferences.Count - i);
    //            Debug.LogWarning("Max preferences reached - adjust preference prefab height to add more preferences");
    //            break;
    //        }
    //    }
    //}
}
