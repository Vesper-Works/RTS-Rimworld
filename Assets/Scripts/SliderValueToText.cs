using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSliderTextValue : MonoBehaviour
{
    public string ValueName;
    public Slider sliderUI;
    private TextMeshProUGUI textSliderValue;

    void Start()
    {
        textSliderValue = GetComponent<TextMeshProUGUI>();
        ShowSliderValue();
    }

    public void ShowSliderValue()
    {
        string sliderMessage = ValueName + ": " + sliderUI.value;
        textSliderValue.text = sliderMessage;
    }
}
