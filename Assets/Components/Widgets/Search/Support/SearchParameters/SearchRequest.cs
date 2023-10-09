using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using static DS_Environment.SearchParameters;
using System.Linq;
using System;

public class SearchRequest
{
    
    List<(SRH_Option, string)> parameters = new();

    public SearchRequest()
    {

    }
    public SearchRequest(SRH_Option option, string value)
    {
        parameters.Add((option, value));
    }

    public bool Add(SRH_Option option, string value)
    {
        foreach((SRH_Option _option, string _value) in parameters)
        {
            if (option == _option)
                return false;
        }
        parameters.Add((option, value));
        return true;
    }
    public void Add(SearchRequest otherRequest)
    {
        foreach((SRH_Option option, string value) in otherRequest.parameters)
        {
            Add(option, value);
        }
    }

    public string GetSearchURL()
    {
        parameters.Sort(delegate ((SRH_Option option, string value) element1, (SRH_Option option, string value) element2)
        {
            if ((int)element1.option == (int)element2.option) return 0;
            if ((int)element1.option > (int)element2.option) return 1;
            if ((int)element2.option > (int)element1.option) return -1;
            return 0;
        });

        string url = API_InfoHandler.URL_BASE;

        foreach((SRH_Option option, string value) in parameters)
        {
            url += GetSearchURLParameter(option, value);
            url += "&";
        }
        url = url.Substring(0, url.Length - 1);

        Utilities.String.RemoveZeroWidthSpace(ref url);
        
        return url;
    }

    private static string GetSearchURLParameter(SRH_Option option, string value)
    {
        if (!value.Equals(""))
        {
            return SRH_Name(option) + "=" + value;
        }
        return "";
    }




    public CardInfo[] GetSearchedCollection(ref CardInfo[] deck)
    {
        foreach((SRH_Option option, string value) in parameters)
        {
            deck = deck.Where(LocalParameterFilters(value)[(int)option]).ToArray();
        }
        return deck;
    }


    private static System.Func<CardInfo, bool>[] LocalParameterFilters(string value)
    {
        return new System.Func<CardInfo, bool>[]{
            (CardInfo info) => info.name.Contains(value),
            (CardInfo info) => info.desc.Contains(value),
            (CardInfo info) => info.type.Contains(value),
            (CardInfo info) => info.atk == uint.Parse(value),
            (CardInfo info) => info.def == uint.Parse(value),
            (CardInfo info) => info.level == uint.Parse(value),
            (CardInfo info) => info.race.Equals(value),
            (CardInfo info) => info.attribute.Equals(value),
            (CardInfo info) => info.linkval == int.Parse(value),
            (CardInfo info) => info.linkmarkers.Contains(value),
            (CardInfo info) => info.scale == int.Parse(value),
            (CardInfo info) => {foreach(CardSetInfo setInfo in info.card_sets) { if(setInfo.set_name.Equals(value)) return true; } return false; },
            (CardInfo info) => info.archetype.Equals(value),
            (CardInfo info) => throw new NotImplementedException(),
            (CardInfo info) => throw new NotImplementedException(),
            (CardInfo info) => info.misc_info[0].treated_as.Equals(value),
            (CardInfo info) => throw new NotImplementedException(),
            (CardInfo info) => info.misc_info[0].has_effect == int.Parse(value),
            (CardInfo info) => throw new NotImplementedException(),
            (CardInfo info) => throw new NotImplementedException(),
            (CardInfo info) => throw new NotImplementedException(),
        };
    }
}
