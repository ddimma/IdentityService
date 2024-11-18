namespace IdentityService.Endpoints;

public static class EndpointMapper
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder routes) 
    {
        var app = routes.MapGroup("/Account");

        app.MapPost("/Register",
            async (RegisterUserCommand model, IMediator mediator, CancellationToken cancellationToken) =>
                await RegisterUserEndpointHandler.RegisterUser(model, mediator, cancellationToken));
            
        app.MapPost("/ResetPassword",
            async (ResetPasswordCommand model, IMediator mediator,
                    CancellationToken cancellationToken) =>
                await ResetPasswordEndpointHandler.ResetPassword(model, mediator, cancellationToken));

        app.MapPost("/RequestResetPassword",
            async (RequestResetPasswordCommand model, IMediator mediator,
                    CancellationToken cancellationToken) =>
                await RequestResetPasswordEndpointHandler.RequestResetPassword(model, mediator,
                    cancellationToken));
    }

    public static void MapApplicationEndpoints(this IEndpointRouteBuilder routes)
    {
        var app = routes.MapGroup("/Seed");

        app.MapGet("",
            async (IConfiguration configuration) =>
                await SeedEndpointHandler.Seed(configuration));
    }
}