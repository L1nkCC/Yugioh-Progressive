using UnityEngine;
using System.IO;
using static API_TextureHandler;
using static API_InfoHandler;
using DS_Environment;
using static DS_Environment.ZoneType;
public class tmpLoadDeckPlayScene : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Player opponent;
    [SerializeField] int[] testCardIds;
    [SerializeField] bool runTestCards;
    private void Start()
    {
        if (runTestCards)
            (PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZON_Option.mainDeckZone, DuelistDesignation.Player) as MainDeck).Load(testCardIds);
        else
        {
            ListSave.CardCollectionList playerDeckList = player.deckList;
            PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZON_Option.mainDeckZone, DuelistDesignation.Player).Load(playerDeckList.mainDeck);
            PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZON_Option.extraDeckZone, DuelistDesignation.Player).Load(playerDeckList.extraDeck);
        }
        if (opponent != null)
        {
            ListSave.CardCollectionList opponentDeckList = opponent.deckList;
            PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZON_Option.mainDeckZone, DuelistDesignation.Opponent).Load(opponentDeckList.mainDeck);
            PlaySceneGameObjects.Board.GetComponent<Board>().GetZone(ZON_Option.extraDeckZone, DuelistDesignation.Opponent).Load(opponentDeckList.extraDeck);

        }
    }
}
