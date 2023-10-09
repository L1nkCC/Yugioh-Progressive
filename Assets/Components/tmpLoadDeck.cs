using UnityEngine;
using System.IO;
using static API_TextureHandler;
using static API_InfoHandler;
using DS_Environment;
using static DS_Environment.ZoneType;

public class tmpLoadDeck : MonoBehaviour
{
    private void Start()
    {
        LoadDictionaryJson();
    }
}
