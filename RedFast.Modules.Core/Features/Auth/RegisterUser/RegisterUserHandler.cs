using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities;
using RedFast.Modules.Core.Persistence;
using BC = BCrypt.Net.BCrypt;

namespace RedFast.Modules.Core.Features.Auth.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly RedFastDbContext _context;

    public RegisterUserHandler(RedFastDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if(emailExists)
            throw new Exception("Email já cadastrado.");

        var passwordHash = BC.HashPassword(request.Password);

        var user = new User(request.Email, passwordHash);

        _context.Users.Add(user);

        var sender = new Sender
        {
            Id = Guid.NewGuid(),
            Document = request.Document,
            UserId = user.Id,
            CompanyName = request.Name,
        };

        _context.Senders.Add(sender);

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
