using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DS_Environment;
using System.Threading.Tasks;
using static DS_Environment.ZoneType;

public class Player : NetworkBehaviour
{
    public DuelistDesignation duelist {get; private set; }
    public ListSave.CardCollectionList deckList => PlaySceneGameObjects.Lobby.GetComponent<LobbyManager>().GetPlayerDeckFromLobby();
    [SerializeField] private GameObject connectionPrefab;
    public static Connection connection;


    private void Start()
    {
        if (SceneManagement.inSoloPlayScene())
        {
            PlaySceneGameObjects.Board.GetComponent<Board>().LoadDeck(ListSave.GetDefaultList(ListSave.ListType.deck), DuelistDesignation.Player);
        }
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ulong id = OwnerClientId;
        duelist = id > 0 ? DuelistDesignation.Opponent : DuelistDesignation.Player;
        if (PlaySceneGameObjects.Lobby != null)
            name = PlaySceneGameObjects.Lobby.GetComponent<LobbyManager>().GetPlayerUsernameFromLobby((int)id);
        NetworkSetupConnection();
    }

    public void NetworkSetupConnection()
    {
        //does not check that we are online. relies on a player with duelistDesignation opponent only appears in multiplayer scenes
        if (duelist == DuelistDesignation.Opponent && IsOwner)
        {
            IntializeConnectionServerRPC();
            StartCoroutine(CallOnSecureConnection(LoadMultiplayerSceneServerRPC));
            StartCoroutine(CallOnMultiplayerSceneLoaded(() => StartCoroutine(FlipDesignations())));
            StartCoroutine(CallOnMultiplayerSceneLoaded(RequestDecksServerRPC));
            StartCoroutine(CallOnSetDecks(LoadDecksServerRPC));
            
        }
    }

    private IEnumerator FlipDesignations()
    {
        yield return new WaitUntil(() => { return PlaySceneGameObjects.Board != null && PlaySceneGameObjects.LifePoints != null; });
        PlaySceneGameObjects.Board.GetComponent<Board>().FlipZoneDesignations();
        PlaySceneGameObjects.LifePoints.GetComponent<LP_Manager>().FlipDesignation();
    }



    private IEnumerator CallOnSecureConnection(System.Action action)
    {
        yield return new WaitUntil(() => { return Player.connection != null; });
        action();
    }

    private IEnumerator CallOnMultiplayerSceneLoaded(System.Action action)
    {
        yield return new WaitUntil(() => { return Player.connection != null; });
        yield return new WaitUntil(() => { return connection.PlayerLoadedInMultiplayerScene.Value && connection.OpponentLoadedInMultiplayerScene.Value; });
        action();
    }

    private IEnumerator CallOnSetDecks(System.Action action)
    {
        yield return new WaitUntil(() => { return Player.connection != null; });
        yield return new WaitUntil(() => { return connection.PlayerLoadedInMultiplayerScene.Value && connection.OpponentLoadedInMultiplayerScene.Value; });
        yield return new WaitUntil(() => { return (Player.connection.PlayerDeck.Value.listType == ListSave.ListType.deck && Player.connection.OpponentDeck.Value.listType == ListSave.ListType.deck); });
        action();
    }

    #region Load into Multiplayer scene
    [ServerRpc(RequireOwnership = false)]
    public void LoadMultiplayerSceneServerRPC()
    {
        LoadMultiplayerSceneClientRPC();
    }

    [ClientRpc]
    private void LoadMultiplayerSceneClientRPC()
    {
        SCN_Manager.LoadScene(SceneManagement.BuildIndex.multiPlay);
        SetLoadedInMultiplayerSceneServerRPC(new ServerRpcParams());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLoadedInMultiplayerSceneServerRPC(ServerRpcParams param)
    {
        if ((DuelistDesignation)param.Receive.SenderClientId == DuelistDesignation.Player)
            connection.PlayerLoadedInMultiplayerScene.Value = true;
        if ((DuelistDesignation)param.Receive.SenderClientId == DuelistDesignation.Opponent)
            connection.OpponentLoadedInMultiplayerScene.Value = true;
            
    }
    #endregion
    [ServerRpc(RequireOwnership = false)]
    private void IntializeConnectionServerRPC()
    {
        connection = Instantiate(connectionPrefab).GetComponent<Connection>();
        connection.GetComponent<NetworkObject>().Spawn(true);
        SetConnectionClientRPC(new NetworkObjectReference(connection.GetComponent<NetworkObject>()));
    }
    [ClientRpc]
    private void SetConnectionClientRPC(NetworkObjectReference connectionObjectRef)
    {
        connectionObjectRef.TryGet(out NetworkObject connectionObject);
        connection = connectionObject.GetComponent<Connection>();
    } 

    [ServerRpc(RequireOwnership = false)]
    private void RequestDecksServerRPC()
    {
        SetDecksClientRPC();
    }
    [ServerRpc(RequireOwnership = false)]
    public void LoadDecksServerRPC()
    {
        if ((NetworkManager.ConnectedClientsList.Count >= 2))
        {
            connection.LoadDecksClientRPC();
        }
    }


    [ClientRpc]
    private void SetDecksClientRPC()
    {
        connection.SetDeckServerRPC(deckList, NetworkManager.LocalClient.PlayerObject.GetComponent<Player>().duelist, new ServerRpcParams());
    }

}
