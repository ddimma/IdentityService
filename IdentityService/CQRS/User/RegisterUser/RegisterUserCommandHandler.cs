namespace IdentityService.CQRS.User.RegisterUser;

public class RegisterUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<RegisterUserCommand, IResult>
{
    public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser 
        { 
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
    }
}