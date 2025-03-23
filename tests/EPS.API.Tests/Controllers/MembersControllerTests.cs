using EPS.API.Controllers;
using EPS.Application.DTOs;
using EPS.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EPS.API.Tests.Controllers;

public class MembersControllerTests
{
    private readonly Mock<IMemberService> _memberServiceMock;
    private readonly Mock<ILogger<MembersController>> _loggerMock;
    private readonly MembersController _controller;

    public MembersControllerTests()
    {
        _memberServiceMock = new Mock<IMemberService>();
        _loggerMock = new Mock<ILogger<MembersController>>();
        _controller = new MembersController(_memberServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateMember_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateMemberDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            EmployerId = Guid.NewGuid()
        };

        var memberDto = new MemberDto
        {
            Id = Guid.NewGuid(),
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email
        };

        _memberServiceMock.Setup(x => x.CreateMemberAsync(createDto))
            .ReturnsAsync(memberDto);

        // Act
        var result = await _controller.CreateMember(createDto);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnValue = createdAtActionResult.Value.Should().BeAssignableTo<MemberDto>().Subject;
        returnValue.Should().BeEquivalentTo(memberDto);
    }

    [Fact]
    public async Task GetMember_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var memberDto = new MemberDto
        {
            Id = memberId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _memberServiceMock.Setup(x => x.GetMemberByIdAsync(memberId))
            .ReturnsAsync(memberDto);

        // Act
        var result = await _controller.GetMember(memberId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<MemberDto>().Subject;
        returnValue.Should().BeEquivalentTo(memberDto);
    }

    [Fact]
    public async Task GetMember_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        _memberServiceMock.Setup(x => x.GetMemberByIdAsync(memberId))
            .ThrowsAsync(new Exception("Member not found"));

        // Act
        var result = await _controller.GetMember(memberId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllMembers_ReturnsOkResult()
    {
        // Arrange
        var members = new List<MemberDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        _memberServiceMock.Setup(x => x.GetAllMembersAsync())
            .ReturnsAsync(members);

        // Act
        var result = await _controller.GetAllMembers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IReadOnlyList<MemberDto>>().Subject;
        returnValue.Should().BeEquivalentTo(members);
    }

    [Fact]
    public async Task UpdateMember_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var updateDto = new UpdateMemberDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var result = await _controller.UpdateMember(memberId, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _memberServiceMock.Verify(x => x.UpdateMemberAsync(memberId, updateDto), Times.Once);
    }

    [Fact]
    public async Task DeleteMember_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        // Act
        var result = await _controller.DeleteMember(memberId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _memberServiceMock.Verify(x => x.DeleteMemberAsync(memberId), Times.Once);
    }

    [Fact]
    public async Task CheckBenefitEligibility_ReturnsOkResult()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        _memberServiceMock.Setup(x => x.IsMemberEligibleForBenefitsAsync(memberId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckBenefitEligibility(memberId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(true);
    }
}