using Moq;
using UserService.domain.entities;
using Xunit;
using UserService.application;

namespace UserService.Tests;

public class UnitTests
{
    private readonly application.UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UnitTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new application.UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUserAndReturnId()
    {
        var username = "testuser";
        var password = "password123";
        var userId = "123";

        _userRepositoryMock.Setup(repo => repo.Add(It.IsAny<User>()))
                          .Callback<User>(user => user.Id = userId)
                          .Returns(Task.CompletedTask);

        var result = await _userService.RegisterUser(username, password);

        Assert.Equal(userId, result);
        _userRepositoryMock.Verify(repo => repo.Add(It.Is<User>(u => u.Username == username)), Times.Once);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        var username = "testuser";
        var password = "wrongpassword";

        _userRepositoryMock.Setup(repo => repo.FindByUsername(username))
                           .ReturnsAsync((User)null);

        var result = await _userService.AuthenticateUser(username, password);

        Assert.Null(result);
        _userRepositoryMock.Verify(repo => repo.FindByUsername(username), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        var userId = "123";
        var user = new User("testuser", "password123") { Id = userId };

        _userRepositoryMock.Setup(repo => repo.FindById(userId))
                           .ReturnsAsync(user);

        var result = await _userService.GetUserById(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        _userRepositoryMock.Verify(repo => repo.FindById(userId), Times.Once);
    }

    [Fact]
    public async Task GetCredit_ShouldReturnCredit_WhenUserExists()
    {
        var userId = "123";
        var userCredit = 50;

        _userRepositoryMock.Setup(repo => repo.GetCredit(userId))
                           .ReturnsAsync(userCredit);

        var result = await _userService.GetCredit(userId);

        Assert.Equal(userCredit, result);
        _userRepositoryMock.Verify(repo => repo.GetCredit(userId), Times.Once);
    }

}