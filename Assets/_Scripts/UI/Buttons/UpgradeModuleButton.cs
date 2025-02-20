public class UpgradeModuleButton : ButtonClickHandler
{
    private ServiceView serviceView;
    private Service service;
    public Module Module { get; private set; }
    public void Init(ServiceView serviceView, Module module, Service service)
    {
        this.serviceView = serviceView;
        this.Module = module;
        this.service = service;
    }
    public override void OnButtonClicked()
    {
        Module.Upgrade();
        serviceView.Display(service);
    }
}
