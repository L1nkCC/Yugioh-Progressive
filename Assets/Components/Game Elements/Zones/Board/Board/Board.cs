using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
using NetworkCommunication;
public class Board : MonoBehaviour
{
    [Tooltip("NOTE: Indexing is very important please follow the indexing listed out by ZON_Options")]
    [SerializeField]private Zone[] zones = new Zone[34];

    private void Awake()
    {
        for(int i = 0; i < zones.Length; i++)
        {
            zones[i].board = this;
            zones[i].boardIndex = i;
        }
    }


    public void FlipZoneDesignations()
    {
        foreach(Zone zone in zones)
        {
            if(zone.duelist == DuelistDesignation.Player)
            {
                zone.duelist = DuelistDesignation.Opponent;
            }
            else
            {
                zone.duelist = DuelistDesignation.Player;
            }
        }
    }
    public void LoadDeck(ListSave.CardCollectionList deck, DuelistDesignation duelist)
    {
        GetZone(ZON_Option.mainDeckZone, duelist).Load(deck.mainDeck);
        GetZone(ZON_Option.extraDeckZone, duelist).Load(deck.extraDeck);
    }
    public Card GetCard(CardNetworkMovement.CardLocation location)
    {
        return this.GetZone(location).cards[location.CardIndex];
    }
    #region Get Zone 
    public Zone GetZone(ZON_Option option)
    {
        return GetZone(option, PlaySceneGameObjects.Client.GetComponent<Player>().duelist);
    }

    public Zone GetZone(ZON_Option option, DuelistDesignation duelist)
    {
        return zones[GetZoneIndex(option,duelist)];
    }


    public Zone[] GetZones(ZON_Option option, DuelistDesignation duelist, uint length = MAIN_ZONE_COUNT)
    {
        Zone[] subZones = new Zone[length];
        for (int i = 0; i < subZones.Count(); i++)
        {
            subZones[i] = zones[GetZoneIndex(option,duelist) + i];
        }
        return subZones;
    }

    public Zone GetZone(CardNetworkMovement.CardLocation location)
    {
        return GetZone(location.ZoneIndex, location.duelist);
    }

    private Zone GetZone(int ZoneIndex, DuelistDesignation duelist)
    {
        int index = ZoneIndex;
        if (NetworkStatus.isConnectedToPlayer())
        {
            if ((Unity.Netcode.NetworkManager.Singleton.LocalClientId == (int)DuelistDesignation.Opponent && duelist == DuelistDesignation.Player) ||
                (Unity.Netcode.NetworkManager.Singleton.LocalClientId == (int)DuelistDesignation.Player && duelist == DuelistDesignation.Opponent))
            {
                index += PLAYER_ZONE_COUNT;
            }
        }
        else
        {
            index = duelist == DuelistDesignation.Player ? ZoneIndex : ZoneIndex + PLAYER_ZONE_COUNT;
        }


        return zones[index];
    }
    public static (int, DuelistDesignation) GetZoneDuelistReference(Zone zone)
    {
        int ZoneIndex = zone.boardIndex;
        DuelistDesignation duelist = zone.duelist;
        ZoneIndex = ZoneIndex < PLAYER_ZONE_COUNT ? ZoneIndex : ZoneIndex - PLAYER_ZONE_COUNT;
        return (ZoneIndex, duelist);
    }
    #endregion

    #region toZone

    public bool toZone(Card card, Zone toZone, bool mustBeEmpty = false, bool back = false)
    {
        if (toZone.isEmpty() || !mustBeEmpty)
        {
            int index = back ? toZone.cards.Count : 0;
            toZone.moveCard(card, index);
            return true;
        }
        return false;
    }

    public bool toZone(Card card, ZON_Option toZoneOption, DuelistDesignation duelist, bool mustBeEmpty = false, bool back = false)
    {
        Zone[] applicableZones = new Zone[] { GetZone(toZoneOption, duelist) };
        if(toZoneOption == ZON_Option.mainMonsterZone || toZoneOption == ZON_Option.mainBackrowZone)
        {
            applicableZones = GetZones(toZoneOption, duelist);
        }
        if(toZoneOption == ZON_Option.pendulumZone)
        {
            applicableZones = GetZones(toZoneOption, duelist, PEND_ZONE_COUNT);
        }

        foreach (Zone zone in applicableZones)
        {
            if(toZone(card, zone, mustBeEmpty, back))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Get Zone Index
    /// <summary>
    /// Gets the base Index of a Zone for a Board class. Will not return specific zone for zones with multiple instances ie: Monster, backrow/pendulum. No Board requirement
    /// </summary>
    /// <param name="option"></param>
    /// <param name="duelist"></param>
    /// <returns></returns>
    private static int GetZoneIndex(ZON_Option option, DuelistDesignation duelist)
    {
        int index = (int)option;


        //if in build Scenes
        if (option > ZON_Option.unassigned)
        {
            return index - ((int)ZON_Option.unassigned + 1);
        }

        if (NetworkStatus.isConnectedToPlayer())
        {
            if((Unity.Netcode.NetworkManager.Singleton.LocalClientId == (int)DuelistDesignation.Opponent && duelist == DuelistDesignation.Player) ||
                (Unity.Netcode.NetworkManager.Singleton.LocalClientId == (int)DuelistDesignation.Player && duelist == DuelistDesignation.Opponent))
            {
                index += PLAYER_ZONE_COUNT;
            }
        }
        else
        {
            if (duelist == DuelistDesignation.Opponent)
            {
                index += PLAYER_ZONE_COUNT;
            }
        }
        return index;
    }
    #endregion 
}
