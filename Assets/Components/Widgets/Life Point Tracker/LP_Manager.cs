using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DS_Environment;
using Utilities;
using static Utilities.Instance;
public class LP_Manager : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField inputField;
    [SerializeField] LP_Slider playerLifeSlider;
    [SerializeField] LP_Slider opponentLifeSlider;
    [SerializeField] Button AddLPButton;
    [SerializeField] Button SubLPButton;
    static SFX_Manager sfx_manager;

    private void Awake()
    {
        sfx_manager = DS_Environment.PersistantScripts.SFX_Manager;
        AddLPButton.onClick.AddListener(_add);
        SubLPButton.onClick.AddListener(_subtract);
    }
    public uint InputValidation()
    {
        try
        {
            return uint.Parse(inputField.text);
        }
        catch
        {
            Debug.Log("Parse Failed");
            return 0;
        }
    }

    public void FlipDesignation()
    {
        LP_Slider tmp = playerLifeSlider;
        playerLifeSlider = opponentLifeSlider;
        opponentLifeSlider = tmp;
    }
    private void _add()
    {
        int change = (int)InputValidation();
        if (NetworkStatus.isConnectedToPlayer())
        {
            Player.connection.LP_ChangeServerRPC(change, (DuelistDesignation)Unity.Netcode.NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            playerLifeSlider.Add(InputValidation());
            sfx_manager.PlayGainLifeSFX();
        }
    }
    private void _subtract()
    {
        int change = (int)InputValidation();
        if (NetworkStatus.isConnectedToPlayer())
        {
            Player.connection.LP_ChangeServerRPC(-change, (DuelistDesignation)Unity.Netcode.NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            playerLifeSlider.Subtract(InputValidation());
            sfx_manager.PlayLoseLifeSFX();
        }
    }

    public void ChangeLife(int lifeChange, DuelistDesignation duelist)
    {
        LP_Slider target = duelist == DuelistDesignation.Player ? playerLifeSlider : opponentLifeSlider;

        if(lifeChange > 0)
        {
            target.Add((uint)lifeChange);
            sfx_manager.PlayGainLifeSFX();
        }
        else if(lifeChange < 0)
        {
            target.Subtract((uint)(-lifeChange));
            sfx_manager.PlayLoseLifeSFX();
        }
    }

}
