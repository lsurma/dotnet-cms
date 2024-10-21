namespace Surma.Cms.UI.Blazor.Components.ActionHandling;

public class ActionHandlingPanelContentModel
{
    public Type ComponentType { get; set; }
    
    public ActionPanelParametersBag Parameters { get; set; } = new ActionPanelParametersBag();

    protected Func<Task> StateHasChanged { get; set; }

    public void Bind(ActionHandlingPanel actionHandlingPanel)
    {
        StateHasChanged = actionHandlingPanel.InvokeStateHasChangedAsync;
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