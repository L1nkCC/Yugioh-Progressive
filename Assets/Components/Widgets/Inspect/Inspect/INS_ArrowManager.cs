using UnityEngine;
using Utilities;
using DS_Environment;
using static DS_Environment.LinkArrow;
public class INS_ArrowManager : MonoBehaviour
{
    [SerializeField] INS_Arrow[] arrows = new INS_Arrow[Enum.Length(typeof(INS_ARW_Option))];

    private void Awake()
    {
        INS_Arrow[] childArrows = GetComponentsInChildren<INS_Arrow>(true);
        foreach(INS_Arrow childArrow in childArrows)
        {
            arrows[(int)childArrow.arrowType] = childArrow;
        }

        foreach(INS_ARW_Option option in System.Enum.GetValues(typeof(INS_ARW_Option)))
        {
            if(arrows[(int)option] == null)
            {
                Debug.LogError("INS_ArrowManager does not have correct arrow children");
            }
        }
    }

    public void SetArrows(Card card)
    {
        if (card.Info.isLinkMonster())
        {
            foreach(INS_ARW_Option type in System.Enum.GetValues(typeof(INS_ARW_Option)))
            {
                arrows[(int)type].enabled = true;
                arrows[(int)type].SetArrow(false);
                foreach (string infotype in card.Info.linkmarkers)
                {
                    if(infotype.Equals(type.INS_ARW_Name()))
                        arrows[(int)type].SetArrow(true);
                }
            }
        }
        else
        {
            foreach (INS_ARW_Option type in System.Enum.GetValues(typeof(INS_ARW_Option)))
            {
                arrows[(int)type].enabled = false;
            }
        }
    }

}
