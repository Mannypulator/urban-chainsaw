using EPS.API.Controllers;
using EPS.Application.DTOs;
using EPS.Application.Services;
using EPS.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EPS.API.Tests.Controllers;

public class ContributionsControllerTests
{
    private readonly Mock<IContributionService> _contributionServiceMock;
    private readonly Mock<ILogger<ContributionsController>> _loggerMock;
    private readonly ContributionsController _controller;

    public ContributionsControllerTests()
    {
        _contributionServiceMock = new Mock<IContributionService>();
        _loggerMock = new Mock<ILogger<ContributionsController>>();
        _controller = new ContributionsController(_contributionServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateContribution_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateContributionDto
        {
            MemberId = Guid.NewGuid(),
            Amount = 1000,
            ContributionDate = DateTime.UtcNow,
            Type = ContributionType.Monthly,
            TransactionReference = "REF123"
        };

        var contributionDto = new ContributionDto
        {
            Id = Guid.NewGuid(),
            MemberId = createDto.MemberId,
            Amount = createDto.Amount,
            ContributionDate = createDto.ContributionDate
        };

        _contributionServiceMock.Setup(x => x.CreateContributionAsync(createDto))
            .ReturnsAsync(contributionDto);

        // Act
        var result = await _controller.CreateContribution(createDto);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnValue = createdAtActionResult.Value.Should().BeAssignableTo<ContributionDto>().Subject;
        returnValue.Should().BeEquivalentTo(contributionDto);
    }

    [Fact]
    public async Task GetContribution_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var contributionId = Guid.NewGuid();
        var contributionDto = new ContributionDto
        {
            Id = contributionId,
            MemberId = Guid.NewGuid(),
            Amount = 1000,
            ContributionDate = DateTime.UtcNow
        };

        _contributionServiceMock.Setup(x => x.GetContributionByIdAsync(contributionId))
            .ReturnsAsync(contributionDto);

        // Act
        var result = await _controller.GetContribution(contributionId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<ContributionDto>().Subject;
        returnValue.Should().BeEquivalentTo(contributionDto);
    }

    [Fact]
    public async Task GetMemberContributions_ReturnsOkResult()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<ContributionDto>
        {
            new() { Id = Guid.NewGuid(), MemberId = memberId, Amount = 1000 },
            new() { Id = Guid.NewGuid(), MemberId = memberId, Amount = 2000 }
        };

        _contributionServiceMock.Setup(x => x.GetMemberContributionsAsync(memberId))
            .ReturnsAsync(contributions);

        // Act
        var result = await _controller.GetMemberContributions(memberId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IReadOnlyList<ContributionDto>>().Subject;
        returnValue.Should().BeEquivalentTo(contributions);
    }

    [Fact]
    public async Task ValidateContribution_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var contributionId = Guid.NewGuid();

        // Act
        var result = await _controller.ValidateContribution(contributionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _contributionServiceMock.Verify(x => x.ValidateContributionAsync(contributionId), Times.Once);
    }

    [Fact]
    public async Task ProcessContribution_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var contributionId = Guid.NewGuid();

        // Act
        var result = await _controller.ProcessContribution(contributionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _contributionServiceMock.Verify(x => x.ProcessContributionAsync(contributionId), Times.Once);
    }

    [Fact]
    public async Task CalculateInterest_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var contributionId = Guid.NewGuid();

        // Act
        var result = await _controller.CalculateInterest(contributionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _contributionServiceMock.Verify(x => x.CalculateInterestAsync(contributionId), Times.Once);
    }

    [Fact]
    public async Task GetTotalContributionsForPeriod_ReturnsOkResult()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;
        var totalContributions = 3000m;

        _contributionServiceMock.Setup(x => x.GetTotalContributionsForPeriodAsync(memberId, startDate, endDate))
            .ReturnsAsync(totalContributions);

        // Act
        var result = await _controller.GetTotalContributionsForPeriod(memberId, startDate, endDate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(totalContributions);
    }

    [Fact]
    public async Task ValidateMonthlyContribution_ReturnsOkResult()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributionDate = DateTime.UtcNow;
        var hasContribution = true;

        _contributionServiceMock.Setup(x => x.ValidateMonthlyContributionAsync(memberId, contributionDate))
            .ReturnsAsync(hasContribution);

        // Act
        var result = await _controller.ValidateMonthlyContribution(memberId, contributionDate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(hasContribution);
    }
}