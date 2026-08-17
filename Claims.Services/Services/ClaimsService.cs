using Claims.Auditing.Abstractions;
using Claims.Data.Abstractions;
using Claims.Services.Abstractions;
using FluentValidation;

namespace Claims.Services.Services
{
    /// <inheritdoc cref="IClaimsService"/>
    public class ClaimsService : IClaimsService
    {
        private readonly IClaimsRepository _repository;
        private readonly IAuditer _auditer;
        private readonly IValidator<Claim> _validator;

        public ClaimsService(
            IClaimsRepository repository, 
            IAuditer auditer,
            IValidator<Claim> validator)
        {
            _repository = repository;
            _auditer = auditer;
            _validator = validator;
        }

        /// <inheritdoc cref="IClaimsService.GetClaimsAsync"/>
        public Task<IEnumerable<Claim>> GetClaimsAsync() => _repository.GetAllAsync();

        /// <inheritdoc cref="IClaimsService.GetClaimAsync"/>
        public Task<Claim?> GetClaimAsync(string id) => _repository.GetByIdAsync(id);

        /// <inheritdoc cref="IClaimsService.CreateClaimAsync"/>
        public async Task<Claim> CreateClaimAsync(Claim claim)
        {
            await _validator.ValidateAndThrowAsync(claim);

            claim.Id = Guid.NewGuid().ToString();

            await _repository.AddAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");

            return claim;
        }

        /// <inheritdoc cref="IClaimsService.DeleteClaimAsync"/>
        public async Task DeleteClaimAsync(string id)
        {
            var claim = await _repository.GetByIdAsync(id);
            if (claim is null) return;

            await _repository.DeleteAsync(claim);
            _auditer.AuditClaim(id, "DELETE");
        }
    }
}
