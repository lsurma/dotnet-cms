namespace Surma.Cms.UI.Blazor.Components.ActionHandling;

public class ActionHandlingContext
{
    public string ElementDisplayName { get; set; } = "";
    
    public ActionsStack ActionsStack { get; set; } = new();
    
    public bool AnyActionInProgress => ActionsStack.Any(a => a.InProgress);
    
    public Dictionary<string, Func<Task>> Actions { get; set; } = new();
    
    protected Func<Task> StateHasChanged { get; set; }

    public void Bind(ActionHandlingPanel actionHandlingPanel)
    {
        StateHasChanged = actionHandlingPanel.InvokeStateHasChangedAsync;
    }
    
    public void Change(Action<ActionHandlingContext> setter)
    {
        setter(this);
        StateHasChanged.Invoke();
    }

}

public class ActionsStack : List<ActionState>
{
    
}

public class ActionState
{
    public string Name { get; set; } = "";
    
    public bool InProgress { get; set; } = false;
}