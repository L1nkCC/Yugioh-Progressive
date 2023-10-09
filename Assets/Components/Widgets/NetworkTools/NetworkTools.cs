using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class NetworkTools : MonoBehaviour
{
    private void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        //Server
        buttons[0].onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        //Host
        buttons[1].onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        //Client
        buttons[2].onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
