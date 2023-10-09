using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using Utilities;
using DG.Tweening;
using static DS_Environment.ZoneType;

public class MainMonsterZone : Zone
{
    private const float OVERLAY_POSITION_OFFSET = .2f;

    public override ZON_Option type => ZON_Option.mainMonsterZone;

    protected override bool removeCard(Card card, bool delete = false)
    {
        if (base.removeCard(card, delete))
        {
            if (!card.InAttack)
                card.changeBattlePosition();
            return true;
        }
        else
            return false;
    }


    public override void Position()
    {
        if(cards.Count >1)
        {
            for(int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.DOMove(new Vector3(transform.position.x + (transform.position.x * OVERLAY_POSITION_OFFSET * i), transform.position.y, transform.position.z), PositionAnimationTime);
            }
        }
        else
        {
            base.Position();
        }
    }

    public override void DoubleClicked(Card card)
    {
        base.DoubleClicked(card);
        if (cards.IndexOf(card) == 0)
            BTN_Handler.BTN_Click(card, BTN_Option.changeBattlePosition);
        else
            BTN_Handler.BTN_Click(card, BTN_Option.toGraveyard);
    }
}
