using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Services; // For IAuthService, IEmpleadoService
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IEmpleadoRepository _repository;
    private readonly IEmpleadoService _empleadoService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(
        IEmpleadoRepository repository, 
        IEmpleadoService empleadoService, 
        IEmailService emailService, 
        IConfiguration configuration,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _repository = repository;
        _empleadoService = empleadoService;
        _emailService = emailService;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResponse?> LoginAsync(AuthRequest request)
    {
        // 1. Find Identity User by Email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        // 2. Validate Password (Documento)
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Documento, false);
        if (!result.Succeeded) return null;

        // 3. Find Domain Employee for Claims
        var empleado = await _repository.GetByDocumentoAsync(request.Documento);
        if (empleado == null) return null; // Should not happen if sync is correct

        // 4. Generate JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "SecretKeyTooShortNeedsToBeLonger12345");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, empleado.Id.ToString()),
                new Claim(ClaimTypes.Email, empleado.Email),
                new Claim(ClaimTypes.Name, $"{empleado.Nombres} {empleado.Apellidos}")
            }),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthResponse { Token = tokenHandler.WriteToken(token), Email = empleado.Email };
    }

    public async Task<EmpleadoDto> RegisterAsync(CreateEmpleadoDto request)
    {
        // 1. Validation: Check if email already exists in Identity
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null) throw new Exception("El correo ya está registrado.");

        // 2. Create Empleado (Domain)
        var created = await _empleadoService.CreateAsync(request);

        // 3. Create Identity User (Auth)
        var newUser = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(newUser, request.Documento); // Password = Documento

        if (!result.Succeeded)
        {
            // Rollback (Delete created employee) - In a real app use Transaction
            // Since we don't have transaction here easily across contexts (if different), we just delete.
            // But they share context ideally.
            try { await _empleadoService.DeleteAsync(created.Id); } catch {}
            throw new Exception($"Error creando usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // 4. Send welcome email (Fire and forget safe)
        try
        {
            await _emailService.SendEmailAsync(
                created.Email, 
                "Bienvenido a TalentoPlus", 
                $"Hola {created.Nombres}, su registro fue exitoso. Ya puede autenticarse en la plataforma cuando esté habilitado."
            );
        }
        catch (Exception ex)
        {
            // Log but don't fail the registration
            Console.WriteLine($"Warning: Could not send welcome email to {created.Email}. Error: {ex.Message}");
        }
            
        return created;
    }
}
