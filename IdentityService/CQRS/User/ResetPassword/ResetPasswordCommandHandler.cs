namespace IdentityService.CQRS.User.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, IResult>
{
    private const string UserCodeDescription = "This user does not exists";
    private const string PasswordCodeDescription = "The password and confirmation password do not match.";

    public async Task<IResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null) 
        {
            return Results.BadRequest(
                new List<IdentityError>
                {
                    new()
                    {
                        Code = "User",
                        Description = UserCodeDescription
                    }
                });
        }

        if (request.Password != request.ConfirmPassword) 
        {
            return Results.BadRequest(new List<IdentityError>
            {
                new()
                {
                    Code = "Password",
                    Description = PasswordCodeDescription
                }
            });
        }
        
        var result = await userManager.ResetPasswordAsync(user, request.Code, request.Password);
        return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
    }
}