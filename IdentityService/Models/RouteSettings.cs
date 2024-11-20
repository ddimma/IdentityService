namespace IdentityService.Models;

public class RouteSettings
{
    [Required] public required string AccountApiGroup { get; set; }
    [Required] public required string RegisterEndPoint { get; set; }
    [Required] public required string ResetPasswordEndPoint { get; set; }
    [Required] public required string RequestResetPasswordEndPoint { get; set; }
}