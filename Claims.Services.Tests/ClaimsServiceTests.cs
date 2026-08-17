using Claims.Auditing.Abstractions;
using Claims.Data.Abstractions;
using Claims.Services.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Claims.Services.Tests;

public class ClaimsServiceTests
{
    private readonly Mock<IClaimsRepository> _repository = new();
    private readonly Mock<IValidator<Claim>> _validator = new();
    private readonly Mock<IAuditer> _auditer = new();
    private readonly ClaimsService _sut;

    public ClaimsServiceTests()
    {
        _sut = new ClaimsService(_repository.Object, _auditer.Object, _validator.Object);
    }

    private void SetupValidationResult(ValidationResult result)
    {
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Claim>>(), It.IsAny<CancellationToken>()))
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
    public async Task CreateClaimAsync_ValidClaim_SetsIdPersistsAndAudits()
    {
        // Arrange
        SetupValidationResult(new ValidationResult());

        var claim = new Claim { CoverId = "cover-1", DamageCost = 1000m, Created = DateTime.UtcNow };

        // Act
        var result = await _sut.CreateClaimAsync(claim);

        // Assert
        Assert.False(string.IsNullOrEmpty(result.Id));
        _repository.Verify(r => r.AddAsync(claim), Times.Once);
        _auditer.Verify(a => a.AuditClaim(result.Id, "POST"), Times.Once);
    }

    [Fact]
    public async Task CreateClaimAsync_InvalidClaim_ThrowsAndDoesNotPersist()
    {
        // Arrange
        SetupValidationResult(new ValidationResult(new[]
        {
            new ValidationFailure("DamageCost", "too high")
        }));

        var claim = new Claim { CoverId = "cover-1", DamageCost = 200_000m };

        // Act
        var act = () => _sut.CreateClaimAsync(claim);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _repository.Verify(r => r.AddAsync(It.IsAny<Claim>()), Times.Never);
        _auditer.Verify(a => a.AuditClaim(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteClaimAsync_ExistingClaim_RemovesAndAudits()
    {
        // Arrange
        var claim = new Claim { Id = "claim-1" };
        _repository.Setup(r => r.GetByIdAsync("claim-1")).ReturnsAsync(claim);

        // Act
        await _sut.DeleteClaimAsync("claim-1");

        // Assert
        _repository.Verify(r => r.DeleteAsync(claim), Times.Once);
        _auditer.Verify(a => a.AuditClaim("claim-1", "DELETE"), Times.Once);
    }

    [Fact]
    public async Task DeleteClaimAsync_NonExistentClaim_DoesNothing()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((Claim?)null);

        // Act
        await _sut.DeleteClaimAsync("missing");

        // Assert
        _repository.Verify(r => r.DeleteAsync(It.IsAny<Claim>()), Times.Never);
        _auditer.Verify(a => a.AuditClaim(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}