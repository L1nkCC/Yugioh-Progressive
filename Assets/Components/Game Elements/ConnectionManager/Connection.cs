using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DS_Environment;
using NetworkCommunication;
using static ListSave;
using YGO_Commands;

public class Connection : NetworkBehaviour
{
    public NetworkVariable<CardCollectionList> PlayerDeck = new NetworkVariable<CardCollectionList>(new CardCollectionList(ListType.fail));
    public NetworkVariable<CardCollectionList> OpponentDeck = new NetworkVariable<CardCollectionList>(new CardCollectionList(ListType.fail));

    public NetworkVariable<bool> PlayerLoadedInMultiplayerScene = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> OpponentLoadedInMultiplayerScene = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Stack<System.Action> cardCommandStack = new Stack<System.Action>();




/*    private async void Update()
    {
        if (cardCommandStack.Count > 0)
        {
            System.Action cmd = cardCommandStack.Pop();
            Debug.Log(cmd.Method.Name);
            await Task.Run(cmd);
        }
    }
*/
    private void Push(System.Action cmd)
    {
        //cardCommandStack.Push(() => { Debug.Log("CMD.Name in Push" + cmd.Method.Name); return new Task(cmd); });
        cmd();
    }

    #region Deck Startup
    [ServerRpc(RequireOwnership = false)]
    public void SetDeckServerRPC(CardCollectionList deck, DuelistDesignation duelist, ServerRpcParams param)
    {
        if (duelist == DuelistDesignation.Player)
        {
            PlayerDeck.Value = deck;
        } else if (duelist == DuelistDesignation.Opponent)
        {
            OpponentDeck.Value = deck;
        }
        Debug.Log("SetDecksClientRPC : " + deck.name + " \tDuelist : " + duelist + " \tClient : " + param.Receive.SenderClientId);

    }
    [ClientRpc]
    public void LoadDecksClientRPC()
    {
        PlaySceneGameObjects.Board.GetComponent<Board>().LoadDeck(PlayerDeck.Value, DuelistDesignation.Player);
        PlaySceneGameObjects.Board.GetComponent<Board>().LoadDeck(OpponentDeck.Value, DuelistDesignation.Opponent);
    }
   
    #endregion



    #region Player Actions
    #region Shuffle
    [ServerRpc(RequireOwnership = false)]
    public void ShuffleServerRPC(ZoneType.ZON_Option ZoneIndex, DuelistDesignation duelist)
    {
        Push(() =>
        {
            List<Card> cards = PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZoneIndex, duelist).cards;
            CardNetworkMovement[] shuffleMovements = new CardNetworkMovement[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                CardNetworkMovement movement;
                movement.origin = new CardNetworkMovement.CardLocation(cards[i]);
                movement.destination = new CardNetworkMovement.CardLocation(cards[i]);
                shuffleMovements[i] = movement;
            }

            for (int n = cards.Count - 1; n > 0; n--)
            {
                int k = Random.Range(0, n);
                CardNetworkMovement.CardLocation temp = shuffleMovements[n].destination;
                shuffleMovements[n].destination = shuffleMovements[k].destination;
                shuffleMovements[k].destination = temp;
            }
            MoveCardsClientRPC(shuffleMovements);
        });
    }

    #endregion


    #region Move Card
    [ServerRpc(RequireOwnership = false)]
    public void MoveCardServerRPC(CardNetworkMovement movement)
    {
        Push(() => MoveCardClientRPC(movement));
    }
    [ClientRpc]
    private void MoveCardClientRPC(CardNetworkMovement movement)
    {
        Board board = PlaySceneGameObjects.Board.GetComponent<Board>();
        Card card = board.GetZone(movement.origin).cards[movement.origin.CardIndex];
        Zone destinationZone = board.GetZone(movement.destination);
        destinationZone.moveCard(card, movement.destination.CardIndex);
    }

    [ClientRpc]
    private void MoveCardsClientRPC(CardNetworkMovement[] movements)
    {
        Board board = PlaySceneGameObjects.Board.GetComponent<Board>();
        (Card card, Zone destZone, int destIndex)[] referencedMovements = new (Card,Zone,int)[movements.Length];
        for(int i = 0; i < movements.Length; i++)
        {
            referencedMovements[i].card = board.GetCard(movements[i].origin);
            referencedMovements[i].destZone = board.GetZone(movements[i].destination);
            referencedMovements[i].destIndex = movements[i].destination.CardIndex;
        }
        foreach((Card card, Zone destZone, int destIndex) referencedMovement in referencedMovements)
        {
            referencedMovement.destZone.moveCard(referencedMovement.card, referencedMovement.destIndex);
        }

    }
    #endregion

    #region BTN_Actions
    [ServerRpc(RequireOwnership = false)]
    public void BTN_ActionServerRPC(CardNetworkMovement.CardLocation target, BTN_Option option)
    {
        Push(() =>
        {
            if (BTN_Handler.applic[(int)option](PlaySceneGameObjects.Board.GetComponent<Board>().GetCard(target)))
            {
                BTN_ActionClientRPC(target, option);
            }
            else
            {
                Debug.LogError("Button Action Refused by applicable restrictions on Host");
            }
        });
    }

    [ClientRpc]
    private void BTN_ActionClientRPC(CardNetworkMovement.CardLocation target, BTN_Option option)
    {
        BTN_Handler.BTN_Actions[(int)option](PlaySceneGameObjects.Board.GetComponent<Board>().GetCard(target));
    }
    #endregion

    #region Chat
    [ServerRpc(RequireOwnership = false)]
    public void CMD_ChatMessageServerRPC(string message)
    {
        Push(() => CMD_ChatMessageClientRPC(message)); //required because it could result in cards moving
    }
    [ClientRpc]
    private void CMD_ChatMessageClientRPC(string message)
    {
        CMD_Manager.PostMessage(message);
    }
    #endregion

    #region LP Tracker
    [ServerRpc(RequireOwnership = false)]
    public void LP_ChangeServerRPC(int change, DuelistDesignation duelist)
    {
        LP_ChangeClientRPC(change, duelist);
    }
    [ClientRpc]
    private void LP_ChangeClientRPC(int change, DuelistDesignation duelist)
    {
        PlaySceneGameObjects.LifePoints.GetComponent<LP_Manager>().ChangeLife(change, duelist);
    }
    #endregion

    #region Phases
    [ServerRpc(RequireOwnership = false)]
    public void PHA_ChangeServerRPC(PHA_Handler.PHA_Option option)
    {
        PHA_ChangeClientRPC(option);
    }
    [ClientRpc]
    private void PHA_ChangeClientRPC(PHA_Handler.PHA_Option option)
    {
        PHA_Handler.PHA_Select(option);
    }
    #endregion
    #endregion
}
