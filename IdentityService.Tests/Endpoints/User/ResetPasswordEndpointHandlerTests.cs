using FluentAssertions;
using IdentityService.Endpoints.User;
using IdentityService.Models;
using IdentityService.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace IdentityService.Tests.Endpoints.User
{
    public class ResetPasswordEndpointHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> mockUserManager;
        public ResetPasswordEndpointHandlerTests() 
        {
            mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
        }
        [Fact]
        public async Task ResetPassword_WhenUserUserWithThisEmailDoesNotExists_ReturnsBadRequestWithIdentityError() 
        {
            // Arrange
            ResetPassword model = new()
            {
                Email = "email@mail.com",
                Code = "some token",
                Password = "Passworddd",
                ConfirmPassword = "passwordddd",
            };
            mockUserManager.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return (null); });

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "User",
                    Description = "This user does not exists"
                }});
            var expectedValues = ResultHelper.GetValuesFromObject(expectedResult);

            // Act
            IResult result = await  ResetPasswordEnpointHandler.ResetPassword(model, mockUserManager.Object, new CancellationToken());

            // Assert
            var actualValues = ResultHelper.GetValuesFromObject(result);
            actualValues.Should().BeEquivalentTo(expectedValues);
        }

        [Fact]
        public async Task ResetPassword_WhenPasswordAndConfirmedPasswordDoNotMatch_ReturnsBadRequestWithIdentityError()
        {
            // Arrange
            ResetPassword model = new()
            {
                Email = "email@mail.com",
                Code = "some token",
                Password = "Not a password",
                ConfirmPassword = "passwordddd",
            };
            mockUserManager.Setup(c => c.FindByEmailAsync(model.Email)).Returns( async () => { return new ApplicationUser { }; });

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "Password",
                    Description = "The password and confirmation password do not match."
                } });
            var expectedValues = ResultHelper.GetValuesFromObject(expectedResult);

            // Act
            IResult result = await ResetPasswordEnpointHandler.ResetPassword(model, mockUserManager.Object, new CancellationToken());

            // Assert
            var actualValues = ResultHelper.GetValuesFromObject(result);
            actualValues.Should().BeEquivalentTo(expectedValues);
        }

        [Fact]
        public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsOk()
        {
            // Arrange
            ResetPassword model = new()
            {
                Email = "email@mail.com",
                Code = "some token",
                Password = "passwordddd",
                ConfirmPassword = "passwordddd",
            };
            ApplicationUser user = new ApplicationUser();
            mockUserManager.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return user; });
            mockUserManager.Setup(c => c.ResetPasswordAsync(user, model.Code, model.Password))
                .Returns(async () =>
                {
                    return IdentityResult.Success;
                });

            IResult expectedResult = Results.Ok();
            var expectedValues = ResultHelper.GetValuesFromObject(expectedResult);

            // Act
            IResult result = await ResetPasswordEnpointHandler.ResetPassword(model, mockUserManager.Object, new CancellationToken());

            // Assert
            var actualValues = ResultHelper.GetValuesFromObject(result);
            actualValues.Should().BeEquivalentTo(expectedValues);
        }

        [Fact]
        public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsBadRequestWithErrors()
        {
            // Arrange
            ResetPassword model = new()
            {
                Email = "email@mail.com",
                Code = "some token",
                Password = "passwordddd",
                ConfirmPassword = "passwordddd",
            };

            ApplicationUser user = new ApplicationUser();
            mockUserManager.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return user; });

            IdentityErrorDescriber descr = new IdentityErrorDescriber();
            IdentityResult identityResult = IdentityResult.Failed(new IdentityError[]
                    {
                        descr.PasswordTooShort(6),
                        descr.InvalidToken()
                    });
            mockUserManager.Setup(c => c.ResetPasswordAsync(user, model.Code, model.Password))
                .Returns(async () =>
                {
                    return identityResult;
                });

            IResult expectedResult = Results.BadRequest(identityResult.Errors);
            var expectedValues = ResultHelper.GetValuesFromObject(expectedResult);

            // Act
            IResult result = await ResetPasswordEnpointHandler.ResetPassword(model, mockUserManager.Object, new CancellationToken());

            // Assert
            var actualValues = ResultHelper.GetValuesFromObject(result);
            actualValues.Should().BeEquivalentTo(expectedValues);
        }
    }
}