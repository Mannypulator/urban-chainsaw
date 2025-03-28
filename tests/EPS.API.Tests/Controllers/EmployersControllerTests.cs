using EPS.API.Controllers;
using EPS.Application.DTOs;
using EPS.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPS.API.Tests.Controllers;

public class EmployersControllerTests
{
    private readonly EmployersController _controller;
    private readonly Mock<IEmployerService> _employerServiceMock;
    private readonly Mock<ILogger<EmployersController>> _loggerMock;

    public EmployersControllerTests()
    {
        _employerServiceMock = new Mock<IEmployerService>();
        _loggerMock = new Mock<ILogger<EmployersController>>();
        _controller = new EmployersController(_employerServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateEmployer_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateEmployerDto
        {
            Name = "Test Company",
            RegistrationNumber = "REG123",
            Address = "123 Business St",
            ContactPerson = "John Manager",
            Email = "john@testcompany.com",
            Phone = "+1234567890"
        };

        var employerDto = new EmployerDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            RegistrationNumber = createDto.RegistrationNumber
        };

        _employerServiceMock.Setup(x => x.CreateEmployerAsync(createDto))
            .ReturnsAsync(employerDto);

        // Act
        var result = await _controller.CreateEmployer(createDto);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnValue = createdAtActionResult.Value.Should().BeAssignableTo<EmployerDto>().Subject;
        returnValue.Should().BeEquivalentTo(employerDto);
    }

    [Fact]
    public async Task GetEmployer_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var employerDto = new EmployerDto
        {
            Id = employerId,
            Name = "Test Company",
            RegistrationNumber = "REG123"
        };

        _employerServiceMock.Setup(x => x.GetEmployerByIdAsync(employerId))
            .ReturnsAsync(employerDto);

        // Act
        var result = await _controller.GetEmployer(employerId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<EmployerDto>().Subject;
        returnValue.Should().BeEquivalentTo(employerDto);
    }

    [Fact]
    public async Task GetAllEmployers_ReturnsOkResult()
    {
        // Arrange
        var employers = new List<EmployerDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Company 1", RegistrationNumber = "REG1" },
            new() { Id = Guid.NewGuid(), Name = "Company 2", RegistrationNumber = "REG2" }
        };

        _employerServiceMock.Setup(x => x.GetAllEmployersAsync())
            .ReturnsAsync(employers);

        // Act
        var result = await _controller.GetAllEmployers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IReadOnlyList<EmployerDto>>().Subject;
        returnValue.Should().BeEquivalentTo(employers);
    }

    [Fact]
    public async Task GetActiveEmployers_ReturnsOkResult()
    {
        // Arrange
        var employers = new List<EmployerDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Active Company 1", IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "Active Company 2", IsActive = true }
        };

        _employerServiceMock.Setup(x => x.GetActiveEmployersAsync())
            .ReturnsAsync(employers);

        // Act
        var result = await _controller.GetActiveEmployers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IReadOnlyList<EmployerDto>>().Subject;
        returnValue.Should().BeEquivalentTo(employers);
    }

    [Fact]
    public async Task UpdateEmployer_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var updateDto = new UpdateEmployerDto
        {
            Name = "Updated Company",
            RegistrationNumber = "REG123",
            Address = "456 Business Ave"
        };

        // Act
        var result = await _controller.UpdateEmployer(employerId, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _employerServiceMock.Verify(x => x.UpdateEmployerAsync(employerId, updateDto), Times.Once);
    }

    [Fact]
    public async Task DeactivateEmployer_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var employerId = Guid.NewGuid();

        // Act
        var result = await _controller.DeactivateEmployer(employerId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _employerServiceMock.Verify(x => x.DeactivateEmployerAsync(employerId), Times.Once);
    }

    [Fact]
    public async Task GetEmployerMembers_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var members = new List<MemberDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        _employerServiceMock.Setup(x => x.GetEmployerMembersAsync(employerId))
            .ReturnsAsync(members);

        // Act
        var result = await _controller.GetEmployerMembers(employerId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IReadOnlyList<MemberDto>>().Subject;
        returnValue.Should().BeEquivalentTo(members);
    }

    [Fact]
    public async Task GetTotalContributions_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;
        var totalContributions = 5000m;

        _employerServiceMock.Setup(x => x.GetTotalContributionsAsync(employerId, startDate, endDate))
            .ReturnsAsync(totalContributions);

        // Act
        var result = await _controller.GetTotalContributions(employerId, startDate, endDate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(totalContributions);
    }

    [Fact]
    public async Task ValidateEmployer_ReturnsOkResult()
    {
        // Arrange
        var employerId = Guid.NewGuid();
        var isValid = true;

        _employerServiceMock.Setup(x => x.ValidateEmployerAsync(employerId))
            .ReturnsAsync(isValid);

        // Act
        var result = await _controller.ValidateEmployer(employerId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(isValid);
    }
}