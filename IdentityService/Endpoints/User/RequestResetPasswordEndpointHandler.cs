using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Endpoints.User
{
    public class RequestResetPasswordEndpointHandler
    {
        public static async Task<IResult> RequestResetPassword(RequestResetPassword model, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return Results.BadRequest(
                    new List<IdentityError>{new IdentityError
                    {
                        Code = "User",
                        Description = "This user does not exists"
                    }
                });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            return Results.Ok(new
            {
                isSuccess = true,
                result = new { hash = token }
            });
        }
    }
}
