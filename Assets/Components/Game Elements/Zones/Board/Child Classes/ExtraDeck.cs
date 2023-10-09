using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
public class ExtraDeck : Deck
{
    protected override bool PLCY_moveToFacedown => true;

    public override ZON_Option type => ZON_Option.extraDeckZone;

    protected override void addCard(Card card, int index = 0)
    {
        base.addCard(card, index);
        if (card.Info.isPendulum())
        {
            BTN_Handler.BTN_Click(card, BTN_Option.flip);
        }
    }
}
