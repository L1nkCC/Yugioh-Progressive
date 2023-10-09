public class INS_Image : UnityEngine.UI.Image
{
    public void SetImage(Card card, API_TextureHandler.CardImageOption option = API_TextureHandler.CardImageOption.standard)
    {
        API_TextureHandler.SetTexture(this, card.Info, option);
    }

}
