namespace Surma.Cms.Core;

public interface ILocalEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : ICmsEvent;
}