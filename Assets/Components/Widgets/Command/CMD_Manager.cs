using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static Utilities.Instance;
using DS_Environment;
using static DS_Environment.ZoneType;

namespace YGO_Commands
{
    public class CMD_Manager : MonoBehaviour, IInstance<CMD_Manager>
    {
        //the have no zone case will use this as standard
        private static ZON_Option DEFAULT_FROM_ZONE => ZON_Option.mainDeckZone;
        public const string CMD_SIGNIFIER = "/";
        private static bool isCommand(string input) => input.Trim().StartsWith(CMD_SIGNIFIER);
        private static Board board;

        private static readonly System.Action<uint, ZON_Option>[] CMD_Actions = new System.Action<uint, ZON_Option>[]
        {
            new System.Action<uint, ZON_Option> (_unassigned),
            new System.Action<uint, ZON_Option> (_mill),
            new System.Action<uint, ZON_Option> (_draw),
            new System.Action<uint, ZON_Option> (_toBanish),
            new System.Action<uint, ZON_Option> (_toBanishFacedown),
            new System.Action<uint, ZON_Option> (_reveal),
        };

        [SerializeField] private TMPro.TMP_InputField inputField;
        [SerializeField] private ScrollRect outputField;

        private void Awake()
        {
            this.SetInstance();

            board = PlaySceneGameObjects.Board.GetComponent<Board>();

            inputField.onSubmit.AddListener(CMD_Call);
            inputField.onSubmit.AddListener(AddMessage);
            inputField.onSubmit.AddListener((string input) => inputField.text = null);
        }

        public static void AddMessage(string message)
        {
            if (NetworkStatus.isConnectedToPlayer())
            {
                Player.connection.CMD_ChatMessageServerRPC(message);
            }
            else
            {
                PostMessage(message);
            }
        }
        public static void PostMessage(string message)
        {
            IInstance<CMD_Manager>.Instance.outputField.GetComponentInChildren<TMPro.TextMeshProUGUI>().text += message + "\n";
            Canvas.ForceUpdateCanvases();
            IInstance<CMD_Manager>.Instance.outputField.verticalNormalizedPosition = 0f;
        }

        public void CMD_Call(string input)
        {
            (CMD_Option option, uint num, ZON_Option zone) = CMD_Parse(input);
            CMD_Press(option)(num, zone);
        }

        public System.Action<uint, ZON_Option> CMD_Press(CMD_Option option)
        {
            return CMD_Actions[(int)option];
        }

        #region CMD_Parsing Methods
        //the name that calls the string in the input
        private static string CMD_Name(CMD_Option option)
        {
            return option switch
            {
                CMD_Option.mill => "mill",
                CMD_Option.draw => "draw",
                CMD_Option.toBanish => "banish",
                CMD_Option.toBanishFacedown => "banishFD",
                CMD_Option.reveal => "reveal",
                _ => "Not a Command",
            };
        }
        private static (CMD_Option, uint, ZON_Option) CMD_Parse(string input)
        {
            (CMD_Option option, uint num, ZON_Option zone) operation = (CMD_Option.unassigned, 1, DEFAULT_FROM_ZONE);

            if (input == null)
                return operation;

            if (!isCommand(input))
                return operation;

            string cmd = input.Trim();
            cmd = cmd.Substring(1);//removefirstelement the cmd denoter
            string[] cmdParameters = cmd.Split(' ');

            if (cmdParameters.Length > 3)
            {
                Debug.LogError("Too many Parameters for command");
                return operation;
            }

            foreach (CMD_Option possibleOption in System.Enum.GetValues(typeof(CMD_Option)))
            {
                if (CMD_Name(possibleOption).Equals(cmdParameters[0]))
                {
                    Debug.Log(possibleOption);
                    operation.option = possibleOption;
                    break;
                }
            }
            if (cmdParameters.Length == 1)
            {
                return operation;
            }

            //determine s_num
            if (uint.TryParse(cmdParameters[1], out operation.num))
            {
                if (cmdParameters.Length == 3)
                {
                    foreach (ZON_Option possibleZone in System.Enum.GetValues(typeof(ZON_Option)))
                    {
                        if (ZON_Name(possibleZone).Equals(cmdParameters[2]))
                        {
                            operation.zone = possibleZone;
                        }
                    }
                }
            }
            else
            {
                if (cmdParameters.Length == 2)
                {
                    foreach (ZON_Option possibleZone in System.Enum.GetValues(typeof(ZON_Option)))
                    {
                        if (ZON_Name(possibleZone).Equals(cmdParameters[1]))
                        {
                            operation.zone = possibleZone;
                        }
                    }
                }
            }
            return operation;
        }
        #endregion
        #region CMD press methods
        private static void _mill(uint numberOfCards, ZON_Option fromZone)
        {
            Debug.Log("mill : " + numberOfCards + " : " + fromZone);
            numberOfCards = board.GetZone(fromZone).Count < numberOfCards ? (uint)board.GetZone(fromZone).Count : numberOfCards;

            for (uint i = 0; i < numberOfCards; i++)
            {
                BTN_Handler.BTN_Click(board.GetZone(fromZone).TopCard, BTN_Option.mill);
            }
        }
        private static void _draw(uint numberOfCards, ZON_Option fromZone)
        {
            numberOfCards = board.GetZone(fromZone).Count < numberOfCards ? (uint)board.GetZone(fromZone).Count : numberOfCards;
            for (uint i = 0; i < numberOfCards; i++)
            {
                BTN_Handler.BTN_Click(board.GetZone(fromZone).TopCard, BTN_Option.draw);
            }
        }
        private static void _toBanish(uint numberOfCards, ZON_Option fromZone)
        {
            numberOfCards = board.GetZone(fromZone).Count < numberOfCards ? (uint)board.GetZone(fromZone).Count : numberOfCards;
            for (uint i = 0; i < numberOfCards; i++)
            {
                BTN_Handler.BTN_Click(board.GetZone(fromZone).TopCard, BTN_Option.toBanish);
            }
        }
        private static void _toBanishFacedown(uint numberOfCards, ZON_Option fromZone)
        {
            numberOfCards = board.GetZone(fromZone).Count < numberOfCards ? (uint)board.GetZone(fromZone).Count : numberOfCards;
            for (uint i = 0; i < numberOfCards; i++)
            {
                BTN_Handler.BTN_Click(board.GetZone(fromZone).TopCard, BTN_Option.toBanishFacedown);
            }
        }
        private static void _reveal(uint position, ZON_Option fromZone)
        {

        }
        private static void _unassigned(uint numberOfCards, ZON_Option fromZone)
        {

        }
        #endregion


    }
}
