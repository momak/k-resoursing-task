using Claims.Auditing.Abstractions;
using Claims.Data.Abstractions;
using Claims.Services.Abstractions;
using FluentValidation;

namespace Claims.Services.Services
{
    /// <inheritdoc cref="ICoversService"/>
    public class CoversService : ICoversService
    {
        private readonly ICoversRepository _repository;
        private readonly IPremiumCalculator _premiumCalculator;
        private readonly IAuditer _auditer;
        private readonly IValidator<Cover> _validator;

        public CoversService(
            ICoversRepository repository, 
            IPremiumCalculator premiumCalculator, 
            IAuditer auditer,
            IValidator<Cover> validator)
        {
            _repository = repository;
            _premiumCalculator = premiumCalculator;
            _auditer = auditer;
            _validator = validator;
        }

        /// <inheritdoc cref="ICoversService.GetCoversAsync"/>
        public Task<IEnumerable<Cover>> GetCoversAsync() => _repository.GetAllAsync();

        /// <inheritdoc cref="ICoversService.GetCoverAsync(string)"/>
        public Task<Cover?> GetCoverAsync(string id) => _repository.GetByIdAsync(id);

        /// <inheritdoc cref="ICoversService.ComputePremiumAsync(DateTime, DateTime, CoverType)"/>
        public Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType) 
            => Task.FromResult(_premiumCalculator.Compute(startDate, endDate, coverType));

        /// <inheritdoc cref="ICoversService.CreateCoverAsync(Cover)"/>
        public async Task<Cover> CreateCoverAsync(Cover cover)
        {
            await _validator.ValidateAndThrowAsync(cover);

            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = _premiumCalculator.Compute(cover.StartDate, cover.EndDate, cover.Type);

            await _repository.AddAsync(cover);
            _auditer.AuditCover(cover.Id, "POST");

            return cover;
        }

        /// <inheritdoc cref="ICoversService.DeleteCoverAsync(string)"/>
        public async Task DeleteCoverAsync(string id)
        {
            var cover = await _repository.GetByIdAsync(id);
            if (cover is null) return;

            await _repository.DeleteAsync(cover);
            _auditer.AuditCover(id, "DELETE");
        }
    }
}
