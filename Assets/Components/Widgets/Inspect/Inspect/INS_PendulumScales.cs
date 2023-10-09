using UnityEngine;
using DS_Environment;
public class INS_PendulumScales : MonoBehaviour
{
    private UnityEngine.UI.Image[] scaleImages;
    private TMPro.TextMeshProUGUI[] scaleNumbers;


    protected void Awake()
    {
        scaleImages = GetComponentsInChildren<UnityEngine.UI.Image>();
        scaleNumbers = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (UnityEngine.UI.Image scaleImage in scaleImages)
        {
            scaleImage.enabled = false;
        }
        foreach (TMPro.TextMeshProUGUI scaleNumber in scaleNumbers)
        {
            scaleNumber.enabled = false;
        }
    }
    public void SetScales(Card card)
    {
        foreach(UnityEngine.UI.Image scaleImage in scaleImages)
        {
            scaleImage.enabled = card.Info.isPendulum();
        }
        foreach(TMPro.TextMeshProUGUI scaleNumber in scaleNumbers)
        {
            scaleNumber.enabled = card.Info.isPendulum();
            scaleNumber.text = card.Info.scale.ToString();
        }
    }
}
