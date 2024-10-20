namespace Surma.Cms.UI.Blazor.Components;

public class ActionContext
{
    public string ElementDisplayName { get; set; } = "";
    
    public bool InProgress { get; set; } = false;
    
    public Dictionary<string, Func<Task>> Actions { get; set; } = new();
    
    protected Func<Task> StateHasChanged { get; set; }

    public void Bind(ActionPanel actionPanel)
    {
        StateHasChanged = actionPanel.InvokeStateHasChangedAsync;
    }
    
    public void Change(Action<ActionContext> setter)
    {
        setter(this);
        StateHasChanged.Invoke();
    }
}