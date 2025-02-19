public class SettingsButton : ButtonClickHandler
{
    public override void OnButtonClicked()
    {
        CanvasManager.StackPage(typeof(OptionsPage));
    }
}
