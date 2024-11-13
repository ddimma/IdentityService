using FluentAssertions;
using IdentityService.Endpoints.User;
using IdentityService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace IdentityService.Tests.Endpoints.User
{
    public class ResetPasswordEndpointHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        public ResetPasswordEndpointHandlerTests() 
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
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
            _userManagerMock.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return (null); });

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "User",
                    Description = "This user does not exists"
                }});
            var expected = expectedResult as BadRequest<List<IdentityError>>;

            // Act
            IResult actualResult = await  ResetPasswordEnpointHandler.ResetPassword(model, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
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
            _userManagerMock.Setup(c => c.FindByEmailAsync(model.Email)).Returns( async () => { return new ApplicationUser { }; });

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "Password",
                    Description = "The password and confirmation password do not match."
                } });
            var expected = expectedResult as BadRequest<List<IdentityError>>;

            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(model, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
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
            _userManagerMock.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return user; });
            _userManagerMock.Setup(c => c.ResetPasswordAsync(user, model.Code, model.Password))
                .Returns(async () =>
                {
                    return IdentityResult.Success;
                });

            IResult expectedResult = Results.Ok();
            var expected = expectedResult as Ok;

            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(model, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as Ok;
            actual.Should().BeEquivalentTo(expected);
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
            _userManagerMock.Setup(c => c.FindByEmailAsync(model.Email)).Returns(async () => { return user; });

            IdentityErrorDescriber descr = new IdentityErrorDescriber();
            IdentityResult identityResult = IdentityResult.Failed(new IdentityError[]
                    {
                        descr.PasswordTooShort(6),
                        descr.InvalidToken()
                    });
            _userManagerMock.Setup(c => c.ResetPasswordAsync(user, model.Code, model.Password))
                .Returns(async () =>
                {
                    return identityResult;
                });

            IResult expectedResult = Results.BadRequest(identityResult.Errors);
            var expected = expectedResult as BadRequest<List<IdentityError>>;
            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(model, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
        }
    }
}