using Claims.Auditing.Abstractions;
using Claims.Data.Abstractions;
using Claims.Services.Abstractions;
using Claims.Services.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Claims.Services.Tests;

public class CoversServiceTests
{
    private readonly Mock<ICoversRepository> _repository = new();
    private readonly Mock<IPremiumCalculator> _premiumCalculator = new();
    private readonly Mock<IValidator<Cover>> _validator = new();
    private readonly Mock<IAuditer> _auditer = new();
    private readonly CoversService _sut;

    public CoversServiceTests()
    {
        _sut = new CoversService(_repository.Object, _premiumCalculator.Object, _auditer.Object, _validator.Object);
    }

    private void SetupValidationResult(ValidationResult result)
    {
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Cover>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
                return result;
            });
    }

    [Fact]
    public async Task CreateCoverAsync_ValidCover_SetsIdComputesPremiumAndAudits()
    {
        // Arrange
        SetupValidationResult(new ValidationResult());

        var cover = new Cover { StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(30), Type = CoverType.Yacht };
        _premiumCalculator.Setup(p => p.Compute(cover.StartDate, cover.EndDate, cover.Type)).Returns(1500m);

        // Act
        var result = await _sut.CreateCoverAsync(cover);

        // Assert
        Assert.False(string.IsNullOrEmpty(result.Id));
        Assert.Equal(1500m, result.Premium);
        _repository.Verify(r => r.AddAsync(cover), Times.Once);
        _auditer.Verify(a => a.AuditCover(result.Id, "POST"), Times.Once);
    }

    [Fact]
    public async Task CreateCoverAsync_InvalidCover_ThrowsAndDoesNotPersist()
    {
        // Arrange
        SetupValidationResult(new ValidationResult([new ValidationFailure("StartDate", "invalid")]));

        var cover = new Cover { StartDate = DateTime.UtcNow.Date.AddDays(-1), EndDate = DateTime.UtcNow.Date };

        // Act
        var act = () => _sut.CreateCoverAsync(cover);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _repository.Verify(r => r.AddAsync(It.IsAny<Cover>()), Times.Never);
        _auditer.Verify(a => a.AuditCover(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCoverAsync_ExistingCover_RemovesAndAudits()
    {
        // Arrange
        var cover = new Cover { Id = "cover-1" };
        _repository.Setup(r => r.GetByIdAsync("cover-1")).ReturnsAsync(cover);

        // Act
        await _sut.DeleteCoverAsync("cover-1");

        // Assert
        _repository.Verify(r => r.DeleteAsync(cover), Times.Once);
        _auditer.Verify(a => a.AuditCover("cover-1", "DELETE"), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_NonExistentCover_DoesNothing()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((Cover?)null);

        // Act
        await _sut.DeleteCoverAsync("missing");

        // Assert
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Cover>()), Times.Never);
        _auditer.Verify(a => a.AuditCover(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}