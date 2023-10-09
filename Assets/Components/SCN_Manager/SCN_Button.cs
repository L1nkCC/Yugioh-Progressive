using DS_Environment;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class SCN_Button : MonoBehaviour
{
    [SerializeField] SceneManagement.BuildIndex SceneDestination;
    private void Awake()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SCN_Manager.LoadScene(SceneDestination));
    }
}
