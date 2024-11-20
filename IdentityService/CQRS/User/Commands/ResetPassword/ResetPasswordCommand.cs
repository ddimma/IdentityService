namespace IdentityService.CQRS.User.Commands.ResetPassword;

public class ResetPasswordCommand: IRequest<IResult>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string? ConfirmPassword { get; set; }
    public required string Code { get; set; }
}