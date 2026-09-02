using PhotoEditor.DTOs;
using PhotoEditor.Models;

namespace PhotoEditor.Data;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> users = [];

    public User? GetUser(LoginDto login)
    {
        if (!users.TryGetValue(login.Username, out var user))
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(
            login.Password,
            user.Password
        ))
        {
            return null;
        }

        return user;
    }

    public bool HasUser(string Username)
    {
        return users.ContainsKey(Username);
    }

    public bool StoreUser(RegisterDto register)
    {
        if (HasUser(register.Username))
        {
            return false;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

        var user = new User
        {
            Id = users.Count + 1,
            Username = register.Username,
            Password = passwordHash
        };

        users.Add(user.Username, user);

        return true;
    }
}