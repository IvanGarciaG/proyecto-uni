using Shared.Kernel;
using Users.Domain.Events;

namespace Users.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string email, string fullName, string passwordHash)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            FullName = fullName,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user.Email, user.FullName));
        return user;
    }

    public void Deactivate()
    {
        IsActive = false;
        RaiseDomainEvent(new UserDeactivatedDomainEvent(Id));
    }
}
