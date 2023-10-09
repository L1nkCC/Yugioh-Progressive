using UnityEngine;
using UnityEngine.UI;
using DS_Environment;
public class SoundOptionManager : MonoBehaviour
{
    [SerializeField] Slider[] SoundSliders;


    private void Awake()
    {
        SoundSliders = new Slider[System.Enum.GetValues(typeof(SFX_Manager.SFX_Option)).Length];

        foreach (SFX_Manager.SFX_Option option in System.Enum.GetValues(typeof(SFX_Manager.SFX_Option)))
        {
            GameObject newSoundSlider = Instantiate(Resources.Load<GameObject>(ResourcePath.SFX_Slider), this.transform);
            newSoundSlider.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = System.Enum.GetName(typeof(SFX_Manager.SFX_Option), option);
            SoundSliders[(int)option] = newSoundSlider.GetComponentInChildren<Slider>();
            SoundSliders[(int)option].value = PersistantScripts.SFX_Manager.volumeSettings[(int)option];
            SoundSliders[(int)option].onValueChanged.AddListener((float value) => { OnSliderValueChange(value, option); });
        }
    }

    private void OnSliderValueChange(float value, SFX_Manager.SFX_Option option)
    {
        PersistantScripts.SFX_Manager.volumeSettings[(int)option] = value;
        SFX_Manager.SaveVolumeSettings();
    }

    

}
