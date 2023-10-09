using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
using UnityEngine.SceneManagement;
public class Persistant : MonoBehaviour
{
    [SerializeField] bool DestroyInstead = false;
    [SerializeField] List<SceneManagement.BuildIndex> SaveThrough = new();
    [SerializeField] UnityEngine.Events.UnityAction<Scene, Scene> OnActiveSceneChange;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        OnActiveSceneChange = ChangedActiveScene;
        SceneManager.activeSceneChanged += OnActiveSceneChange;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        if (DestroyInstead && SaveThrough.Contains((SceneManagement.BuildIndex)next.buildIndex))
            Destroy(this.gameObject);
        if (!DestroyInstead && !SaveThrough.Contains((SceneManagement.BuildIndex)next.buildIndex))
            Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChange;
    }

}
