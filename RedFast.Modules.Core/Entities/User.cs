namespace RedFast.Modules.Core.Entities;

public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public string Role { get; private set; } = "sender";

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    protected User() { }

    public User(string email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
    }
}
