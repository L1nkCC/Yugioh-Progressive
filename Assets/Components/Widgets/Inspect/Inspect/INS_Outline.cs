using UnityEngine;
using DS_Environment;
using Utilities;
using System.Linq;

public class INS_Outline : MonoBehaviour
{
    UnityEngine.UI.Image frameOutline;
    UnityEngine.UI.Image pendulumOutlineOverlay;

    private void Awake()
    {
        frameOutline = GetComponent<UnityEngine.UI.Image>();
        frameOutline.color = CardFrame.defaultFrame;
        pendulumOutlineOverlay = GetComponentsInChildren<UnityEngine.UI.Image>().Where(child => child.gameObject != this.gameObject).ElementAt(0);//get parent exclusive Image from children
        pendulumOutlineOverlay.color = CardFrame.spellCardFrame;
        pendulumOutlineOverlay.enabled = false;
    }

    public void SetOutlineColor(Card card)
    {
        pendulumOutlineOverlay.enabled = card.Info.isPendulumFrame();
        frameOutline.color = CardFrame.FrameColor(card.Info);
    }
}
