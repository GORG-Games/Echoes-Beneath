using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(FadeThenLoad(sceneName));
    }

    private IEnumerator FadeThenLoad(string sceneName)
    {
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeIn();
        Debug.Log("LOADING!");
        SceneManager.LoadScene(sceneName);              // Мгновенная загрузка
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(FadeThenLoadIndex(sceneIndex));
    }
    private IEnumerator FadeThenLoadIndex(int sceneIndex)
    {
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeIn();
        Debug.Log("LOADING!");
        SceneManager.LoadScene(sceneIndex);              // Мгновенная загрузка
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartCoroutine(FadeThenLoadIndex(currentScene.buildIndex));
        }
        else
        {
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneAsyncWithFade(nextIndex));
        }
        else
        {
            Debug.LogWarning("Next scene index out of range!");
        }
    }
    private IEnumerator LoadSceneAsyncWithFade(string sceneName)
    {
        /*Debug.Log("Fade Out");
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeOut();

        Debug.Log("SceneManager.LoadSceneAsync");
        yield return SceneManager.LoadSceneAsync(sceneName);

        Debug.Log("Fade In");
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeIn();

        Debug.Log("DONE!");*/

        Debug.Log("[FadeController] Fade In");
        yield return FadeController.Instance.FadeIn();

        Debug.Log("[FadeController] Loading scene...");
        yield return SceneManager.LoadSceneAsync(sceneName);

        Debug.Log("[FadeController] Fade Out");
        yield return FadeController.Instance.FadeOut();

        Debug.Log("[FadeController] DONE");
    }

    private IEnumerator LoadSceneAsyncWithFade(int sceneIndex)
    {
        /*Debug.Log("Fade Out");
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeOut();

        Debug.Log("SceneManager.LoadSceneAsync");
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
        {
            Debug.Log("... loading: " + asyncOp.progress);
            yield return null;
        }

        Debug.Log("Fade In");
        if (FadeController.Instance != null)
            yield return FadeController.Instance.FadeIn();
        
        Debug.Log("DONE!");*/

        Debug.Log("[FadeController] Fade In");
        yield return FadeController.Instance.FadeIn();

        Debug.Log("[FadeController] Loading scene...");
        yield return SceneManager.LoadSceneAsync(sceneIndex);

        Debug.Log("[FadeController] Fade Out");
        yield return FadeController.Instance.FadeOut();

        Debug.Log("[FadeController] DONE");
    }
    public void LoadSceneByIndexUnpaused(int index)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
