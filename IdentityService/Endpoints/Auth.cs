using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Authentication;
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
            
            app.MapPost("/Login", async (
                LoginRequest request,
                UserManager<ApplicationUser> userManager,
                IIdentityServerInteractionService interaction,
                HttpContext httpContext) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");
 
                var user = await userManager.FindByNameAsync(request.UserName);
                if (user == null)
                    return Results.Unauthorized();
 
                var isValid = await userManager.CheckPasswordAsync(user, request.Password);
                if (!isValid)
                    return Results.Unauthorized();
 
                var roles = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
 
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    IdentityServerConstants.DefaultCookieAuthenticationScheme);
 
                var principal = new ClaimsPrincipal(claimsIdentity);
                await httpContext.SignInAsync(
                    IdentityServerConstants.DefaultCookieAuthenticationScheme,
                    principal);
 
                return Results.Ok(new
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            });
        }
    }
}
