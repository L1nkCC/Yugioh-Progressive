using System;
using System.Collections.Generic;

[System.Serializable]
public struct RequestInfo
{
    public CardInfo[] data;
}


[System.Serializable]
public struct CardInfo
{
    public int id;
    public string name;
    public string type;
    public string frameType;
    public string desc;
    public uint atk;
    public uint def;
    public uint level;
    public string race;
    public string attribute;
    public string archetype;
    public int scale;
    public int linkval;
    public string[] linkmarkers;
    public CardSetInfo[] card_sets;
    public CardBanInfo banlist_info;
    public CardImageURLs[] card_images;
    public CardPriceInfo[] card_prices;
    public CardMiscInfo[] misc_info;
}


#region CardInfo Supporting structs:  Set,Image,Price
[System.Serializable]
public struct CardSetInfo 
{
    public string set_name;
    public string set_code;
    public string set_rarity;
    public string set_rarity_code;
    public string set_price;
}

[System.Serializable]
public struct CardImageURLs 
{
    public string image_url;
    public string image_url_small;
    public string image_url_cropped;
}
[System.Serializable]
public struct CardPriceInfo
{
    public string cardmarket_price;
    public string tcgplayer_price;
    public string ebay_price;
    public string amazon_price;
    public string coolstuffinc_price;
}

[System.Serializable]
public struct CardMiscInfo
{
    public string beta_name;
    public int views;
    public int viewsweek;
    public int upvotes;
    public int downvotes;
    public string[] formats;
    public string treated_as;
    public string tcg_date;
    public string ocg_date;
    public int konami_id;
    public int has_effect;
}

[System.Serializable]
public struct CardBanInfo
{
    public string ban_tcg;
    public string ban_ocg;
    public string ban_goat;
}
#endregion
