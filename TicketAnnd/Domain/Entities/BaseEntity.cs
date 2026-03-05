using System.ComponentModel.DataAnnotations;
using TicketAnnd.Domain.Events;

namespace TicketAnnd.Domain.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}