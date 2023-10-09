using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DS_Environment.ZoneType;
using System.Linq;

public abstract class BuilderZone : Zone
{
    protected abstract int GUI_ZONE_SPOTS_NUMBER { get; }
    protected abstract int MAX_INSTANCES_OF_CARDS { get; }

    protected virtual Transform[] positions => GetComponentsInChildren<Transform>().Where(child => child.gameObject != this.gameObject).ToArray();
    protected override Vector2 CARD_SIZE => new Vector2(70f, 101.1f);

    protected virtual bool PLCY_guiZonesIsMaxSize => true;

    public override void Position()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            cards[i].transform.SetParent(PlaySceneGameObjects.CardHolder.transform);
            cards[i].toFrontOfScreen();
            cards[i].transform.position = positions[cards.Count - 1 - i].position;
        }
    }

    public override void moveCard(Card card, int index = 0)
    {
        if (PLCY_guiZonesIsMaxSize && cards.Count > GUI_ZONE_SPOTS_NUMBER)
        {
            Delete(card);
            return;
        }
        
        if (NumberOfCardInstances(card.Info) < MAX_INSTANCES_OF_CARDS)
        {
            base.moveCard(card, index);
        }
        else
        {
            Position();
        }
    }


    public int NumberOfCardInstances(CardInfo info)
    {
        int count = 0;
        foreach(Card card in cards)
        {
            if(card.Info.id == info.id)
            {
                count++;
            }
        }
        return count;
    }

    public override void DoubleClicked(Card card)
    {
        board.toZone(card, ZON_Option.b_searchZone, PlaySceneGameObjects.Player.GetComponent<Player>().duelist);
    }

}
