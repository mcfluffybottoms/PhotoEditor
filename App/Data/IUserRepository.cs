using PhotoEditor.Models;
using PhotoEditor.DTOs;

namespace PhotoEditor.Data;

public interface IUserRepository
{
    public User? GetUser(LoginDto user);
    public bool StoreUser(RegisterDto user);
    public bool HasUser(string Username);
} 