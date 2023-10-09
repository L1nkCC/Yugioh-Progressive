using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
public static class BTN_Handler
{
    //button options
    //order they will be shown


    public static readonly System.Func<Card, bool>[] applic = new System.Func<Card, bool>[] {
            (Card card) => {return Zone.PLCY_debug; },
            (Card card) => {return card.Zone.type != ZON_Option.mainDeckZone; },
            (Card card) => {return card.Zone.type == ZON_Option.handZone && card.Info.isMainDeckMonster() && !PHA_Handler.InBattlePhase; },
            (Card card) => {return card.Zone.type == ZON_Option.mainMonsterZone && !PHA_Handler.InBattlePhase && !card.Faceup; },
            (Card card) => {return card.Zone.type != ZON_Option.mainMonsterZone && card.Zone.type != ZON_Option.linkZone && card.Info.isMonster() && card.Faceup; },
            (Card card) => {return card.Zone.type != ZON_Option.mainMonsterZone && card.Zone.type != ZON_Option.linkZone && card.Info.isMonster() && card.Faceup; },
            (Card card) => {return card.Zone.type == ZON_Option.handZone; },
            (Card card) => {return card.Zone.type.IsOnFieldType(); },
            (Card card) => {return card.Zone.type.IsOnFieldType() || card.Zone.type == ZON_Option.handZone; },
            (Card card) => {return card.Zone.type != ZON_Option.graveyardZone; },
            (Card card) => {return card.Zone.type != ZON_Option.banishZone; },
            (Card card) => {return card.Zone.type != ZON_Option.banishZone; },
            (Card card) => {return card.Zone.type != ZON_Option.extraDeckZone && (card.Info.isPendulum() || card.Info.isExtraDeckMonster()); },
            (Card card) => {return card.Zone.type != ZON_Option.mainDeckZone && !(card.Info.isExtraDeckMonster()); },
            (Card card) => {return card.Zone.type == ZON_Option.mainDeckZone; },
            (Card card) => {return card.Zone.type == ZON_Option.mainDeckZone; },
            (Card card) => {return card.Zone.type != ZON_Option.mainBackrowZone && card.Info.isSpellTrap(); },
            (Card card) => {return Zone.PLCY_debug; },
            (Card card) => {return card.Zone.type != ZON_Option.handZone && !card.Info.isExtraDeckMonster(); },
            (Card card) => {return card.Zone.type == ZON_Option.mainDeckZone; },
            (Card card) => {return Zone.PLCY_debug; },
            (Card card) => {return !card.Faceup && (PlaySceneGameObjects.Client.GetComponent<Player>().duelist == card.Zone.duelist || Zone.PLCY_debug); },
            (Card card) => {return card.Zone.type != ZON_Option.linkZone; },
            (Card card) => {return card.Zone.type == ZON_Option.mainMonsterZone && !card.Info.isLinkMonster(); },
            (Card card) => {return card.Zone.type == ZON_Option.mainMonsterZone && card.InAttack; },
            (Card card) => {return card.Zone.Count > 1 && (PlaySceneGameObjects.Client.GetComponent<Player>().duelist == card.Zone.duelist || Zone.PLCY_debug); },
            (Card card) => {return card.Info.isFieldSpell() && card.Zone.type != ZON_Option.fieldSpellZone; },
            (Card card) => {return card.Zone.type != ZON_Option.mainDeckZone && !(card.Info.isExtraDeckMonster()); },

    };



    #region Applicable Buttons regarding Policies, Card Status, Card type, Phase, and Zone

    #region Policy Button Masks
    //all buttons that start with BTN_to... are movement options
    private static readonly HashSet<BTN_Option> BTN_MOVE_OPTIONS = new HashSet<BTN_Option>()
    {
        BTN_Option.toGraveyard,
        BTN_Option.toBanish,
        BTN_Option.toBanishFacedown,
        BTN_Option.toHand,
        BTN_Option.toTopOfDeck,
        BTN_Option.toExtraDeck,
        BTN_Option.toSpellTrap,
        BTN_Option.toLink,
        BTN_Option.toBottomOfDeck,
    };

    private static readonly HashSet<BTN_Option> BTN_DEBUG_OPTIONS = new HashSet<BTN_Option>()
    {
        BTN_Option.unassigned,
        BTN_Option.inspect,
    };

    public static readonly HashSet<BTN_Option> BTN_CLIENT_ONLY_OPTIONS = new HashSet<BTN_Option>()
    {
        BTN_Option.view,
    };

    private static void BTN_PolicyMask(Card card, ref HashSet<BTN_Option> options)
    {
        if (!Zone.PLCY_showMovementButtons)
        {
            options.ExceptWith(BTN_MOVE_OPTIONS);
        }
        if (!Zone.PLCY_debug)
        {
            options.ExceptWith(BTN_DEBUG_OPTIONS);
        }
    }
    #endregion
    public static HashSet<BTN_Option> BTN_Applicable(Card card)
    {
        HashSet<BTN_Option> applicableOptions = new();

        foreach (BTN_Option option in System.Enum.GetValues(typeof(BTN_Option)))
        {
            if (applic[(int)option](card))
                applicableOptions.Add(option);
        }

        BTN_PolicyMask(card, ref applicableOptions);
        return applicableOptions;
    }
    #endregion



    //must be parallel to Enumeration
    public static System.Action<Card>[] BTN_Actions = new System.Action<Card>[]
    {
        new System.Action<Card>(_unassigned),
        new System.Action<Card>(_declare),
        new System.Action<Card>(_normalSummon),
        new System.Action<Card>(_flipSummon),
        new System.Action<Card>(_specialSummonAttack),
        new System.Action<Card>(_specialSummonDefense),
        new System.Action<Card>(_set),
        new System.Action<Card>(_flip),
        new System.Action<Card>(_activate),
        new System.Action<Card>(_toGraveyard),
        new System.Action<Card>(_toBanish),
        new System.Action<Card>(_toBanishFacedown),
        new System.Action<Card>(_toExtraDeck),
        new System.Action<Card>(_toTopOfDeck),
        new System.Action<Card>(_mill),
        new System.Action<Card>(_shuffleDeck),
        new System.Action<Card>(_toSpellTrap),
        new System.Action<Card>(_move),
        new System.Action<Card>(_toHand),
        new System.Action<Card>(_draw),
        new System.Action<Card>(_inspect),
        new System.Action<Card>(_reveal),
        new System.Action<Card>(_toLink),
        new System.Action<Card>(_changeBattlePosition),
        new System.Action<Card>(_attack),
        new System.Action<Card>(_view),
        new System.Action<Card>(_toFieldZone),
        new System.Action<Card>(_toBottomOfDeck),
    };

    private static string BTN_Name(BTN_Option option)
    {
        return option switch
        {
            BTN_Option.declare => "Declare",
            BTN_Option.normalSummon => "Norm Summon",
            BTN_Option.specialSummonAttack => "SS Atk",
            BTN_Option.specialSummonDefense => "SS Def",
            BTN_Option.flipSummon => "Flip Summon",
            BTN_Option.set => "Set",
            BTN_Option.flip => "Flip",
            BTN_Option.activate => "Activate",
            BTN_Option.toGraveyard => "to GY",
            BTN_Option.toBanish => "Banish",
            BTN_Option.toBanishFacedown => "Banish FD",
            BTN_Option.toExtraDeck => "to Ex. Deck",
            BTN_Option.toTopOfDeck => "to Deck top",
            BTN_Option.mill => "Mill",
            BTN_Option.shuffleDeck => "Shuffle",
            BTN_Option.toSpellTrap => "to S/T",
            BTN_Option.move => "Move",
            BTN_Option.toHand => "to Hand",
            BTN_Option.draw => "Draw",
            BTN_Option.inspect => "Inspect",
            BTN_Option.reveal => "Reveal",
            BTN_Option.toLink => "Link Summon",
            BTN_Option.changeBattlePosition => "Change BP",
            BTN_Option.attack => "Attack",
            BTN_Option.view => "View",
            BTN_Option.toFieldZone => "to FS",
            BTN_Option.toBottomOfDeck => "to Deck Bot",
            _ => "Unnamed",
        };
    }

    //have acess to Button Actions
    public static void BTN_Click(Card card, BTN_Option option)
    {
        if (NetworkStatus.isConnectedToPlayer() && !BTN_CLIENT_ONLY_OPTIONS.Contains(option))
        {
            Player.connection.BTN_ActionServerRPC(new NetworkCommunication.CardNetworkMovement.CardLocation(card), option);
        }
        else
        {
            BTN_Actions[(int)option](card);
        }
    }
    public static void BTN_Assign(BTN_Option option, System.Action<Card> action)
    {
        BTN_Actions[(int)option] += action;
    }

    #region Button Press Methods
    /*
    These are the methods associated with each button press
    Note that they do not take into consideration whether the action is viable (unless it would crash; all are crash safe)
    using these directly might cause unnormal occurance
     */

    private static void _declare(Card card)
    {
	    
    }
    private static void _normalSummon(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.mainMonsterZone, card.Zone.duelist, true);
    }
    private static void _specialSummonAttack(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.mainMonsterZone, card.Zone.duelist, true);
    }
    private static void _specialSummonDefense(Card card)
    {
        card.changeBattlePosition();
        card.Zone.board.toZone(card, ZON_Option.mainMonsterZone, card.Zone.duelist, true);
    }
    private static void _flipSummon(Card card)
    {
        if (!card.Faceup)
        {
            _flip(card);
            card.changeBattlePosition();
        }
    }
    private static void _set(Card card)
    {
        bool success = false;


        if (card.Info.isMonster())
        {
            if (card.Zone.board.toZone(card, ZON_Option.mainMonsterZone, card.Zone.duelist, true))
            {
                card.changeBattlePosition();
                success = true;
            }
        }
        if (card.Info.isPendulum())
        {
            success = card.Zone.board.toZone(card, ZON_Option.pendulumZone, card.Zone.duelist, true);
        }
        if (card.Info.isFieldSpell())
        {
            success = card.Zone.board.toZone(card, ZON_Option.fieldSpellZone, card.Zone.duelist, true);
        }
        if (card.Info.isSpellTrap())
        {
            success = card.Zone.board.toZone(card, ZON_Option.mainBackrowZone, card.Zone.duelist, true);
        }
        if (success)
        {
            card.flip();
        }
        
    }
    private static void _flip(Card card)
    {
        card.flip();
        if((card.Zone.type == ZON_Option.mainMonsterZone || card.Zone.type == ZON_Option.linkZone) && (card.Faceup && card.InAttack))
        {
            card.changeBattlePosition();
        }
    }
    private static void _activate(Card card)
    {
        if (card.Zone.type == ZON_Option.handZone)
        {
            if (card.Info.isPendulum())
                card.Zone.board.toZone(card, ZON_Option.pendulumZone, card.Zone.duelist);
            if (card.Info.isFieldSpell())
                card.Zone.board.toZone(card, ZON_Option.mainBackrowZone, card.Zone.duelist);
            if (card.Info.isSpellTrap())
                _toSpellTrap(card);
        }
        else
        {
            if(card.Zone.type == ZON_Option.mainBackrowZone && !card.Faceup)
            {
                card.flip();
            }
        }
    }
    private static void _toGraveyard(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.graveyardZone, card.Zone.duelist);
    }
    private static void _toBanish(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.banishZone, card.Zone.duelist);
    }
    private static void _toBanishFacedown(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.banishZone, card.Zone.duelist);
        card.flip();
    }
    private static void _toExtraDeck(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.extraDeckZone, card.Zone.duelist);
    }
    private static void _toTopOfDeck(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.mainDeckZone, card.Zone.duelist);
    }
    private static void _mill(Card card)
    {
        if(card.Zone.type == ZON_Option.mainDeckZone)
            card.Zone.board.toZone(card, ZON_Option.graveyardZone, card.Zone.duelist);
    }
    private static void _shuffleDeck(Card card)
    {
	    if(card.Zone.type == ZON_Option.mainDeckZone)
        {
            (card.Zone as Deck).Shuffle();
        }
    }
    private static void _toSpellTrap(Card card) //is referenced by another button press method
    {
        card.Zone.board.toZone(card, ZON_Option.mainBackrowZone, card.Zone.duelist, true);
    }
    private static void _move(Card card)
    {
	    
    }
    private static void _toHand(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.handZone, card.Zone.duelist);
    }
    private static void _draw(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.handZone, card.Zone.duelist);
    }
    private static void _inspect(Card card)
    {
        //return if trying to look at an opponents facedown card
        if (SceneManagement.inMultiPlayScene() && (ulong)card.Zone.duelist != Unity.Netcode.NetworkManager.Singleton.LocalClientId && !card.Faceup)
            return;

        if (card.Zone.type != ZON_Option.mainDeckZone && card.Zone.type != ZON_Option.extraDeckZone)//required so that a direct call from a right click will not do anything
        {
            DS_Environment.PlaySceneGameObjects.Inspect.GetComponent<INS_Manager>().DisplayCard(card);
        }
    }
    private static void _reveal(Card card)
    {

    }
    private static void _toLink(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.linkZone, card.Zone.duelist);
    }
    private static void _changeBattlePosition(Card card)
    {
        card.changeBattlePosition();
    }
    private static void _attack(Card card)
    {

    }
    private static void _view(Card card)
    {
        if (PlaySceneGameObjects.ViewZone.activeSelf)
            ViewMenu.Hide();
        ViewMenu.View(card.Zone);
    }
    private static void _toFieldZone(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.fieldSpellZone, card.Zone.duelist);
    }
    private static void _toBottomOfDeck(Card card)
    {
        card.Zone.board.toZone(card, ZON_Option.mainDeckZone, card.Zone.duelist, mustBeEmpty: false, back: false);
    }
    private static void _unassigned(Card card)
    {

    }
    #endregion


    public static class BTN_Menu_Handler
    {
        private static GameObject BTN_prefab => Resources.Load<GameObject>(DS_Environment.ResourcePath.BTN_button); //Actually an expensive operation
        private static HashSet<GameObject> BTN_instances = new HashSet<GameObject>();
        public static void BTN_Menu_Open(Card card)
        {

            BTN_Menu_Close();
            int count = 0;
            foreach (BTN_Option option in BTN_Handler.BTN_Applicable(card))
            {
                GameObject button = GameObject.Instantiate<GameObject>(BTN_prefab, card.gameObject.transform);
                Rect buttonRect = button.GetComponent<RectTransform>().rect;
                button.transform.localPosition = new Vector3(buttonRect.width, buttonRect.height/2f) + new Vector3(0, BTN_prefab.GetComponent<RectTransform>().rect.height) * count;
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = BTN_Name(option);
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => { BTN_Click(card,option); BTN_Menu_Close(); });
                BTN_instances.Add(button);

                count++;
            }
        }
        public static void BTN_Menu_Close()
        {
            foreach (GameObject button in BTN_instances)
            {
                GameObject.Destroy(button);
            }
            BTN_instances.Clear();
        }
    }
}
