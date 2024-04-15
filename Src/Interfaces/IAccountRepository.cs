using courses_dotnet_api.Src.DTOs.Account;
using courses_dotnet_api.Src.Models;

namespace courses_dotnet_api.Src.Interfaces;

public interface IAccountRepository
{
    Task<bool> SaveChangesAsync();
    Task AddAccountAsync(RegisterDto registerDto);
    Task<AccountDto?> GetAccountAsync(string email);
    Task<User?> ValidateUserAsync(string email, string password);
}
