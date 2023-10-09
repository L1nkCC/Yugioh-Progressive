using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
public class MainBackrowZone : Zone
{
    public override ZON_Option type => ZON_Option.mainBackrowZone;
    public override void DoubleClicked(Card card)
    {
        base.DoubleClicked(card);
        BTN_Handler.BTN_Click(card, BTN_Option.flip);
    }
}
