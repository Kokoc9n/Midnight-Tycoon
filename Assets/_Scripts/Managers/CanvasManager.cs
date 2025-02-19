using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private static protected Page[] pagesToLoad;
    private static protected Stack<Page> activePages = new();

    public static CanvasManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        pagesToLoad = FindObjectsOfType<Page>();
    }
    private void Start()
    {
        foreach (Page p in pagesToLoad) p.gameObject.SetActive(false);

        // Starting Page
        StackPage(typeof(GameUIPage));
    }

    public static void StackPage(Type pageType) // Make awaitable.
    {
        Page page = pagesToLoad.FirstOrDefault(p => p.GetType() == pageType);
        activePages.Push(page);

        page.OnAppear();
    }
    public static void PopPage()
    {
        activePages.Peek().OnDisappear();
        activePages.Pop();
    }
    public static void SwitchPage(Type pageType)
    {
        if (activePages.First().GetType() == pageType) return;
        activePages.First().OnDisappear();
        activePages.Pop();

        Page page = pagesToLoad.FirstOrDefault(p => p.GetType() == pageType);
        page.OnAppear();
        activePages.Push(page);
    }
    #region Popups
    public async Task<TPayload> ShowConfirmationPopop<TPayload>(TPayload payload1, TPayload payload2, string description = null)
    {
        var tcs = new TaskCompletionSource<TPayload>();

        var popup = GameObject.Instantiate(Resources.Load("Prefabs/ConfirmationPopup"), Instance.transform) as GameObject;
        var levelFailedView = popup.GetComponent<ConfirmationPopupView>();

        levelFailedView.confirmButton.onClick.RemoveAllListeners();
        levelFailedView.closeButton.onClick.RemoveAllListeners();

        levelFailedView.confirmButton.onClick.AddListener(() => OnButtonPressed(tcs, payload1, popup.transform));
        levelFailedView.closeButton.onClick.AddListener(() => OnButtonPressed(tcs, payload2, popup.transform));

        levelFailedView.description.text = description;
        return await tcs.Task;

        void OnButtonPressed(TaskCompletionSource<TPayload> tcs, TPayload payload, Transform popup)
        {
            tcs.TrySetResult(payload);
            Destroy(popup.gameObject);
        }
    }
    #endregion
}