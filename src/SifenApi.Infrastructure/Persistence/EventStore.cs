using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SifenApi.Domain.Common;
using SifenApi.Domain.Interfaces;
using SifenApi.Infrastructure.Persistence.Context;

namespace SifenApi.Infrastructure.Persistence;

public class EventStore : IEventStore
{
    private readonly SifenDbContext _context;

    public EventStore(SifenDbContext context)
    {
        _context = context;
    }

    public async Task SaveEventAsync<T>(T @event) where T : DomainEvent
    {
        var eventData = new StoredEvent
        {
            AggregateId = @event.EventId,
            EventType = @event.GetType().FullName ?? @event.GetType().Name,
            Data = JsonConvert.SerializeObject(@event),
            OccurredOn = @event.OccurredOn
        };

        _context.Set<StoredEvent>().Add(eventData);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        var events = await _context.Set<StoredEvent>()
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.OccurredOn)
            .ToListAsync();

        return events.Select(e => 
            JsonConvert.DeserializeObject<DomainEvent>(e.Data) ?? 
            throw new InvalidOperationException($"Could not deserialize event {e.Id}"))
            .ToList();
    }

    public async Task<IEnumerable<T>> GetEventsAsync<T>(Guid aggregateId) where T : DomainEvent
    {
        var eventTypeName = typeof(T).FullName;
        
        var events = await _context.Set<StoredEvent>()
            .Where(e => e.AggregateId == aggregateId && e.EventType == eventTypeName)
            .OrderBy(e => e.OccurredOn)
            .ToListAsync();

        return events.Select(e => 
            JsonConvert.DeserializeObject<T>(e.Data) ?? 
            throw new InvalidOperationException($"Could not deserialize event {e.Id}"))
            .ToList();
    }
}
