using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;

    public void adjustSlider(int value)
    {
        slider.value = value;
    }

    public void setSliderMax(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }
}
