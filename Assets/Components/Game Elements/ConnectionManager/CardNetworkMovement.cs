using DS_Environment;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;


namespace NetworkCommunication
{
    [System.Serializable]
    public struct CardNetworkMovement : INetworkSerializable
    {
        [System.Serializable]
        public struct CardLocation : INetworkSerializable
        {
            public int ZoneIndex;
            public int CardIndex;
            public DuelistDesignation duelist;

            public CardLocation(int _zoneIndex, int _cardIndex, DuelistDesignation _duelist)
            {
                ZoneIndex = _zoneIndex;
                CardIndex = _cardIndex;
                duelist = _duelist;
            }
            public CardLocation(Card card)
            {
                (int _zoneIndex, DuelistDesignation _duelist) = Board.GetZoneDuelistReference(card.Zone);
                ZoneIndex = _zoneIndex;
                duelist = _duelist;
                CardIndex = card.Zone.cards.IndexOf(card);
            }
            public override string ToString()
            {
                return "Duelist: " + System.Enum.GetName(typeof(DuelistDesignation), duelist) + "\tZoneIndex: " + ZoneIndex + "\tCardIndex: " + CardIndex;
            }
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ZoneIndex);
                serializer.SerializeValue(ref CardIndex);
                serializer.SerializeValue(ref duelist);
            }
        }

        public CardLocation origin;
        public CardLocation destination;

        public CardNetworkMovement(Card card, Zone destinationZone, int destinationCardIndex = 0)
        {
            origin = new CardLocation(card);
            (int destinationZoneIndex, DuelistDesignation destinationDuelist) = Board.GetZoneDuelistReference(destinationZone);
            destination = new(destinationZoneIndex, destinationCardIndex, destinationDuelist);
        }
        public override string ToString()
        {
            string returnString = "";
            returnString += "ORIGIN:       " + origin.ToString() + "\n";
            returnString += "DESTINATION:  " + destination.ToString() + "\n";
            return returnString;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref origin);
            serializer.SerializeValue(ref destination);
        }
    }
}