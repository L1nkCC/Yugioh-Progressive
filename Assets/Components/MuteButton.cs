using DS_Environment;
using UnityEngine;

[RequireComponent(typeof(UI_Button))]
public class MuteButton : MonoBehaviour
{
    [SerializeField] Sprite UnmutedSprite;
    [SerializeField] Sprite MutedSprite;
    UI_Button button;
    bool muted = false;

    void Awake()
    {
        button = GetComponent<UI_Button>();
        SetMuteStatus(PersistantScripts.SFX_Manager.muted);
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SetMuteStatus(!muted);
    }
    public void SetMuteStatus(bool mute)
    {
        muted = mute;
        button.image.sprite = mute ? MutedSprite : UnmutedSprite;
        PersistantScripts.SFX_Manager.muted = mute;
    }
}
