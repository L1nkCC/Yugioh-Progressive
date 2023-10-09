using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using DG.Tweening;
using static DS_Environment.ZoneType;
public class HandZone : Deck
{
    Vector3 HAND_SPACING = new Vector3(2, 0, 0);

    public override ZON_Option type => ZON_Option.handZone;
    protected override bool PLCY_moveToFacedown => SceneManagement.inMultiPlayScene() && Unity.Netcode.NetworkManager.Singleton.LocalClientId != (ulong)duelist;

    public override void DoubleClicked(Card card)
    {
        base.DoubleClicked(card);
        if (card.Info.isFieldSpell())
            BTN_Handler.BTN_Click(card, BTN_Option.toFieldZone);
        if (card.Info.isMainDeckMonster())
            BTN_Handler.BTN_Click(card, BTN_Option.specialSummonAttack);
        if (card.Info.isSpellTrap())
            BTN_Handler.BTN_Click(card, BTN_Option.set);
        if (card.Info.isExtraDeckMonster())
            BTN_Handler.BTN_Click(card, BTN_Option.toExtraDeck);
        
    }

    public override void Position()
    {
        if (cards.Count == 0)
            return;

        if (cards.Count % 2 == 0) 
        {
            for (int i = 0; i < cards.Count/2; i++)
            {
                cards[i].transform.DOMove((transform.position + HAND_SPACING/2) - HAND_SPACING * (i+1), PositionAnimationTime);
            }
            for (int i = cards.Count/2; i < cards.Count; i++)
            {
                cards[i].transform.DOMove( (transform.position + HAND_SPACING / 2) + HAND_SPACING * (i - cards.Count / 2), PositionAnimationTime);
            }
        }
        else
        {
            for (int i = 0; i < cards.Count / 2; i++)
            {
                cards[i].transform.DOMove( transform.position - HAND_SPACING * (i+1), PositionAnimationTime);
            }
            for(int i = cards.Count/2; i < cards.Count; i++)
            {
                cards[i].transform.DOMove( transform.position + HAND_SPACING * (i-cards.Count/2), PositionAnimationTime);
            }
        }
    }
}
