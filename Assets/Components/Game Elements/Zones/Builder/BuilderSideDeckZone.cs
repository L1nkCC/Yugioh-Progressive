using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class BuilderSideDeckZone : BuilderZone
{
    public override ZoneType.ZON_Option type => ZoneType.ZON_Option.b_sideDeckZone;

    public const int UPPER_VALID_CARD_COUNT = 15;
    public const int LOWER_VALID_CARD_COUNT = 0;

    protected override int MAX_INSTANCES_OF_CARDS => 3;

    protected override int GUI_ZONE_SPOTS_NUMBER => UPPER_VALID_CARD_COUNT;
}
