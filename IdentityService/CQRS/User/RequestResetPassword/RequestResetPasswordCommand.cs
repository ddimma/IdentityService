namespace IdentityService.CQRS.User.RequestResetPassword;

public class RequestResetPasswordCommand : IRequest<IResult>
{
    public required string Email { get; set; }
}