namespace IdentityService.CQRS.User.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, IResult>
{
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
                        Description = CodeDescriptions.UserCodeDescription
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
                    Description = CodeDescriptions.PasswordCodeDescription
                }
            });
        }
        
        var result = await userManager.ResetPasswordAsync(user, request.Code, request.Password);
        return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
    }
}