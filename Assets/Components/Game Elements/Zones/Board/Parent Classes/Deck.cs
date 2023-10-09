using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.ZoneType;
public abstract class Deck : Zone
{


    //FisherYatesShuffle
    public void Shuffle()
    {
        if (NetworkStatus.isConnectedToPlayer())
        {
            Player.connection.ShuffleServerRPC(this.type, this.duelist);
        }
        else
        {
            for (int n = cards.Count - 1; n > 0; n--)
            {
                int k = Random.Range(0, n);
                Card temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
            }
            Position();
        }
    }


}
