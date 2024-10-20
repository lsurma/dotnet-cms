using Microsoft.Extensions.Primitives;

namespace Surma.Cms.Core.DynamicPages;

public class DynamicContentChangeToken : IChangeToken
{
    public bool ActiveChangeCallbacks { get; private init; }

    public bool HasChanged { get; private set; }

    public bool CanChange { get; init; }

    public string? Name { get; }

    protected List<RegisteredCallback> Callbacks { get; } = new List<RegisteredCallback>();
    
    protected DynamicContentChangeToken(string name)
    {
        Name = name;
    }
    
    public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
    {
        if (CanChange)
        {
            Callbacks.Add(new RegisteredCallback(callback, state));
        }
        
        return EmptyDisposable.Instance;
    }
    
    internal void TriggerChange()
    {
        HasChanged = true;
        foreach (var callback in Callbacks)
        {
            callback.Callback(callback.State);
        }
        HasChanged = false;
    }


    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        
        private EmptyDisposable() { }
        
        public void Dispose() { }
    }

    protected class RegisteredCallback(Action<object?> callback, object? state)
    {
        public Action<object?> Callback { get; } = callback;
        public object? State { get; } = state;
    }
    
    public static DynamicContentChangeToken WithActiveChangeCallback(string name)
    {
        return new DynamicContentChangeToken(name)
        {
            HasChanged = false,
            CanChange = true,
            ActiveChangeCallbacks = true
        };
    }

    public static DynamicContentChangeToken NeverChanging(string name)
    {
        return new DynamicContentChangeToken(name)
        {
            HasChanged = false,
            CanChange = false
        };
    }
}