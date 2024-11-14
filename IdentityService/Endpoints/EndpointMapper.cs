using IdentityService.Endpoints.User;
using IdentityService.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace IdentityService.Endpoints
{
    public static class EndpointMapper
    {
        public static void MapAccountEndpoints(this IEndpointRouteBuilder routes) 
        {
            var app = routes.MapGroup("/Account");
            app.MapPost("/ResetPassword",async (ResetPassword model, UserManager<ApplicationUser> userManager, CancellationToken cancelationToken) => await ResetPasswordEndpointHandler.ResetPassword(model,userManager, cancelationToken));
        }
    }
}