using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderPoolZone : BuilderSearchZone
{
    public override ZoneType.ZON_Option type => ZoneType.ZON_Option.b_poolZone;
    public override bool PLCY_copyOnGrab => false;
    protected override int MAX_INSTANCES_OF_CARDS => 3;
    protected override int GUI_ZONE_SPOTS_NUMBER => this.transform.childCount-1;//remove PageTurner from count
    protected override HashSet<ZoneType.ZON_Option> AcceptMovedCardsFromTheseZones => new HashSet<ZoneType.ZON_Option>() { ZoneType.ZON_Option.b_searchZone, ZoneType.ZON_Option.b_poolZone };

}
