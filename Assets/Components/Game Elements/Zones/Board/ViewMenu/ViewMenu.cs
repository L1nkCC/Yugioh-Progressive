using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using static DS_Environment.ZoneType;
public class ViewMenu : Zone
{
    public Zone referenceZone;
    public UnityEngine.UI.GridLayoutGroup grid;
    public UnityEngine.UI.Button exitButton;
    protected override bool PLCY_moveToFacedown => false;

    public override ZON_Option type => referenceZone.type;

    protected override void Awake()
    {
        base.Awake();
        grid = GetComponentInChildren<UnityEngine.UI.GridLayoutGroup>();
        exitButton = GetComponentInChildren<UnityEngine.UI.Button>();
        exitButton.onClick.AddListener(() => Hide());
    }

    public override void DoubleClicked(Card card)
    {
        base.DoubleClicked(card);
        BTN_Handler.BTN_Click(card, BTN_Option.toHand);
    }


    private void view(ZON_Option option, DuelistDesignation duelist)
    {
        referenceZone = PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(option, duelist);
        for(int i= referenceZone.Count-1; i >=0; i--)
        {
            this.moveCard(referenceZone.cards[i]);
        }
    }
    public override void Position()
    {
        foreach (Card card in cards)
        {
            card.gameObject.transform.SetParent(grid.transform);
        }
        UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(this.transform as RectTransform);
    }

    private void hide()
    {
        for(int i = Count-1; i>=0; i--)
        {
            referenceZone.moveCard(cards[i]);
        }
    }

    public static void View(Zone zone)
    {
        PlaySceneGameObjects.ViewZone.SetActive(true);
        PlaySceneGameObjects.ViewZone.GetComponent<ViewMenu>().view(zone.type, zone.duelist);
    }
    public static void Hide()
    {
        PlaySceneGameObjects.ViewZone.GetComponent<ViewMenu>().hide();
        PlaySceneGameObjects.ViewZone.SetActive(false);
    }

}
