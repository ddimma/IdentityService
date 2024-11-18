using IdentityService.Models;

namespace IdentityService.Endpoints;

public static class EndpointMapper
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder routes, IConfiguration configuration) 
    {
        var routeSettingsValue = configuration.GetSection("RouteSettings").Get<RouteSettings>();
        if (routeSettingsValue == null) throw new ArgumentNullException(nameof(routeSettingsValue));
        
        var app = routes.MapGroup(routeSettingsValue.AccountApiGroup);

        app.MapPost(routeSettingsValue.RegisterEndPoint,
            async (RegisterUserCommand model, IMediator mediator, CancellationToken cancellationToken) =>
                await RegisterUserEndpointHandler.RegisterUser(model, mediator, cancellationToken));
            
        app.MapPost(routeSettingsValue.ResetPasswordEndpoint,
            async (ResetPasswordCommand model, IMediator mediator,
                    CancellationToken cancellationToken) =>
                await ResetPasswordEndpointHandler.ResetPassword(model, mediator, cancellationToken));

        app.MapPost(routeSettingsValue.RequestResetPasswordEndpoint,
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