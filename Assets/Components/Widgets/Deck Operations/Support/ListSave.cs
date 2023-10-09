using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Netcode;

public static class ListSave
{
    private static readonly string ROOT_DECK_DIRECTORY = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "deckLists";
    private static readonly string ROOT_POOL_DIRECTORY = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "poolLists";
    private const string JSON_FILE_EXTENSION = ".json";
    private const string DEFAULT_LIST_NAME = "Defualt";
    private static readonly string DEFAULT_DECK_PATH= Application.persistentDataPath + Path.AltDirectorySeparatorChar + "defaultDeck.txt";
    private static readonly string DEFAULT_POOL_PATH = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "defaultPool.txt";

    public enum ListType
    {
        fail,
        deck,
        pool,
    }

    [System.Serializable]
    public struct CardCollectionList : INetworkSerializable
    {
        public string name;
        public int[] mainDeck;
        public int[] sideDeck;
        public int[] extraDeck;
        public ListType listType;

        public CardCollectionList(ListType _listType)
        {
            name = DEFAULT_LIST_NAME;
            mainDeck = new int[0];
            sideDeck = new int[0];
            extraDeck = new int[0];
            listType = _listType;
        }

        public CardCollectionList(string _name, ListType _listType)
        {
            name = _name;
            mainDeck = new int[0];
            sideDeck = new int[0];
            extraDeck = new int[0];
            listType = _listType;
        }
        public CardCollectionList(string _name, ListType _listType, int[] _mainDeck)
        {
            name = _name;
            mainDeck = _mainDeck;
            sideDeck = new int[0];
            extraDeck = new int[0];
            listType = _listType;
        }
        public CardCollectionList(string _name, ListType _listType , int[] _mainDeckCards, int[] _sideDeckCards, int[] _extraDeckCards)
        {
            name = _name;
            mainDeck =_mainDeckCards;
            sideDeck =_sideDeckCards;
            extraDeck =_extraDeckCards;
            listType = _listType;
        }
        public CardCollectionList(CardCollectionList copyFrom)
        {
            name = copyFrom.name;
            mainDeck = copyFrom.mainDeck;
            sideDeck = copyFrom.sideDeck;
            extraDeck = copyFrom.extraDeck;
            listType = copyFrom.listType;
        }

        public bool IsValidDeck()
        {
            return mainDeck.Length >= BuilderMainDeckZone.LOWER_VALID_CARD_COUNT && mainDeck.Length <= BuilderMainDeckZone.UPPER_VALID_CARD_COUNT && 
                sideDeck.Length >= BuilderSideDeckZone.LOWER_VALID_CARD_COUNT && sideDeck.Length <= BuilderSideDeckZone.UPPER_VALID_CARD_COUNT && 
                extraDeck.Length >= BuilderExtraDeckZone.LOWER_VALID_CARD_COUNT && extraDeck.Length <= BuilderExtraDeckZone.UPPER_VALID_CARD_COUNT &&
                listType != ListType.fail && !string.IsNullOrWhiteSpace(name);
        }

        public static int[] GetIds(Card[] cards)
        {
            int[] ids = new int[cards.Length];
            for(int i = 0; i < cards.Length; i++)
            {
                ids[i] = cards[i].Info.id;
            }
            return ids;
        }
        public override string ToString()
        {
            return  "Deck Name: " + name + "\n\tMain Deck Length: " + mainDeck.Length + "\n\tSide Deck Length: " 
                + sideDeck.Length + "\n\tExtra Deck Length: " + extraDeck.Length + "\n\tPool: " + listType;
        }

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref mainDeck);
            serializer.SerializeValue(ref sideDeck);
            serializer.SerializeValue(ref extraDeck);
            serializer.SerializeValue(ref listType);
        }
        // ~INetworkSerializable
    }

    private static string GetPath(CardCollectionList List)
    {
        return GetPath(List.name, List.listType);
    }
    private static string GetPath(string name, ListType listType)
    {
        return GetDirectory(listType) + Path.AltDirectorySeparatorChar + name + JSON_FILE_EXTENSION;
    }
    private static string GetDirectory(ListType listType)
    {
        if (listType == ListType.pool)
            return ROOT_POOL_DIRECTORY;
        if (listType == ListType.deck)
            return ROOT_DECK_DIRECTORY;
        return "";
    }
    public static string GetDefaultPath(ListType listType)
    {
        if (listType == ListType.pool)
            return DEFAULT_POOL_PATH;
        if (listType == ListType.deck)
            return DEFAULT_DECK_PATH;
        return "";
    }

    public static string[] GetListNames(ListType listType)
    {
        Utilities.IO.AssureDirectory(GetDirectory(listType));
        string[] ListsPaths = Directory.GetFiles(GetDirectory(listType));
        string[] ListsNames = new string[ListsPaths.Length];
        for(int i = 0; i < ListsPaths.Length; i++)
        {
            ListsNames[i] = ListsPaths[i].Replace(GetDirectory(listType), "").Replace(JSON_FILE_EXTENSION, "").Replace("/","").Replace("\\","");;
        }
        return ListsNames;
    }

    public static bool ListNameUniquenessValidator(string possibleName, ListType listType)
    {
        if (possibleName == null)
            return false;
        foreach(string usedName in GetListNames(listType))
        {
            if (possibleName.Equals(usedName))
            {
                return false;
            }
        }
        return true;
    }

    public static bool SaveList(CardCollectionList List, int attempt = 100)
    {
        if (attempt < 0)
        {
            return false;
        }


        string json = JsonUtility.ToJson(List, true);
        Utilities.IO.AssureDirectory(GetDirectory(List.listType));
        try
        {
            using (StreamWriter writer = new StreamWriter(GetPath(List)))
            {
                writer.Write(json);
                writer.Close();
                Debug.Log("Save: " + GetPath(List));
            }
            return true;
        }
        catch (IOException)
        {
            return SaveList(List, attempt - 1);
        }
    }

    public static CardCollectionList LoadList(string name, ListType listType)
    {
        try
        {
            using (StreamReader reader = new StreamReader(GetPath(name, listType)))
            {
                string json = reader.ReadToEnd();
                return JsonUtility.FromJson<CardCollectionList>(json);
            }
        }
        catch (IOException)
        {
            Debug.Log("LoadList Failed");
            CardCollectionList returnList = new();
            returnList.listType = ListType.fail;
            return returnList;
        }
    }

    public static void SetDefaultList(string name, ListType listType)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(GetDefaultPath(listType)))
            {
                writer.Write(name);
                writer.Close();
            }
        }
        catch (IOException)
        {
            Debug.LogWarning("SetDefaultList failed");
        }
    }

    public static CardCollectionList GetDefaultList(ListType listType)
    {
        string ListName = "";
        try
        {
            using (StreamReader reader = new StreamReader(GetDefaultPath(listType)))
            {
                ListName = reader.ReadToEnd();
            }
            CardCollectionList List = LoadList(ListName, listType);
            if (List.listType != ListType.fail)
                return List;

        }
        catch (IOException)
        {
        }
        try
        {
            ListName = GetListNames(listType)[0];
            Debug.Log("LIST NAME: " + ListName);
            SetDefaultList(ListName, listType);//Set Default deck list if one is not selected
            return LoadList(ListName,listType);
        }
        catch { };

        CardCollectionList defaultList = new CardCollectionList(listType);
        SaveList(defaultList);
        SetDefaultList(defaultList.name, listType);
        return defaultList;


    }



    public static void DeleteList(string name, ListType listType)
    {
        System.IO.File.Delete(GetPath(name, listType));
    }
}
