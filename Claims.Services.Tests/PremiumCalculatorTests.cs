using Claims.Services.Services;

namespace Claims.Services.Tests;

public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _sut = new();

    [Theory]
    [InlineData(CoverType.Yacht, 1.1)]
    [InlineData(CoverType.PassengerShip, 1.2)]
    [InlineData(CoverType.Tanker, 1.5)]
    [InlineData(CoverType.BulkCarrier, 1.3)]  // "other types"
    [InlineData(CoverType.ContainerShip, 1.3)]
    public void Compute_ForShortPeriod_AppliesCorrectTypeMultiplier(CoverType coverType, decimal multiplier)
    {
        // Arrange
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(10); // fully within the "full rate" 30-day tier

        // Act
        var result = _sut.Compute(start, end, coverType);

        // Assert
        Assert.Equal(10 * 1250m * multiplier, result);
    }

    [Fact]
    public void Compute_ExactlyThirtyDays_AllDaysAtFullRate()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(30);

        // Act
        var result = _sut.Compute(start, end, CoverType.Tanker);

        // Assert
        Assert.Equal(30 * 1250m * 1.5m, result);
    }

    [Fact]
    public void Compute_180Days_Yacht_AppliesFullRateThenFivePercentDiscount()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(180);

        var dayRate = 1250m * 1.1m;
        var expected = 30 * dayRate + 150 * dayRate * 0.95m;

        // Act
        var result = _sut.Compute(start, end, CoverType.Yacht);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_365Days_NonYacht_AppliesAllThreeTiers()
    {
        // Arrange
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(365);

        var dayRate = 1250m * 1.3m; // "other types"
        var expected = 30 * dayRate
                       + 150 * dayRate * 0.98m
                       + 185 * dayRate * 0.97m; // 365 - 30 - 150 = 185, 2%+1% stacked

        // Act
        var result = _sut.Compute(start, end, CoverType.BulkCarrier);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compute_NoDays_ReturnsZero()
    {
        // Arrange
        var date = new DateTime(2026, 1, 1);

        // Act
        var result = _sut.Compute(date, date, CoverType.Yacht);

        // Assert
        Assert.Equal(0m, result);
    }
}