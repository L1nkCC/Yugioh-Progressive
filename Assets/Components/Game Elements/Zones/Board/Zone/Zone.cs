using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DS_Environment;
using DG.Tweening;
using NetworkCommunication;
using static DS_Environment.ZoneType;

public abstract class Zone : MonoBehaviour, IDropHandler
{
    [SerializeField] protected float PositionAnimationTime = .1f;

    public List<Card> cards = new List<Card>();
    public Board board { get; set; }
    public int boardIndex { get; set; }
    public Card TopCard => (cards.Count > 0) ? cards[0] : null;
    public int Count => cards.Count;
    protected virtual Vector2 CARD_SIZE => new Vector2(110.77f, 160f);
    [SerializeField] public DuelistDesignation duelist = DuelistDesignation.Player;
    public abstract ZON_Option type { get; }

    //policies
    protected virtual bool PLCY_moveToFacedown => false;//always flip faceup unless this
    public virtual bool PLCY_copyOnGrab => false;
    public const bool PLCY_showMovementButtons = false;//for later implementation
    public const bool PLCY_debug = false;
    

    protected virtual void Awake()
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
    }
    public virtual void moveCard(Card card, int index = 0)
    {

        if (cards.Contains(card) && cards.IndexOf(card) == index)//checks to see if it is the same zone it is already in
        {
            Debug.Log("Card is already contained in this");
        }
        else
        {
            if (card.Zone != null)
            {
                card.Zone.removeCard(card);
            }
            addCard(card, index);
            card.RotateForDuelistChange();
        }

        Position();
    }
    protected virtual void addCard(Card card, int index)
    {
        card.Zone = this;
        cards.Insert(index, card);

        card.changeSize(CARD_SIZE);

        if (PLCY_moveToFacedown)
        {
            if (card.Faceup) card.flip();
        }
        else
        {
            //flip all faceup
            if (!card.Faceup) card.flip();
        }


    }

    //remove card from its zone
    protected virtual bool removeCard(Card card, bool delete = false)
    {
        if (card.Zone == null)
        {
            Destroy(card.gameObject);
            return false;
        }

        if (card.Zone.PLCY_copyOnGrab && !delete)
        {
            Card copyCard = card.Copy();
        }


        bool successOfRemove = card.Zone.cards.Remove(card);
        if (successOfRemove)
        {
            card.Zone.Position();
            card.Zone = null;
        }

        if (delete)
            Destroy(card.gameObject);

        return successOfRemove;
    }
    /// <summary>
    /// Position elements visually
    /// </summary>
    public virtual void Position() 
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            cards[i].transform.SetParent(PlaySceneGameObjects.CardHolder.transform);
            cards[i].toFrontOfScreen();
            cards[i].transform.DOMove(transform.position, PositionAnimationTime);
        }
        
    }

    public void Load(int[] deckList)
    {
        if (deckList == null)
            return;
        CardInfo[] deckInfo = API_InfoHandler.GetCardInfo(deckList);
        Load(deckInfo);
    }
    public virtual void Load(CardInfo[] deckInfo)
    {
        Empty();
        foreach (CardInfo info in deckInfo)
        {
            Card.Instantiate(info, this, 0);
        }
        Position();
    }

    private void Empty()
    {
        for(int i = cards.Count-1; i>=0; i--)
        {
            Delete(cards[i]);
        }
    }


    public virtual void DoubleClicked(Card card) {}

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        Card droppedCard;
        if (eventData.pointerDrag.TryGetComponent<Card>(out droppedCard))
        {
            if (droppedCard.ZoneClientDuelistMatch)
            {
                if (NetworkStatus.isConnectedToPlayer())
                {
                    Player.connection.MoveCardServerRPC(new CardNetworkMovement(droppedCard, this));
                }
                else
                {
                    board.toZone(droppedCard, this);
                }
                droppedCard.Drop();
            }
        }
    }

    public CardInfo[] ContainsCardIDs()
    {
        CardInfo[] infos = new CardInfo[cards.Count];
        for(int i = 0; i < cards.Count; i++)
        {
            infos[i] = cards[i].Info;
        }
        return infos;
    }

    public bool isEmpty()
    {
        return cards.Count == 0;
    }

    protected void Delete(Card card)
    {
        removeCard(card, true);
    }
}
