using FluentAssertions;
using IdentityService.Endpoints.User;
using IdentityService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace IdentityService.Tests.Endpoints.User
{
    public class ResetPasswordEndpointHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Moq.Language.Flow.ISetup<UserManager<ApplicationUser>, Task<IdentityResult>> _resetPasswordAsyncSetup;
        private readonly Moq.Language.Flow.ISetup<UserManager<ApplicationUser>, Task<ApplicationUser>> _findByEmailAsyncSetup;
        private readonly ApplicationUser _currentUser;
        private readonly ResetPassword _defaultModel;
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

            _defaultModel = new()
            {
                Email = "email@mail.com",
                Code = "some token",
                Password = "passwordddd",
                ConfirmPassword = "passwordddd",
            };
            _currentUser = new ApplicationUser();

            _resetPasswordAsyncSetup = _userManagerMock.Setup(c => c.ResetPasswordAsync(_currentUser, _defaultModel.Code, _defaultModel.Password));
            _findByEmailAsyncSetup = _userManagerMock.Setup(c => c.FindByEmailAsync(_defaultModel.Email));
        }
        [Fact]
        public async Task ResetPassword_WhenUserUserWithThisEmailDoesNotExists_ReturnsBadRequestWithIdentityError() 
        {
            // Arrange
            _resetPasswordAsyncSetup
                .ReturnsAsync(() => null);

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "User",
                    Description = "This user does not exists"
                }});
            var expected = expectedResult as BadRequest<List<IdentityError>>;

            // Act
            IResult actualResult = await  ResetPasswordEnpointHandler.ResetPassword(_defaultModel, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ResetPassword_WhenPasswordAndConfirmedPasswordDoNotMatch_ReturnsBadRequestWithIdentityError()
        {
            // Arrange
            _defaultModel.Password = "some password";
            _defaultModel.ConfirmPassword = "another password";

            _findByEmailAsyncSetup
                .ReturnsAsync (() => new ApplicationUser { });

            IResult expectedResult = Results.BadRequest(
                new List<IdentityError> {
                new IdentityError
                {
                    Code = "Password",
                    Description = "The password and confirmation password do not match."
                } });
            var expected = expectedResult as BadRequest<List<IdentityError>>;

            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(_defaultModel, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsOk()
        {
            // Arrange
            _findByEmailAsyncSetup
                .ReturnsAsync(() => _currentUser);
            _resetPasswordAsyncSetup
                .ReturnsAsync(() =>IdentityResult.Success);

            IResult expectedResult = Results.Ok();
            var expected = expectedResult as Ok;

            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(_defaultModel, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as Ok;
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsBadRequestWithErrors()
        {
            // Arrange

            _findByEmailAsyncSetup
                .ReturnsAsync(() => _currentUser);

            IdentityErrorDescriber descr = new IdentityErrorDescriber();
            IdentityResult identityResult = IdentityResult.Failed(new IdentityError[]
                    {
                        descr.PasswordTooShort(6),
                        descr.InvalidToken()
                    });
            _userManagerMock.Setup(c => c.ResetPasswordAsync(_currentUser, _defaultModel.Code, _defaultModel.Password))
                .ReturnsAsync (() => identityResult);

            IResult expectedResult = Results.BadRequest(identityResult.Errors);
            var expected = expectedResult as BadRequest<List<IdentityError>>;

            // Act
            IResult actualResult = await ResetPasswordEnpointHandler.ResetPassword(_defaultModel, _userManagerMock.Object, new CancellationToken());

            // Assert
            var actual = actualResult as BadRequest<List<IdentityError>>;
            actual.Should().BeEquivalentTo(expected);
        }
    }
}