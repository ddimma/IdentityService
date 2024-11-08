using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Endpoints
{
    public static class Auth
    {
        public static void AccountEndpoints(this IEndpointRouteBuilder routes)
        {
            var app = routes.MapGroup("/Account");

            app.MapPost("/Register", async (RegisterUserRequest request, UserManager<ApplicationUser> userManager, IIdentityServerInteractionService interaction, HttpContext httpContext) =>
            {
                // Validate request
                if (request is not null)
                {
                    var identityErrorsList = new List<IdentityError>();
                    var user = new ApplicationUser 
                    { 
                        UserName = request.UserName,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    };
                    //var existingUserByEmail = await userManager.FindByEmailAsync(user.Email);
                    //var existngUserByUsername = await userManager.FindByNameAsync(user.UserName);
                    //if (existingUserByEmail is not null)
                    //{
                    //    identityErrorsList.Add(new IdentityError { Code = "Email", Description = "Email is taken."});
                    //}

                    var result = await userManager.CreateAsync(user, request.Password);


                    if (result.Succeeded)
                    {
                        // Issue authentication cookie
                        var isUser = new IdentityServerUser(user.Id)
                        {
                            DisplayName = user.UserName,
                        };

                        return Results.Ok();
                    }
                    else
                    {
                        return Results.BadRequest(result.Errors);
                    }
                }

                return Results.BadRequest("Invalid model state");
            });
        }
    }
}
