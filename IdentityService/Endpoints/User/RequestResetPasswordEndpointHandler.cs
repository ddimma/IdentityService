namespace IdentityService.Endpoints.User;

public static class RequestResetPasswordEndpointHandler
{
    public static async Task<IResult> RequestResetPassword(RequestResetPasswordCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result;
    }
}