namespace IdentityService.CQRS.User.Commands.RequestResetPassword;

public class RequestResetPasswordCommand : IRequest<IResult>
{
    public required string Email { get; set; }
}