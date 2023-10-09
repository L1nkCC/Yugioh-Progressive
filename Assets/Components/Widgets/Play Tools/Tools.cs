using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGO_Commands;
public class Tools : MonoBehaviour
{
    static SFX_Manager sfx_manager;

    private void Awake()
    {
        sfx_manager = DS_Environment.PersistantScripts.SFX_Manager;
    }

    public void FlipCoin()
    {
        int side = Random.Range(0, 2);
        string sideName = "Heads";
        if (side == 0) sideName = "Tails";
        CMD_Manager.AddMessage("Coin was flipped to " + sideName);
        sfx_manager.PlayFlipCoinSFX();
    }

    public void RollDie()
    {
        int side = Random.Range(0, 6);
        CMD_Manager.AddMessage("Die was rolled to " + side);
        sfx_manager.PlayRollDieSFX();
    }
    
}
