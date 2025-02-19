public class ReturnButton : ButtonClickHandler
{
    public async override void OnButtonClicked()
    {
        var confirmed = await CanvasManager.Instance.ShowConfirmationPopop(false, true, "Return to level selection?");
        if (confirmed)
        {
            //StartCoroutine(LevelManager.Instance.ForceEndLevel());
        }
    }
}
