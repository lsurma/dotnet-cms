using Microsoft.AspNetCore.Components;

namespace Surma.Cms.Dev.Blazor.Components.Dynamic;

public interface IComponentInterface
{
    public string Value { get; set; }
    
    public EventCallback<string> ValueChanged { get; set; }
}