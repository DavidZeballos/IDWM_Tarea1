using courses_dotnet_api.Src.DTOs.Account;
using courses_dotnet_api.Src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courses_dotnet_api.Src.Controllers;

public class AccountController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;

    private readonly ITokenService _tokenService;

    public AccountController(IUserRepository userRepository, IAccountRepository accountRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        // Verificar si el usuario existe y la contraseña es correcta
        var user = await _accountRepository.ValidateUserAsync(loginDto.Email, loginDto.Password);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        
        // Generar un token para el usuario
        var token = _tokenService.CreateToken(user.Rut);

        // Crear un AccountDto para devolver al cliente
        var accountDto = new AccountDto
        {
            Rut = user.Rut,
            Name = user.Name,
            Email = user.Email,
            Token = token
        };

        return Ok(accountDto);
    }

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterDto registerDto)
    {
        if (
            await _userRepository.UserExistsByEmailAsync(registerDto.Email)
            || await _userRepository.UserExistsByRutAsync(registerDto.Rut)
        )
        {
            return TypedResults.BadRequest("User already exists");
        }

        await _accountRepository.AddAccountAsync(registerDto);

        if (!await _accountRepository.SaveChangesAsync())
        {
            return TypedResults.BadRequest("Failed to save user");
        }

        AccountDto? accountDto = await _accountRepository.GetAccountAsync(registerDto.Email);

        return TypedResults.Ok(accountDto);
    }
}
