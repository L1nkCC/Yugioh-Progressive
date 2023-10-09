using UnityEngine;
using DS_Environment;
public class INS_Attribute : MonoBehaviour
{
    UnityEngine.UI.Image image;
    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    public void SetAttribute(Card card)
    {
        if (card.Info.isSpellTrap())
            image.sprite = ResourcePath.CardAttribute.GetAttributeImage(card.Info.frameType);//Path is saved as an attribute but uses names SPELL and TRAP, this is the easiest way to get that
        else
            image.sprite = ResourcePath.CardAttribute.GetAttributeImage(card.Info.attribute);
    } 


}
