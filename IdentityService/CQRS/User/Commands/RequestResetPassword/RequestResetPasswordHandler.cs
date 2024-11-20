namespace IdentityService.CQRS.User.Commands.RequestResetPassword;

public class RequestResetPasswordHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<RequestResetPasswordCommand, IResult>
{
    public async Task<IResult> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var applicationUser = await userManager.FindByEmailAsync(request.Email);
        if (applicationUser is null)
        {
            return Results.BadRequest(
                new List<IdentityError>
                {
                    new()
                    {
                        Code = CodeDescriptions.UserCode,
                        Description = CodeDescriptions.UserCodeDescription
                    }
                });
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(applicationUser);

        return Results.Ok(new
        {
            isSuccess = true,
            result = new { hash = token }
        });
    }
}