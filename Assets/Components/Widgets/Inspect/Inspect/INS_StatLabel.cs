using DS_Environment;
public class INS_StatLabel : TMPro.TextMeshProUGUI
{
    public void SetAttackAndDefense(Card card)
    {
        if (!card.Info.isLinkMonster() && !card.Info.isSpellTrap())
            text = "" + card.Info.atk + "/" + card.Info.def;
        else
            text = "";
    }
}
