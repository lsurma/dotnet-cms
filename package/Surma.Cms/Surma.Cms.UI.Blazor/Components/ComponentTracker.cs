namespace Surma.Cms.UI.Blazor.Components;

public class ComponentNode
{
    public string ComponentName { get; set; }
    public List<ComponentNode> Children { get; set; } = new();
}

public class ComponentTracker
{
    public ComponentNode Root { get; private set; } = new() { ComponentName = "Root" };

    private Stack<ComponentNode> _componentStack = new();

    public ComponentTracker()
    {
        _componentStack.Push(Root);  // Start with the root node
    }

    public void RegisterComponent(string componentName)
    {
        var currentParent = _componentStack.Peek();  // Get the current parent component
        var newNode = new ComponentNode { ComponentName = componentName };
        currentParent.Children.Add(newNode);  // Add the new component as a child

        _componentStack.Push(newNode);  // Set the new component as the current parent
    }

    public void UnregisterComponent()
    {
        _componentStack.Pop();  // Remove the current component when it's done rendering
    }
}