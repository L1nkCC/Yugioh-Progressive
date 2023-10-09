using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Linq;

public class BuilderMainDeckZone : BuilderZone
{
    public override ZoneType.ZON_Option type => ZoneType.ZON_Option.b_mainDeckZone;

    public const int UPPER_VALID_CARD_COUNT = 60;
    public const int LOWER_VALID_CARD_COUNT = 40;

    protected override int MAX_INSTANCES_OF_CARDS => 3;

    protected override int GUI_ZONE_SPOTS_NUMBER => UPPER_VALID_CARD_COUNT;

    public override void moveCard(Card card, int index = 0)
    {
        if (cards.Count < GUI_ZONE_SPOTS_NUMBER)
            base.moveCard(card, index);
        else
            Delete(card);
    }


}
