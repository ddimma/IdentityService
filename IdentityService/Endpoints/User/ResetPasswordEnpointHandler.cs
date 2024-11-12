using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Endpoints.User
{
    public class ResetPasswordEnpointHandler
    {
        public static async Task<IResult> ResetPassword(ResetPassword model, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken) 
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(model.Email);
            if(user is null) 
            {
                return Results.BadRequest(
                    new List<IdentityError>{new IdentityError
                {
                    Code = "User",
                    Description = "This user does not exists"
                }});
            }

            if(model.Password != model.ConfirmPassword) 
            {
                return Results.BadRequest(new List<IdentityError> {
                    new IdentityError
                {
                    Code = "Password",
                    Description = "The password and confirmation password do not match."
                }});
            }

            IdentityResult result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            
            return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
        }
    }
}