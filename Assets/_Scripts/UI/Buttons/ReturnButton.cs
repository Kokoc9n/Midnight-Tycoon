public class ReturnButton : ButtonClickHandler
{
    public async override void OnButtonClicked()
    {
        CanvasManager.PopPage();
    }
}
