using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private int sliderValue;
    [SerializeField] private Slider _slider;

    private void Update()
    {
        SetSliderValue(Time.deltaTime);
    }

    public void SetSliderValue(float value)
    {
        if(_slider.value >= _slider.maxValue)
        {
            _slider.value = 0;
        }
        else
        {
            _slider.value += value;
        }
     
    }
}
