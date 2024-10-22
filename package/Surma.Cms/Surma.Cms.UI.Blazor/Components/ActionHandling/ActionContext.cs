namespace Surma.Cms.UI.Blazor.Components.ActionHandling;

public class ActionContext
{
    public string ElementDisplayName { get; set; } = "";
    
    public ActionsStack ActionsStack { get; set; } = new();
    
    public bool AnyActionInProgress => ActionsStack.Any(a => a.InProgress);
    
    public bool ActionInProgress(string actionName) => ActionsStack.Any(a => a.Name == actionName && a.InProgress);

    public Dictionary<string, Func<Task>> Actions { get; set; } = new();
    
    protected Func<Task>? StateHasChanged { get; set; }

    public void Register(ActionPanel actionPanel)
    {
        StateHasChanged = actionPanel.InvokeStateHasChangedAsync;
    }
    
    public void Unregister(ActionPanel actionPanel)
    {
        StateHasChanged = null;
    }
    
    public void Change(Action<ActionContext> setter)
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