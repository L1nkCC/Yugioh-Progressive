using UnityEngine;
using UnityEngine.EventSystems;
using DS_Environment;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    #region Variables

    public CardParticles particles;

    //info
    public CardInfo Info { get; set; }

    //components
    private UnityEngine.UI.Image _image;

    //true private
    private static Canvas _canvas => FindObjectOfType<Canvas>();// must have only one canvas;//do not edit
    private bool _isFollowingMouse;
    private CanvasGroup _canvasGroup;
    private float _timeOfLastClick;
    private RectTransform _rect;

    //Constants
    private const float DOUBLE_CLICK_TIME = .28f;
    private const float GRABBED_SIZE_MULTIPLIER = 1.5f;
    private const float GRABBED_ALPHA_MULTIPLIER = .5f;

    //Resources
    public Sprite CardFace { get; set; }
    public static Sprite CardBack => Resources.Load<Sprite>(ResourcePath.cardBack);
    public static GameObject cardPrefab => Resources.Load<GameObject>(ResourcePath.card);

    //public status
    public bool Faceup { get; private set; } = true;
    public bool InAttack { get; private set; } = true;
    public Zone Zone;

    //Actions
    public System.Action Grab;
    public System.Action Drop;
    public System.Action<Card> DoubleClick;
    public System.Action Inspect;


    //
    public bool ZoneClientDuelistMatch => SceneManagement.inSoloPlayScene() ? true : Zone.duelist == PlaySceneGameObjects.Client.GetComponent<Player>().duelist;

    //Buttons
    static readonly PointerEventData.InputButton _selectButton = PointerEventData.InputButton.Left;
    static readonly PointerEventData.InputButton _inspectButton = PointerEventData.InputButton.Right;

    #endregion

    private void Awake()
    {
        particles = new CardParticles(this);

        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<UnityEngine.UI.Image>();
        _rect = GetComponent<RectTransform>();

        Grab += _grabbed;
        Drop += _dropped;
        Inspect += _inspected;
        DoubleClick += _doubleClicked;

        particles.AttachToButtons();


    }

    public static Card Instantiate(CardInfo info, Zone zone, int index = 0)
    {
        GameObject cardObject = Instantiate(cardPrefab, PlaySceneGameObjects.CardHolder.transform);
        Card card = cardObject.GetComponent<Card>();
        zone.moveCard(card, index);
        card.Load(info);
        return card;
    }

    public Card Copy()
    {
        return Instantiate(Info, Zone, GetIndexInZone());
    }


    public void Load(int cardId)
    {
        Info = API_InfoHandler.GetCardInfo(cardId);
        LoadTexture(cardId);
    }
    public void Load(CardInfo _info)
    {
        Info = _info;
        Load(Info.id);
    }

    public void RefreshTexture()
    {
        if (Faceup)
        {
            _image.sprite = CardFace;
        }
        else
        {
            _image.sprite = CardBack;
        }
    }
    public void LoadTexture(int cardId)
    {
        API_TextureHandler textureHandler = new();
        textureHandler.SetTexture(this);
    }


    private void Update()
    {
        if (_isFollowingMouse)
        {
            Vector2 movePos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Input.mousePosition, _canvas.worldCamera,
                out movePos);

            transform.position = _canvas.transform.TransformPoint(movePos);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        BTN_Handler.BTN_Menu_Handler.BTN_Menu_Close();
        if (eventData.button == _selectButton)
        {
            if (Time.time - _timeOfLastClick < DOUBLE_CLICK_TIME)
            {
                DoubleClick(this);
            }
            _timeOfLastClick = Time.time;
        }
        if (eventData.button == _inspectButton)
        {
            Inspect();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == _selectButton && ZoneClientDuelistMatch) Grab();
    }
    public void OnDrop(PointerEventData eventData)
    {
        Zone.OnDrop(eventData);
    }

    private void _grabbed()
    {
        _isFollowingMouse = true;
        _canvasGroup.blocksRaycasts = false;
        toFrontOfScreen();

        transform.SetParent(PlaySceneGameObjects.CardHolder.transform);
        _image.sprite = CardFace;

        _rect.transform.localScale *= GRABBED_SIZE_MULTIPLIER;
        _canvasGroup.alpha *= GRABBED_ALPHA_MULTIPLIER;
    }
    private void _dropped()
    {
        _isFollowingMouse = false;
        _canvasGroup.blocksRaycasts = true;

        if (!Faceup)
            _image.sprite = CardBack;

        _rect.transform.localScale /= GRABBED_SIZE_MULTIPLIER;
        _canvasGroup.alpha /= GRABBED_ALPHA_MULTIPLIER;

    }
    private void _inspected()
    {
        //open BTN menu
        if(SceneManagement.inPlayScene() && ZoneClientDuelistMatch)
            BTN_Handler.BTN_Menu_Handler.BTN_Menu_Open(this);

        //update inspect screen
        if (SceneManagement.inMultiPlayScene())
            BTN_Handler.BTN_Actions[(int)BTN_Option.inspect](this);
        else
            BTN_Handler.BTN_Click(this, BTN_Option.inspect);
    }

    public void View()
    {
        _image.enabled = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _image.enabled = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public static void PlayParticles(Card card)
    {
        card.GetComponent<ParticleSystem>().Play();
    }

    private int GetIndexInZone()
    {
        return Zone.cards.IndexOf(this);
    }

    public void toFrontOfScreen()
    {
        _rect.SetAsLastSibling();
    }

    private void _doubleClicked(Card card)
    {
        if(ZoneClientDuelistMatch)
            Zone.DoubleClicked(card);
    }


    public void flip()
    {
        Faceup = !Faceup;
        if (Faceup)
            _image.sprite = CardFace;
        else
            _image.sprite = CardBack;

    }

    public void RotateForDuelistChange()
    {
        if(Zone.duelist == DuelistDesignation.Opponent)
            transform.DORotate(new Vector3(0, 0, 180), 1, RotateMode.Fast);
        else
            transform.DORotate(new Vector3(0, 0, 0), 1, RotateMode.Fast);

    }

    private void toDefense()
    {
        transform.DORotate(new Vector3(0, 0, 90), 1);
        InAttack = false;
    }
    private void toAttack()
    {
        transform.DORotate(new Vector3(0, 0, 0), 1);
        InAttack = true;
    }

    public void changeSize(Vector2 size)
    {
        _rect.sizeDelta = size;
    }

    public void changeBattlePosition()
    {
        if (InAttack)
        {
            toDefense();
        }
        else
        {
            toAttack();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }


}
