using System.Security.Cryptography;
using System.Text;
using KnotBond.Controllers;
using KnotBond.Data;
using KnotBond.DTOs;
using KnotBond.Entities;
using KnotBond.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnotBond;

public class AccountController : BaseController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExist(registerDTO.Username)) return BadRequest("Username has already taken.");
        using var hmac = new HMACSHA256();
        var user = new AppUser
        {
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Username);
        if (user == null) return Unauthorized("invalid Username or password");
        using var hmac = new HMACSHA256(user.PasswordSalt);
        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < ComputeHash.Length; i++)
            if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid username or Password");
        return new UserDTO
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
        };
    }
    private async Task<bool> UserExist(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
