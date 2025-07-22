using SifenApi.Domain.Common;

namespace SifenApi.Domain.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync<T>(T @event) where T : DomainEvent;
    Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId);
    Task<IEnumerable<T>> GetEventsAsync<T>(Guid aggregateId) where T : DomainEvent;
}