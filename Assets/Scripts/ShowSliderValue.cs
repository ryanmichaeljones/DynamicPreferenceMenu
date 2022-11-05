using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    private Slider _slider;
    private TMP_Text _valueText;

    void Start()
    {
        _slider = GetComponent<Slider>();
        _valueText = GetComponentInChildren<TMP_Text>();

        UpdateValueText(_slider.value);
        _slider.onValueChanged.AddListener(UpdateValueText);
    }

    private void UpdateValueText(float value) => _valueText.text = value.ToString();
}
