using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using Utilities;
public class API_TextureHandler
{
    private static readonly string ROOT_PATH = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "images";
    private static readonly string FILE_EXTENSION = ".jpg";

    public void SetTexture(Card card)
    {
        card.StartCoroutine(SetCardFaceTexture(card));
    }

    public static void SetTexture(UnityEngine.UI.Image image, CardInfo info, CardImageOption imageOption, int alternateArtIndex = 0)
    {
        image.StartCoroutine(SetImageTexture(image, info, imageOption, alternateArtIndex));
    }


    public enum CardImageOption
    {
        standard = 0,
        small,
        cropped
    }
    #region Get Methods -- Path,Directory,URL
    private static string GetPath(CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        return (GetDirectory(info,imageOption,alternateArtIndex) + Path.AltDirectorySeparatorChar + (int)imageOption + FILE_EXTENSION);
    }
    private static string GetDirectory(CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        return (ROOT_PATH + Path.AltDirectorySeparatorChar + info.id + Path.AltDirectorySeparatorChar + alternateArtIndex);
    }

    private static string GetURL(CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        switch (imageOption)
        {
            case CardImageOption.standard:
                return info.card_images[alternateArtIndex].image_url;
            case CardImageOption.small:
                return info.card_images[alternateArtIndex].image_url_small;
            case CardImageOption.cropped:
                return info.card_images[alternateArtIndex].image_url_cropped;
            default:
                Debug.Log("GetURL Error");
                return info.card_images[0].image_url;
        }
    }
    #endregion
    #region Texture File Methods
    private static IEnumerator DownloadCardTextureToFile(CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0, bool refresh = false)
    {
        if(System.IO.File.Exists(GetPath(info, imageOption, alternateArtIndex)) && !refresh)
        {
            yield break;
        }
        Debug.Log("Requesting Card image from YGOprodeck API");
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(GetURL(info, imageOption, alternateArtIndex));
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("recieveImageData ERROR");
            Debug.Log(webRequest.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            IO.AssureDirectory(GetDirectory(info, imageOption, alternateArtIndex));
            System.IO.File.WriteAllBytes(GetPath(info, imageOption, alternateArtIndex), texture.EncodeToJPG());
        }
    }
    private static void SetCardFaceTextureFromFile(Card card, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        if (!System.IO.File.Exists(GetPath(card.Info, imageOption, alternateArtIndex)))
        {
            return;
        }
        Texture2D tmpTex = new(1, 2);
        byte[] rawData = System.IO.File.ReadAllBytes(GetPath(card.Info, imageOption, alternateArtIndex));
        
        if (!tmpTex.LoadImage(rawData))
            Debug.Log("GetCardTextureFile() Failed");

        card.CardFace = SpriteFromTexture2D(tmpTex);
        card.RefreshTexture();
    }
    private static IEnumerator SetCardFaceTexture(Card card, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        yield return DownloadCardTextureToFile(card.Info, imageOption, alternateArtIndex);
        SetCardFaceTextureFromFile(card, imageOption, alternateArtIndex);
    }
    private static void SetImageTextureFromFile(UnityEngine.UI.Image image, CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        if (!System.IO.File.Exists(GetPath(info, imageOption, alternateArtIndex)))
        {
            return;
        }
        Texture2D tmpTex = new(1, 2);
        byte[] rawData = System.IO.File.ReadAllBytes(GetPath(info, imageOption, alternateArtIndex));

        if (!tmpTex.LoadImage(rawData))
            Debug.Log("GetCardTextureFile() Failed");

        image.sprite = SpriteFromTexture2D(tmpTex);
    }
    private static IEnumerator SetImageTexture(UnityEngine.UI.Image image, CardInfo info, CardImageOption imageOption = CardImageOption.standard, int alternateArtIndex = 0)
    {
        yield return DownloadCardTextureToFile(info, imageOption, alternateArtIndex);
        SetImageTextureFromFile(image, info, imageOption, alternateArtIndex);
        Debug.Log(image.name +" set as " + info.id + " Texture Set");
    }

    #endregion
    #region helpers

    private static Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
    #endregion
}
