using UnityEngine;

public class INS_Description : UnityEngine.UI.ScrollRect
{
    private TMPro.TextMeshProUGUI textBox;
    protected override void Awake()
    {
        base.Awake();
        textBox = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void SetDescription(Card card)
    {
        textBox.text = SetUpDescription(card.Info);
        Canvas.ForceUpdateCanvases();
    }

    private static string SetUpDescription(CardInfo info)
    {
        string description = "";
        description += info.name + "\n";
        description += info.race  +" " + info.type + "\n";
        description += "\n";
        description += info.desc + "\n" + "\n";
        return description;
    }
}
