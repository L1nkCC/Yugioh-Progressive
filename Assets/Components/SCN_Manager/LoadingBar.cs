using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    TMPro.TextMeshProUGUI textBox;
    UnityEngine.UI.Slider loadingBar;
    void Start()
    {
        textBox = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        loadingBar = GetComponentInChildren<UnityEngine.UI.Slider>();
    }
    /// <summary>
    /// This will automatically take care of the .1 hold on the percentages given by the LoadSceneAsync operation
    /// </summary>
    /// <param name="loadingPercent"></param>
    public void SetSceneLoadingValue(float loadingPercent)
    {
        float truePercent = Mathf.Clamp01(loadingPercent / .9f);

        SetValue(truePercent);
    }
    public void SetValue(float percent)
    {
        loadingBar.value = percent;
        textBox.text = (int)(percent*100f) + "%";
    }
    public float GetValue()
    {
        return loadingBar.value;
    }

}
