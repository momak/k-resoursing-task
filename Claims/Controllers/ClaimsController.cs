using Claims.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;


namespace Claims.Controllers
{
    /// <summary>
    /// Claims Api Controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimsService _claimsService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService)
        {
            _logger = logger;
            _claimsService = claimsService;
        }

        /// <summary>
        /// Gets all claims.
        /// </summary>
        /// <returns>A list of all claims.</returns>
        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimsService.GetClaimsAsync();
        }

        /// <summary>
        /// Gets a claim by its ID.
        /// </summary>
        /// <param name="id">The ID of the claim to retrieve.</param>
        /// <returns>The claim with the specified ID, or null if not found.</returns>
        [HttpGet("{id}")]
        public async Task<Claim?> GetAsync(string id)
        {
            return await _claimsService.GetClaimAsync(id);
        }

        /// <summary>
        /// Creates a new claim.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>The created claim.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            var created = await _claimsService.CreateClaimAsync(claim);
            return Ok(created);
        }

        /// <summary>
        /// Deletes a claim by its ID.
        /// </summary>
        /// <param name="id">The ID of the claim to delete.</param>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _claimsService.DeleteClaimAsync(id);
        }


    }
}
