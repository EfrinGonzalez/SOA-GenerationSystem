namespace SOA.DomainEvents
{
    /// <summary>
    /// Base class for all domain events.
    /// </summary>
    public abstract record UserDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Raised when a user is registered through public/self-registration.
    /// </summary>
    public record UserSelfRegistered(Guid UserId, string Email) : UserDomainEvent;

    /// <summary>
    /// Raised when an admin creates a user.
    /// </summary>
    public record UserCreatedByAdmin(Guid UserId, string Email) : UserDomainEvent;

    /// <summary>
    /// Raised when a user is updated by an admin.
    /// </summary>
    public record UserUpdated(Guid UserId, string Email, bool IsActive) : UserDomainEvent;

    /// <summary>
    /// Raised when a user is deleted by an admin.
    /// </summary>
    public record UserDeleted(Guid UserId) : UserDomainEvent;

}
