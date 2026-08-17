using Claims.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories
{
    /// <inheritdoc cref="ICoversRepository"/>
    public class CoversRepository : ICoversRepository
    {
        private readonly ClaimsContext _context;

        public CoversRepository(ClaimsContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="ICoversRepository.GetAllAsync"/>
        public async Task<IEnumerable<Cover>> GetAllAsync() =>
            await _context.Covers.ToListAsync();

        /// <inheritdoc cref="ICoversRepository.GetByIdAsync(string)"/>
        public async Task<Cover?> GetByIdAsync(string id) =>
            await _context.Covers.SingleOrDefaultAsync(c => c.Id == id);

        /// <inheritdoc cref="ICoversRepository.AddAsync(Cover)"/>
        public async Task AddAsync(Cover cover)
        {
            _context.Covers.Add(cover);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="ICoversRepository.DeleteAsync(Cover)"/>
        public async Task DeleteAsync(Cover cover)
        {
            _context.Covers.Remove(cover);
            await _context.SaveChangesAsync();
        }
    }
}
