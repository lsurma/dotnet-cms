namespace Surma.Cms.UI.Blazor.Components.ActionHandling;

public class ActionHandlingPanelContentModel
{
    public Type ComponentType { get; set; }
    
    public ActionHandlingPanelParametersBag Parameters { get; set; } = new ActionHandlingPanelParametersBag();

    protected Func<Task>? StateHasChanged { get; set; }

    public void Register(ActionHandlingPanel actionHandlingPanel)
    {
        StateHasChanged = actionHandlingPanel.InvokeStateHasChangedAsync;
    }
    
    public void Unregister(ActionHandlingPanel actionHandlingPanel)
    {
        StateHasChanged = null;
    }
    
    public void Change(Func<ActionHandlingPanelParametersBag, ActionHandlingPanelParametersBag> paramsSetter)
    {
        var newParams = paramsSetter(Parameters);
        Parameters = newParams;
        StateHasChanged.Invoke();
    }
}

public class ActionHandlingPanelParametersBag : Dictionary<string, object>
{
}