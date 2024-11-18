namespace IdentityService.Endpoints.User;

public static class ResetPasswordEndpointHandler
{
    public static async Task<IResult> ResetPassword(ResetPasswordCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken) as IResult;
        return result;
    }
}