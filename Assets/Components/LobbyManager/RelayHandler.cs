using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using UnityEngine;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;

//Reliant that LobbyManager set up Authenication
public static class RelayHandler
{
    public static async Task<string> CreateRelay(ListSave.CardCollectionList deckList)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.MAX_PLAYER_COUNT);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            return joinCode;
            
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
            return "Relay Allocation Failed";
        }
    }
    public static async void JoinRelay(string _joinCode, ListSave.CardCollectionList deckList)
    {
        try
        {
            Debug.Log("Joining Relay with code : " + _joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

}
