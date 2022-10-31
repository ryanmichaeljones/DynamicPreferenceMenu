using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    private Slider slider;
    private TMP_Text valueText;

    void Start()
    {
        slider = GetComponent<Slider>();
        valueText = GetComponentInChildren<TMP_Text>();

        UpdateValueText(slider.value);
        slider.onValueChanged.AddListener(UpdateValueText);
    }

    private void UpdateValueText(float value) => valueText.text = value.ToString();
}
