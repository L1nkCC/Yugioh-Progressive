using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine;
using System.Collections;
using System.Linq;
using Utilities;

public class LobbyManager : MonoBehaviour, Utilities.Instance.IInstance<LobbyManager>
{
    //Player Details
    [SerializeField] TMPro.TMP_InputField usernameInput;
    [SerializeField] TMPro.TMP_Dropdown deckListInput;

    private string username => usernameInput == null ? "Unnamed Player" : Utilities.String.RemoveZeroWidthSpace(usernameInput.text);
    private ListSave.CardCollectionList deckList => deckListInput == null ?
        ListSave.GetDefaultList(ListSave.ListType.deck) : ListSave.LoadList(deckListInput.options[deckListInput.value].text, ListSave.ListType.deck);

    //Lobby Details
    private const bool SHOW_DECK_NAMES_INT_VAL = true;
    private const string RELAY_JOIN_CODE_INT_VAL = "0";
    public const int MAX_PLAYER_COUNT = 2;

    //Lobby information
    private Lobby hostLobby;
    private Lobby joinedLobby;

    //Timers
    private float heartbeatTimer;
    private const float HEARTBEAT_MAX_TIME = 15f;

    private float updateTimer;
    private const float UPDATE_MAX_TIME = 1.5f;

    //Called when a player joins the joinedLobby
    private System.Action OnPlayerJoin;
    private System.Action OnPlayerLeave;
    private System.Action OnHostAllocatedRelay = new(OnHostAllocatedRelayBase) ;

    //Player Data Dictionary Keys
    private const string USER_NAME = "Username";
    private const string DECK_LIST = "DeckList";

    //Lobby Data Dictionary Keys
    private const string SHOW_DECK_NAMES_KEY = "ShowDeckNames";
    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";

    //relay connection flag
    private bool relayConnectionFlag = false;



    
    private async void Awake()
    {
        if (Instance.IInstance<LobbyManager>.Instance != null)
            Destroy(this.gameObject);
        this.SetInstance();

        await UnityServices.InitializeAsync();
        Authenticate();
        usernameInput.text = "user" + Utilities.Random.NextInt(10000, 99999);
    }
    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyUpdate();
    }
    #region Backend Lobby Support
    private async void Authenticate()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return;
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    #region HeartBeat and Update timers


    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = HEARTBEAT_MAX_TIME;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void HandleLobbyUpdate()
    {
        if (joinedLobby != null)
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer < 0f)
            {
                updateTimer = UPDATE_MAX_TIME;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                Lobby oldLobby = joinedLobby;
                joinedLobby = lobby;
                if (joinedLobby.Players.Count > oldLobby.Players.Count) OnPlayerJoin();
                if (joinedLobby.Players.Count < oldLobby.Players.Count) OnPlayerLeave();
                if (joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value != RELAY_JOIN_CODE_INT_VAL && !relayConnectionFlag) { OnHostAllocatedRelay(); relayConnectionFlag = true; }
            }
        }
    }
    #endregion

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {USER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, username)},
                        {DECK_LIST, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, JsonUtility.ToJson(deckList))}
                    }
        };
    }

    private Unity.Services.Lobbies.Models.Player GetPlayerFromLobby()
    {
        return joinedLobby.Players.Where((Unity.Services.Lobbies.Models.Player player) => { return player.Id == AuthenticationService.Instance.PlayerId; }).ElementAt(0);
    }

    public string GetPlayerUsernameFromLobby(int playerIndex)
    {
        return joinedLobby.Players[playerIndex].Data[USER_NAME].Value;
    }

    public ListSave.CardCollectionList GetPlayerDeckFromLobby()
    {
        return JsonUtility.FromJson<ListSave.CardCollectionList>(GetPlayerFromLobby().Data[DECK_LIST].Value);
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = username + "'s Lobby";
            int maxPlayers = MAX_PLAYER_COUNT;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {SHOW_DECK_NAMES_KEY, new DataObject(DataObject.VisibilityOptions.Public, SHOW_DECK_NAMES_INT_VAL.ToString(), DataObject.IndexOptions.S1)},
                    {RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, RELAY_JOIN_CODE_INT_VAL, DataObject.IndexOptions.S2)},
                }
            };



            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            Debug.Log("Created Lobby : " + lobby.Name + "    ID: " + lobby.Id + "     Code: " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies Found: " + response.Results.Count);
            foreach (Lobby lobby in response.Results)
            {
                Debug.Log(lobby.Name + " playerMax: " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }


    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions { Player = GetPlayer() };
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            Debug.Log("Joined lobby with code " + lobbyCode);
            CreateJoinedLobbyPanel();
            OnHostAllocatedRelay = () => 
            {
                RelayHandler.JoinRelay(joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value, GetPlayerDeckFromLobby());
                Debug.Log("RELAY_JOIN_CODE: " + joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value);
            };
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
            Panel.CreatePanel("Join Failed", "Please enter a valid lobby code.", JoinLobbyButton);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            if(hostLobby == null) await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            if (hostLobby != null && LobbyService.Instance.GetLobbyAsync(hostLobby.Id) != null)
                await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
            Reset();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
    #endregion

    #region Frontend Lobby Support
    private IEnumerator CreateLobbyButtonCoroutine()
    {
        CreateLobby();
        yield return new WaitUntil(() => { return joinedLobby != null; });
        CreateJoinedLobbyPanel();
        OnPlayerJoin = () => { CreateJoinedLobbyPanel(); Debug.Log("Player Joined"); };
        OnPlayerLeave = () => { CreateJoinedLobbyPanel(); Debug.Log("Player Left"); };
    }

    public void CreateJoinedLobbyPanel()
    {
        if (Utilities.Instance.IInstance<Panel>.Instance != null)
            Utilities.Instance.IInstance<Panel>.Instance.Delete();

        string players = "";
        foreach (Unity.Services.Lobbies.Models.Player player in joinedLobby.Players)
        {
            players += "\t" + player.Data[USER_NAME].Value;
            if (bool.Parse(joinedLobby.Data[SHOW_DECK_NAMES_KEY].Value))
                players += ": \t" + JsonUtility.FromJson<ListSave.CardCollectionList>(player.Data[DECK_LIST].Value).name;
            players += "\n";
        }
        System.Action OnConfirm = LeaveLobby;
        if (joinedLobby.Players.Count > 1 && hostLobby != null && hostLobby.Id.Equals(joinedLobby.Id))
        {
            OnConfirm = StartMultiplayerPlay;
        }
        Panel.CreatePanel(joinedLobby.Name, "Lobby Code : " + joinedLobby.LobbyCode + "\nPlayers :\n" + players, OnConfirm, LeaveLobby);
    }

    #region OnButtonPress
    public void CreateLobbyButton()
    {
        if (!CheckInputs())
            return;
        StartCoroutine(CreateLobbyButtonCoroutine());
    }
    public void JoinLobbyButton()
    {
        if (!CheckInputs())
            return;
        if (Utilities.Instance.IInstance<Panel>.Instance != null)
            Utilities.Instance.IInstance<Panel>.Instance.Delete();
        TextInputPanel.CreatePanel("Join Lobby", "Please enter the Lobby Code.", JoinLobbyByCode, new List<string>());
    }
    #endregion
    #endregion


    private bool CheckInputs()
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Panel.CreatePanel("Invalid Username", "Please enter username without only non-whitespace characters", () => { Utilities.Instance.IInstance<Panel>.Instance.Delete(); });
            return false;
        }

        if (!deckList.IsValidDeck())
        {
            Panel.CreatePanel("Invalid Deck", "Please enter deck of appropriate size", () => { Utilities.Instance.IInstance<Panel>.Instance.Delete(); });
            return false;
        }

        return true;
    }

    public async void StartMultiplayerPlay()
    {
        if (hostLobby != null)
        {
            try
            {
                Debug.Log("StartGame");
                string relayJoinCode = await RelayHandler.CreateRelay(GetPlayerDeckFromLobby());

                hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });
                joinedLobby = hostLobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    private static void OnHostAllocatedRelayBase()
    {
        Debug.Log("BASE relay allocation");
    }
    private void Reset()
    {
        joinedLobby = null;
        hostLobby = null;
        OnPlayerJoin = null;
        OnPlayerLeave = null;
        OnHostAllocatedRelay = new(OnHostAllocatedRelayBase);
        relayConnectionFlag = false;
    }

    private void OnDestroy()
    {
        LeaveLobby();
        this.DeleteInstance();
    }
}
