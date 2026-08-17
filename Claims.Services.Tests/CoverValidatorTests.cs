using Claims.Services.Validation;
using FluentValidation.TestHelper;

namespace Claims.Services.Tests;

public class CoverValidatorTests
{
    private readonly CoverValidator _sut = new();

    [Fact]
    public void StartDate_InThePast_FailsValidation()
    {
        // Arrange
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.Date.AddDays(-1),
            EndDate = DateTime.UtcNow.Date.AddDays(30),
            Type = CoverType.Yacht
        };

        // Act
        var result = _sut.TestValidate(cover);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.StartDate);
    }

    [Fact]
    public void StartDate_Today_PassesValidation()
    {
        // Arrange
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht
        };

        // Act
        var result = _sut.TestValidate(cover);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.StartDate);
    }

    [Fact]
    public void Period_LongerThanOneYear_FailsValidation()
    {
        // Arrange
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(366),
            Type = CoverType.Tanker
        };

        // Act
        var result = _sut.TestValidate(cover);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Period_ExactlyOneYear_PassesValidation()
    {
        // Arrange
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(365),
            Type = CoverType.Tanker
        };

        // Act
        var result = _sut.TestValidate(cover);

        // Assert
        Assert.True(result.IsValid);
    }
}