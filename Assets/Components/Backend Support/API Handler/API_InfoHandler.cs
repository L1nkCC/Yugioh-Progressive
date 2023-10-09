using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Utilities;

[System.Serializable]
public static class API_InfoHandler
{
    private static SerializableDictionary<int, CardInfo> cardInfoDictionary;
    public const string URL_BASE = "https://db.ygoprodeck.com/api/v7/cardinfo.php?";
    public const string URL_ID = URL_BASE + "id=";
    private const int MAX_CARD_ID_REQUEST_VOLUME = 50;
    private static string ROOT_PATH => Application.persistentDataPath + Path.AltDirectorySeparatorChar + "ygocollectionAPI";
    private const string DICTIONARY_FILE_NAME = "AllLocalCards.json";

    private static void AddCardInfoToDictionary(CardInfo info)
    {
        if (cardInfoDictionary == null)
            LoadDictionaryJson();
        if (!cardInfoDictionary.ContainsKey(info.id))
            cardInfoDictionary.Add(info.id, info);
    }

    private static RequestInfo GetRequestInfo(List<string> urls)
    {
        List<CardInfo> cardInfos = new List<CardInfo>();
        foreach(string url in urls)
        {
            cardInfos.AddRange(GetRequestInfo(url).data);
        }
        RequestInfo request = new RequestInfo();
        request.data = cardInfos.ToArray();
        return request;
    }

    private static RequestInfo GetRequestInfo(string url)
    {
        Debug.Log("Requesting Card information from YGOprodeck API");
        Debug.Log(url+"&misc=yes");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "&misc=yes");
        
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using StreamReader reader = new StreamReader(response.GetResponseStream());
            string requestJson = reader.ReadToEnd();
            return JsonUtility.FromJson<RequestInfo>(requestJson);
        }
        catch
        {
            Debug.LogWarning("Request Failed");
            RequestInfo requestInfo = new RequestInfo();
            requestInfo.data = new CardInfo[0];
            return requestInfo;
        }
    }

    public static CardInfo GetCardInfo(int cardId)
    {
        if(cardInfoDictionary == null)
            LoadDictionaryJson();

        if (!cardInfoDictionary.ContainsKey(cardId))
        {
            RequestInfo individualCardWrapped = GetRequestInfo(URL_ID + cardId);
            Debug.Log(cardId + " Requested ID");
            AddCardInfoToDictionary(individualCardWrapped.data[0]);
            SaveDictionaryJson();
        }
        return cardInfoDictionary.GetValueOrDefault(cardId);
    }

    public static CardInfo[] GetCardInfo(int[] cardIds)
    {
        if (cardInfoDictionary == null)
            LoadDictionaryJson();
        CardInfo[] parallelInfo = new CardInfo[cardIds.Length];
        bool needToRequest = false;
        List<string> requestUrl = new List<string>();
        int requestUrlIndex = -1;
        int thisRequestCount = 0;
        for(int i = 0; i < cardIds.Length; i++)
        {
            if (cardInfoDictionary.ContainsKey(cardIds[i]))
            {
                parallelInfo[i] = cardInfoDictionary.GetValueOrDefault(cardIds[i]);
            }
            else
            {
                if(thisRequestCount == 0)
                {
                    requestUrl.Add(URL_ID);
                    requestUrlIndex++;
                }
                requestUrl[requestUrlIndex] += cardIds[i] + ",";
                needToRequest = true;
                thisRequestCount++;
                if(thisRequestCount > MAX_CARD_ID_REQUEST_VOLUME)
                {
                    thisRequestCount = 0;
                }
            }
        }
        if (needToRequest)
        {
            RequestInfo requestInfo = GetRequestInfo(requestUrl);

            for(int i = 0; i < cardIds.Length; i++)
            {
                foreach(CardInfo info in requestInfo.data)
                {
                    if(cardIds[i] == info.id)
                    {
                        parallelInfo[i] = info;
                        AddCardInfoToDictionary(info);
                    }
                }
            }
        }
        SaveDictionaryJson();

        return parallelInfo;
    }




    public static CardInfo[] GetSearchInfo(SearchRequest searchRequest)
    {
        RequestInfo requestInfo = GetRequestInfo(searchRequest.GetSearchURL());
        foreach(CardInfo info in requestInfo.data)
        {
            AddCardInfoToDictionary(info);
        }
        SaveDictionaryJson();
        return requestInfo.data;
    }

    //https://support.unity.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
    public static bool SaveDictionaryJson(int attempt = 100,  string path = DICTIONARY_FILE_NAME)
    {
        if (attempt < 0)
        {
            throw new System.Exception("Save Dictionary Did not work");
        }

        string json = JsonUtility.ToJson(cardInfoDictionary);
        IO.AssureDirectory(ROOT_PATH);
        try
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(ROOT_PATH,path)))
            {
                writer.Write(json);
                writer.Close();
            }
            return true;
        }
        catch (IOException)
        {
            return SaveDictionaryJson(attempt-1);
        }
    }

    public static bool LoadDictionaryJson(string path = DICTIONARY_FILE_NAME)
    {
        try
        {
            using (StreamReader reader = new StreamReader(Path.Combine(ROOT_PATH, path))) { 
                string json = reader.ReadToEnd();
                cardInfoDictionary = JsonUtility.FromJson<SerializableDictionary<int, CardInfo>>(json);
            }
            return true;
        }
        catch (IOException)
        {
            Debug.LogError("IOEXception Caught. Dictionary intialized");
            cardInfoDictionary = new();
            SaveDictionaryJson();
            return false;
        }
    }

    public static void Print(CardInfo info)
    {
        Debug.Log(info.id);
        Debug.Log(info.name);
        Debug.Log(info.type);
        Debug.Log(info.frameType);
        Debug.Log(info.desc);
        Debug.Log(info.atk);
        Debug.Log(info.def);
        Debug.Log(info.level);
        Debug.Log(info.race);
        Debug.Log(info.attribute);
        foreach (CardSetInfo setInfo in info.card_sets)
        {
            Debug.Log(setInfo.set_name);
        }
        foreach (CardImageURLs urls in info.card_images)
        {
            Debug.Log(urls.image_url);
        }
    }


}
