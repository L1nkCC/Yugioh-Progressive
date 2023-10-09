using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class PHA_Handler
{
    [System.Serializable]
    public enum PHA_Option
    {
        unassigned = 0,
        draw,
        standby,
        main1,
        battle,
        main2,
        end,
    };
    private static System.Action[] PHA_Actions = new System.Action[]
    {
        new System.Action(_unassigned),
        new System.Action(_draw),
        new System.Action(_standby),
        new System.Action(_main1),
        new System.Action(_battle),
        new System.Action(_main2),
        new System.Action(_end),
    };

    public const PHA_Option DEFAULT_OPTION = PHA_Option.main1;
    public static PHA_Option PHA_Current;
    public static bool InBattlePhase => (PHA_Current == PHA_Option.battle);

    public static void PHA_Click(PHA_Option option)
    {
        if (DS_Environment.NetworkStatus.isConnectedToPlayer())
        {
            Player.connection.PHA_ChangeServerRPC(option);
        }
        else
        {
            PHA_Select(option);
        }
    }
    public static void PHA_Select(PHA_Option option) 
    {
        PHA_Actions[(int)option]();
    }
    public static void PHA_Assign(PHA_Option option, System.Action action)
    {
        PHA_Actions[(int)option]+= action;
    }

    private static void _draw()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.draw).Select();
    }
    private static void _standby()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.standby).Select();
    }
    private static void _main1()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.main1).Select();
    }
    private static void _battle()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.battle).Select();
    }
    private static void _main2()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.main2).Select();
    }
    private static void _end()
    {
        Utilities.Instance.GetInstanceOf<PHA_Button, PHA_Option>(PHA_Option.end).Select();
    }
    private static void _unassigned()
    {

    }

}