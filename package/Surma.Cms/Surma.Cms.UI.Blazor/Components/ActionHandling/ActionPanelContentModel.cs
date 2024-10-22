namespace Surma.Cms.UI.Blazor.Components.ActionHandling;

public class ActionPanelContentModel
{
    public Type ComponentType { get; set; }
    
    public ActionPanelParametersBag Parameters { get; set; } = new ActionPanelParametersBag();

    protected Func<Task>? StateHasChanged { get; set; }

    public void Register(ActionPanel actionPanel)
    {
        StateHasChanged = actionPanel.InvokeStateHasChangedAsync;
    }
    
    public void Unregister(ActionPanel actionPanel)
    {
        StateHasChanged = null;
    }
    
    public void Change(Func<ActionPanelParametersBag, ActionPanelParametersBag> paramsSetter)
    {
        var newParams = paramsSetter(Parameters);
        Parameters = newParams;
        StateHasChanged.Invoke();
    }
}

public class ActionPanelParametersBag : Dictionary<string, object>
{
}