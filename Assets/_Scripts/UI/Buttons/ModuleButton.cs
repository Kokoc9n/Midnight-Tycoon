
public class ModuleButton : ButtonClickHandler
{
    private ServiceView serviceView;
    public Module Module { get; private set; }
    public void Init(ServiceView serviceView, Module module)
    {
        this.serviceView = serviceView;
        this.Module = module;
    }
    public override void OnButtonClicked()
    {
        serviceView.SelectModule(Module);
    }
}
