namespace IdentityService.Models;

public class IdentitySettings
{
    public bool RequireUniqueEmail { get; set; }
    public bool RequireNonAlphanumericPassword { get; set; }
    public int RequiredLengthPassword { get; set; }
}