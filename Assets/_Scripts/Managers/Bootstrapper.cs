using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async void ExecuteLate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
#if !UNITY_EDITOR
        await LoadSceneAsync(0);
        await LoadSceneAsync(1);
#endif
        //await LoadSceneAsync(0);
        //await LoadSceneAsync(1);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async void Execute()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
    }
    static async Task LoadSceneAsync(int sceneToLoad)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;
        await Task.Yield();
        while (!asyncOperation.isDone)
        {
            float progress = asyncOperation.progress;
            if (ProgressBar.Instance == null) break;
            if (progress < 0.9f)
            {
                await ProgressBar.Instance.Image
                        .FillAmountAsync(progress, 0.5f);
            }
            else
            {
                await ProgressBar.Instance.Image
                        .FillAmountAsync(1f, 2f);
                break;
            }
        }
        asyncOperation.allowSceneActivation = true;
    }
#if !UNITY_EDITOR
    [InitializeOnLoadAttribute]
    public static class DefaultSceneLoader
    {
        static DefaultSceneLoader()
        {
            EditorApplication.playModeStateChanged += LoadDefaultScene;
        }

        static async void LoadDefaultScene(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                await LoadSceneAsync(0);
                await LoadSceneAsync(1);
            }
        }

    }
#endif
}
