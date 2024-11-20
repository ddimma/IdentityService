using AutoFixture;
using FluentAssertions;
using IdentityService.CQRS.User.Commands.ResetPassword;
using IdentityService.Endpoints.User;
using IdentityService.Entities;
using IdentityService.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;

namespace IdentityService.Tests.Endpoints.User;

public class ResetPasswordEndpointHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ISetup<UserManager<ApplicationUser>, Task<IdentityResult>> _resetPasswordAsyncSetup;
    private readonly ISetup<UserManager<ApplicationUser>, Task<ApplicationUser?>> _findByEmailAsyncSetup;
    private readonly ApplicationUser _currentUser;
    private readonly ResetPasswordCommand _defaultModel;
    public ResetPasswordEndpointHandlerTests() 
    {
        var autoFixture = new Fixture();
        
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object, 
            Array.Empty<IUserValidator<ApplicationUser>>(), 
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        _mediatorMock = new Mock<IMediator>();
        
        var randomPassword = autoFixture.Create<string>();
        autoFixture.Customize<ResetPasswordCommand>(c => c
                .With(x => x.Email, $"{autoFixture.Create<string>()}@mail.com")
                .With(x => x.Code, autoFixture.Create<string>())
                .With(x => x.Password, randomPassword)
                .With(x => x.ConfirmPassword, randomPassword)
        );

        _defaultModel = autoFixture.Create<ResetPasswordCommand>();
        _currentUser = autoFixture.Build<ApplicationUser>()
            .With(x => x.FirstName, autoFixture.Create<string>())
            .With(x => x.LastName, autoFixture.Create<string>())
            .Create();

        _resetPasswordAsyncSetup = _userManagerMock.Setup(c => c.ResetPasswordAsync(_currentUser, _defaultModel.Code, _defaultModel.Password));
        _findByEmailAsyncSetup = _userManagerMock.Setup(c => c.FindByEmailAsync(_defaultModel.Email));
    }
    
    [Fact]
    public async Task ResetPassword_WhenUserWithThisEmailDoesNotExists_ReturnsBadRequestWithIdentityError()
    {
        var identityErrors = new List<IdentityError>
        {
            new()
            {
                Code = CodeDescriptions.UserCode,
                Description = CodeDescriptions.UserCodeDescription
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Results.BadRequest(identityErrors));

        // Act
        var actualResult = await ResetPasswordEndpointHandler.ResetPassword(
            _defaultModel, _mediatorMock.Object, new CancellationToken());

        var actualPayload = (actualResult as BadRequest<List<IdentityError>>)?.Value;
        var expectedPayload = identityErrors;

        // Assert
        actualPayload.Should().BeEquivalentTo(expectedPayload);
    }

    [Fact]
    public async Task ResetPassword_WhenPasswordAndConfirmedPasswordDoNotMatch_ReturnsBadRequestWithIdentityError()
    {
        _defaultModel.Password = "some password";
        _defaultModel.ConfirmPassword = "another password";

        var expectedError = new List<IdentityError> 
        {
            new()
            {
                Code = CodeDescriptions.PasswordCode,
                Description = CodeDescriptions.PasswordCodeDescription
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Results.BadRequest(expectedError));

        // Act
        var actualResult = await ResetPasswordEndpointHandler.ResetPassword(
            _defaultModel, 
            _mediatorMock.Object, 
            new CancellationToken());

        // Assert
        actualResult.Should().BeOfType<BadRequest<List<IdentityError>>>();
        var actual = actualResult as BadRequest<List<IdentityError>>;
    
        actual.Should().NotBeNull();
        actual!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        actual.Value.Should().BeEquivalentTo(expectedError);
    }

    [Fact]
    public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsOk()
    {
        // Arrange
        _findByEmailAsyncSetup
            .ReturnsAsync(_currentUser);

        _resetPasswordAsyncSetup
            .ReturnsAsync(IdentityResult.Success);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Results.Ok());

        // Act
        var actualResult = await ResetPasswordEndpointHandler.ResetPassword(
            _defaultModel, _mediatorMock.Object, new CancellationToken());

        // Assert
        actualResult.Should().BeOfType<Ok>();
    }

    [Fact]
    public async Task ResetPassword_WhenIdentityResultSucceeded_ReturnsBadRequestWithErrors()
    {
        // Arrange

        _findByEmailAsyncSetup
            .ReturnsAsync(() => _currentUser);

        var identityErrorDescriber = new IdentityErrorDescriber();
        var identityResult = IdentityResult.Failed(identityErrorDescriber.PasswordTooShort(6), identityErrorDescriber.InvalidToken());
        _userManagerMock.Setup(c => c.ResetPasswordAsync(_currentUser, _defaultModel.Code, _defaultModel.Password))
            .ReturnsAsync (() => identityResult);

        var expectedResult = Results.BadRequest(identityResult.Errors);
        var expected = expectedResult as BadRequest<List<IdentityError>>;

        // Act
        var actualResult = await ResetPasswordEndpointHandler.ResetPassword(_defaultModel, _mediatorMock.Object, new CancellationToken());

        // Assert
        var actual = actualResult as BadRequest<List<IdentityError>>;
        actual.Should().BeEquivalentTo(expected);
    }
}