using Claims.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories
{
    /// <inheritdoc cref="IClaimsRepository"/>
    public class ClaimsRepository : IClaimsRepository
    {
        private readonly ClaimsContext _context;

        public ClaimsRepository(ClaimsContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IClaimsRepository.GetAllAsync"/> 
        public async Task<IEnumerable<Claim>> GetAllAsync() =>
            await _context.Claims.ToListAsync();

        /// <inheritdoc cref="IClaimsRepository.GetByIdAsync(string)"/>
        public async Task<Claim?> GetByIdAsync(string id) =>
            await _context.Claims.SingleOrDefaultAsync(c => c.Id == id);

        /// <inheritdoc cref="IClaimsRepository.AddAsync(Claim)"/>
        public async Task AddAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IClaimsRepository.DeleteAsync(Claim)"/>
        public async Task DeleteAsync(Claim claim)
        {
            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
        }
    }
}
