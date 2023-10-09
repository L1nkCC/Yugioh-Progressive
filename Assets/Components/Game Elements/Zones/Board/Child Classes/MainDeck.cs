using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
public class MainDeck : Deck
{
    protected override bool PLCY_moveToFacedown => true;
    public override ZON_Option type => ZON_Option.mainDeckZone;
    public override void DoubleClicked(Card card)
    {
        base.DoubleClicked(card);
        BTN_Handler.BTN_Click(card, BTN_Option.draw);
    }
}
