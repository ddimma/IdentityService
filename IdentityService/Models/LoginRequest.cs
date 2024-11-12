using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models;

public class LoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}