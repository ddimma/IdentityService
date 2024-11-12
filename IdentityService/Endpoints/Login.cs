using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Endpoints;

public static class Login
{
    /*public static void AccountEndpoints(this IEndpointRouteBuilder routes)
    {
        var app = routes.MapGroup("/Account");
        app.MapPost("/Login",
            async (LoginRequest request, UserManager<ApplicationUser> userManager,
                IIdentityServerInteractionService interaction, HttpContext httpContext) =>
            {
                if (request is not null)
                {
                    var identityErrorsList = new List<IdentityError>();
                    
                    var user = await userManager.FindByNameAsync(request.UserName);
                    if (user == null)
                    {
                        throw new Exception("Username or password is incorrect");
                    }
                    
                    
                }
                return Results.BadRequest("Invalid model state");
            });
    }*/
}