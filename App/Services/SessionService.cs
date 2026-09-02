using PhotoEditor.DTOs;
using PhotoEditor.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PhotoEditor.Services;

public class SessionService(IHttpContextAccessor context, IUserRepository repo) : IAuthService
{
    public const string SessionUserId = "_UserId";
    public async Task<(AuthResult, string?)> LoginAsync(LoginDto login)
    {
        if(string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
        {
            return (AuthResult.EMPTY_FIELD, null);
        }
        var user = repo.GetUser(login);
        if (user is null)
        {
            return (AuthResult.DENIED, null);
        }

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await  context.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return (AuthResult.SUCCESS, user.Username);
    }

    public RegisterResult Register(RegisterDto register)
    {
        if(repo.HasUser(register.Username))
        {
            return RegisterResult.DUBLICATE_USER;
        }

        PasswordStats stats = new(register.Password);
        if(!stats.IsAcceptable())
        {
            return RegisterResult.BAD_PASSWORD_FORMAT;
        }

        bool IsAccepted = repo.StoreUser(register);
        if(!IsAccepted)
        {
            return RegisterResult.REGISTER_FAILED_INTERNAL;
        }
        return RegisterResult.SUCCESS;
    }

    class PasswordStats
    {
        const string allowedSymbols = "!@#$%";
        readonly long NumberCount, SmallLetterCount, BigLetterCount, SymbolsCount, BadSymbols, Length;
        const long NumberCountMin = 0;
        const long SmallLetterCountMin = 0;
        const long BigLetterCountMin = 0;
        const long SymbolsCountMin = 0;
        const long LengthMin = 0;
        public PasswordStats(string password)
        {
            Length = password.Length;
            foreach(char c in password)
            {
                if (char.IsDigit(c))
                {
                    NumberCount++;
                }
                else if (char.IsLower(c))
                {
                    SmallLetterCount++;
                }
                else if (char.IsUpper(c))
                {
                    BigLetterCount++;
                }
                else if (allowedSymbols.Contains(c))
                {
                    SymbolsCount++;
                }
                else
                {
                    BadSymbols++;
                }
            }
        }
        public bool IsAcceptable()
        {
            return
                BadSymbols == 0 &&
                Length >= LengthMin &&
                NumberCount >= NumberCountMin &&
                SmallLetterCount >= SmallLetterCountMin &&
                BigLetterCount >= BigLetterCountMin &&
                SymbolsCount >= SymbolsCountMin;
        }
        public bool ContainsBadSymbols()
        {
            return BadSymbols > 0;
        }
    }
}