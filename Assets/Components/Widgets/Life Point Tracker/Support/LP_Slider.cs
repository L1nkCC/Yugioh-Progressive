using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using Utilities;
using DG.Tweening;
[RequireComponent(typeof(UnityEngine.UI.Slider))]
public class LP_Slider : MonoBehaviour
{
    private TMPro.TextMeshProUGUI _valueLabel;
    private UnityEngine.UI.Slider slider;
    [SerializeField] float OnValueChangeAnimationTime = .7f;
    
    private float trueValue = 8000f;
    private void _updateValueLabel()
    {
        _valueLabel.text = trueValue.ToString();
    }

    public void Add(uint magnitude)
    {
        trueValue += magnitude;
        slider.DOValue(trueValue, OnValueChangeAnimationTime);
        _updateValueLabel();
    }
    public void Subtract(uint magnitude)
    {
        trueValue -= magnitude;
        slider.DOValue(trueValue, OnValueChangeAnimationTime);
        _updateValueLabel();
    }

    protected void Awake()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();

        slider.interactable = false;
        slider.wholeNumbers = true;
        trueValue = slider.value;

        _valueLabel = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _updateValueLabel();
    }
}
