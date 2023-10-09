using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class UI_Button : UnityEngine.UI.Button
{
    static SFX_Manager sfx_manager;
    [SerializeField] float HoverSizeMultiplier = 1.3f;
    [SerializeField] float HoverAnimationTime = .3f;

    protected override void Awake()
    {
        base.Awake();

        if (sfx_manager == null)
            sfx_manager = DS_Environment.PersistantScripts.SFX_Manager;
        onClick.AddListener(PlayClickSound);
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        (this.transform as RectTransform).DOScale(Vector3.one * HoverSizeMultiplier, HoverAnimationTime).SetEase(Ease.InOutSine);
        PlayHoverSound();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        this.DOKill();
        (this.transform as RectTransform).DOScale(Vector3.one, HoverAnimationTime);
    }
    public void PlayClickSound()
    {
        sfx_manager.PlayUI_BTN_CLICK();
    }
    private void PlayHoverSound()
    {
        sfx_manager.PlayUI_BTN_HOVER();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.DOKill();
    }
}
