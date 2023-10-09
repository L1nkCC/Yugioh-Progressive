using System;
using System.Collections;
using UnityEngine;
using DS_Environment;
using UnityEngine.SceneManagement;
public class SCN_Manager : MonoBehaviour
{
    [SerializeField] private static System.Collections.Generic.Stack<SceneManagement.BuildIndex> previousScenes = new System.Collections.Generic.Stack<SceneManagement.BuildIndex>();
    private IEnumerator LoadScene(int sceneId)
    {
        SceneManager.LoadScene((int)SceneManagement.BuildIndex.loading);
        yield return new WaitForSeconds(.1f); //required so AsyncOperation.allowSceneActivation works as expected. Something odd with Loading scenes too close together
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneId);
        op.allowSceneActivation = false;
        yield return new WaitUntil(() =>
        {
            LoadingBar bar = LoadingSceneGameObjects.LoadingBar.GetComponent<LoadingBar>();
            bar.SetSceneLoadingValue(op.progress);
            return LoadSceneAsyncCompletion(op);
        });
        op.allowSceneActivation = true;
        
        
    }

    private bool LoadSceneAsyncCompletion(AsyncOperation loadSceneOperation)
    {
        return (loadSceneOperation.progress >= .899f);
    }

    public static void LoadScene(SceneManagement.BuildIndex build)
    {
        switch (build)
        {
            case (SceneManagement.BuildIndex.quit):
                PullUpQuitConfirmationPanel();
                break;
            case (SceneManagement.BuildIndex.boot):
                throw new Exception("LoadScene was passed boot scene index");
            case (SceneManagement.BuildIndex.loading):
                throw new Exception("LoadScene was passed loading scene index");
            case (SceneManagement.BuildIndex.previous):
                LoadPreviousScene();
                break;
            default:
                SceneManagement.BuildIndex currentSceneBuildIndex = (SceneManagement.BuildIndex)SceneManager.GetActiveScene().buildIndex;
                if (currentSceneBuildIndex != SceneManagement.BuildIndex.boot && currentSceneBuildIndex != SceneManagement.BuildIndex.loading)
                    previousScenes.Push(currentSceneBuildIndex);
                PersistantScripts.SCN_Manager.StartCoroutine(PersistantScripts.SCN_Manager.LoadScene((int)build));
                break;
        }
    }
    private static void LoadPreviousScene()
    {
        if(previousScenes.TryPop(out SceneManagement.BuildIndex previousScene))
        {
            PersistantScripts.SCN_Manager.StartCoroutine(PersistantScripts.SCN_Manager.LoadScene((int)previousScene));
        }
        else
        {
            Debug.LogWarning("There was no previous Scene");
        }
    }
    static void PullUpQuitConfirmationPanel()
    {
        Panel.CreatePanel("Quit Game", "Are you sure you want to quit the game?", Application.Quit);
    }

}
