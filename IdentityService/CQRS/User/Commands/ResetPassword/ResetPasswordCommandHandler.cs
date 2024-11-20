namespace IdentityService.CQRS.User.Commands.ResetPassword;

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
                        Code = UserDescriptions.UserCode,
                        Description = UserDescriptions.UserCodeDescription
                    }
                });
        }

        if (request.Password != request.ConfirmPassword) 
        {
            return Results.BadRequest(new List<IdentityError>
            {
                new()
                {
                    Code = PasswordDescriptions.PasswordCode,
                    Description = PasswordDescriptions.PasswordCodeDescription
                }
            });
        }
        
        var result = await userManager.ResetPasswordAsync(user, request.Code, request.Password);
        return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
    }
}