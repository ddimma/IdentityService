namespace IdentityService.Models;

public class RouteSettings
{
    [Required] public required string AccountApiGroup { get; set; }
    [Required] public required string RegisterEndPoint { get; set; }
    [Required] public required string ResetPasswordEndpoint { get; set; }
    [Required] public required string RequestResetPasswordEndpoint { get; set; }
}