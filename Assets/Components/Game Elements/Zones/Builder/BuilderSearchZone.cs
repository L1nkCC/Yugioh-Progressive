using DS_Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Linq;
public class BuilderSearchZone : BuilderZone
{
    public override ZoneType.ZON_Option type => ZoneType.ZON_Option.b_searchZone;
    public TMPro.TextMeshProUGUI pageCounter;
    private const int MAX_CAPACITY = 450;

    public CardInfo[] results { get; private set; }
    public CardInfo[] filtered { get; private set; }

    protected override int GUI_ZONE_SPOTS_NUMBER => 30;
    public int page = 0;
    public override bool PLCY_copyOnGrab => true;
    protected override bool PLCY_guiZonesIsMaxSize => false;
    protected override int MAX_INSTANCES_OF_CARDS => 1;

    protected override void Awake()
    {
        base.Awake();
        pageCounter = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void IncrementPage()
    {
        if (results.Length > (page + 1) * GUI_ZONE_SPOTS_NUMBER)
        {
            Debug.Log("ANTE PAGE INCREMENT PAGE = " + page);
            page++;
            Debug.Log("POST PAGE INCREMENT PAGE = " + page);
        }
        UpdatePageCounter();
        LoadPage();
    }
    public void DecrementPage()
    {
        if (page > 0)
            page--;
        UpdatePageCounter();
        LoadPage();
    }

    public void UpdatePageCounter()
    {
        pageCounter.text = System.Convert.ToString(page + 1) + " / " + (filtered.Length / GUI_ZONE_SPOTS_NUMBER + 1);
    }

    public override void moveCard(Card card, int index = 0)
    {
        if (cards.Count < MAX_CAPACITY && (card.Zone == null || AcceptMovedCardsFromTheseZones.Contains(card.Zone.type)))
            base.moveCard(card, index);
        else
            Delete(card);
        
    }
    protected virtual HashSet<ZoneType.ZON_Option> AcceptMovedCardsFromTheseZones => new HashSet<ZoneType.ZON_Option>() { ZoneType.ZON_Option.b_searchZone };


    public override void DoubleClicked(Card card)
    {
        Debug.Log(board.name);
        if(card.Info.isExtraDeckMonster() &&  board.GetZone(ZoneType.ZON_Option.b_extraDeckZone).GetType() == typeof(BuilderExtraDeckZone))
        {
            board.toZone(card, ZoneType.ZON_Option.b_extraDeckZone, PlaySceneGameObjects.Player.GetComponent<Player>().duelist);
        }
        else
        {
            board.toZone(card, ZoneType.ZON_Option.b_mainDeckZone, PlaySceneGameObjects.Player.GetComponent<Player>().duelist);
        }
    }

    public void Filter(CardInfo[] _filter)
    {
        page = 0;
        filtered = _filter;
        LoadPage();
    }

    public override void Load(CardInfo[] deckInfo)
    {
        results = deckInfo;
        Filter(results);
    }
    public void LoadPage()
    {
        CardInfo[] shownCards = new CardInfo[GUI_ZONE_SPOTS_NUMBER];
        if (filtered.Length > (page + 1) * GUI_ZONE_SPOTS_NUMBER)
        {
            System.Array.Copy(filtered, page * GUI_ZONE_SPOTS_NUMBER, shownCards, 0, GUI_ZONE_SPOTS_NUMBER);
        }
        else
        {
            shownCards = filtered.ToList().GetRange(page * GUI_ZONE_SPOTS_NUMBER, filtered.Length - (page * GUI_ZONE_SPOTS_NUMBER)).ToArray();
        }
        base.Load(shownCards);
        UpdatePageCounter();
    }
}
