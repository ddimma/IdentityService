namespace IdentityService.Endpoints.User;

public static class RegisterUserEndpointHandler
{
    public static async Task<IResult> RegisterUser(RegisterUserCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result;
    }
}