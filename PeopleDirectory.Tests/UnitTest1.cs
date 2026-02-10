using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Moq;
using PeopleDirectory.Exceptions;
using PeopleDirectory.Models;
using PeopleDirectory.Repositories;
using FluentAssertions;
using PeopleDirectory.Infrastructure;

namespace PeopleDirectory.Tests;

public class UserRepositoryTests
{
    private readonly Mock<IFileWrapper> _fileMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly UserRepository _repository;
    private const string FakePath = "C:/App/Data/users.json";

    public UserRepositoryTests()
    {
        _fileMock = new Mock<IFileWrapper>();
        _envMock = new Mock<IWebHostEnvironment>();

        // Setup the environment to return a predictable path
        _envMock.Setup(e => e.ContentRootPath).Returns("C:/App");

        _repository = new UserRepository(_envMock.Object, _fileMock.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldAssignId1_WhenNoFileExists()
    {
        // Arrange
        _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
        var newUser = new UserDto
        {
            Email = "first@test.com",
            FirstName = "Adam",
            LastName = null,
            JobTitle = null,
            Phone = null
        };

        // Act
        var result = await _repository.CreateUser(newUser);

        // Assert
        result.Id.Should().Be(1);
        // Verify we actually tried to save the JSON
        _fileMock.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowDuplicateEmailException_WhenEmailMatches()
    {
        // Arrange
        var existing = new List<UserDto> { new()
            {
                Id = 1,
                Email = "SAME@test.com",
                FirstName = null,
                LastName = null,
                JobTitle = null,
                Phone = null
            }
        };
        _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
        _fileMock.Setup(f => f.ReadAllTextAsync(It.IsAny<string>()))
                 .ReturnsAsync(JsonSerializer.Serialize(existing));

        var newUser = new UserDto
        {
            Email = "same@test.com",
            FirstName = null,
            LastName = null,
            JobTitle = null,
            Phone = null
        }; // Different case

        // Act
        var act = () => _repository.CreateUser(newUser);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);

        // Act
        var result = await _repository.GetById(99);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("Ali", 5, 1)]  // Alice exists
    [InlineData("ali", 5, 1)]  // Case-insensitive
    [InlineData("Bo", 5, 1)]   // Bob exists
    [InlineData("NonExistent", 5, 0)]
    public async Task SearchByName_ShouldFilterAndLimitResults(string term, int limit, int expectedCount)
    {
        // Arrange
        var users = new List<UserDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Smith",
                JobTitle = null,
                Phone = null,
                Email = null
            },
            new()
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Jones",
                JobTitle = null,
                Phone = null,
                Email = null
            }
        };
        _fileMock.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
        _fileMock.Setup(f => f.ReadAllTextAsync(It.IsAny<string>()))
            .ReturnsAsync(JsonSerializer.Serialize(users));

        // Act
        var results = await _repository.SearchByName(term, limit);

        // Assert
        results.Should().HaveCount(expectedCount);
    }
}