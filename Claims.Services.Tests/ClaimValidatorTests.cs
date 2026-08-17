using Claims.Data.Abstractions;
using Claims.Services.Validation;
using FluentValidation.TestHelper;
using Moq;

namespace Claims.Services.Tests;

public class ClaimValidatorTests
{
    private readonly Mock<ICoversRepository> _coversRepository = new();
    private readonly ClaimValidator _sut;

    public ClaimValidatorTests()
    {
        _sut = new ClaimValidator(_coversRepository.Object);
    }

    [Fact]
    public async Task DamageCost_ExceedsMax_FailsValidation()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1)
        };
        _coversRepository.Setup(r => r.GetByIdAsync(cover.Id)).ReturnsAsync(cover);

        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateTime(2026, 2, 1),
            DamageCost = 100_001m
        };

        // Act
        var result = await _sut.TestValidateAsync(claim);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.DamageCost);
    }

    [Fact]
    public async Task DamageCost_AtMax_PassesValidation()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1)
        };
        _coversRepository.Setup(r => r.GetByIdAsync(cover.Id)).ReturnsAsync(cover);

        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateTime(2026, 2, 1),
            DamageCost = 100_000m
        };

        // Act
        var result = await _sut.TestValidateAsync(claim);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.DamageCost);
    }

    [Fact]
    public async Task CreatedDate_OutsideCoverPeriod_FailsValidation()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1)
        };
        _coversRepository.Setup(r => r.GetByIdAsync(cover.Id)).ReturnsAsync(cover);

        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateTime(2026, 7, 1), // after cover ends
            DamageCost = 1000m
        };

        // Act
        var result = await _sut.TestValidateAsync(claim);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task CoverDoesNotExist_FailsValidation()
    {
        // Arrange
        _coversRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Cover?)null);

        var claim = new Claim
        {
            CoverId = "missing-cover",
            Created = new DateTime(2026, 2, 1),
            DamageCost = 1000m
        };

        // Act
        var result = await _sut.TestValidateAsync(claim);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task CreatedDate_WithinCoverPeriod_PassesValidation()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "cover-1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1)
        };
        _coversRepository.Setup(r => r.GetByIdAsync(cover.Id)).ReturnsAsync(cover);

        var claim = new Claim
        {
            CoverId = cover.Id,
            Created = new DateTime(2026, 3, 15),
            DamageCost = 1000m
        };

        // Act
        var result = await _sut.TestValidateAsync(claim);

        // Assert
        Assert.True(result.IsValid);
    }
}