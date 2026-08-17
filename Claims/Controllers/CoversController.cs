using Claims.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// Covers Api Controller.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoversService _coversService;

    public CoversController(ILogger<CoversController> logger, ICoversService coversService)
    {
        _logger = logger;
        _coversService = coversService;
    }

    /// <summary>
    /// Computes the premium for a given cover type and date range.
    /// </summary>
    /// <param name="startDate">The start date of the coverage period.</param>
    /// <param name="endDate">The end date of the coverage period.</param>
    /// <param name="coverType">The type of cover.</param>
    /// <returns>The computed premium amount.</returns>
    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var result = await _coversService.ComputePremiumAsync(startDate, endDate, coverType);
        return Ok(result);
    }

    /// <summary>
    /// Gets all covers.
    /// </summary>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coversService.GetCoversAsync();
        return Ok(results);
    }

    /// <summary>
    /// Gets a specific cover by its ID.
    /// </summary>
    /// <param name="id">The ID of the cover to retrieve.</param>
    /// <returns>The cover with the specified ID, or null if not found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var results = await _coversService.GetCoversAsync();
        return Ok(results.SingleOrDefault(cover => cover.Id == id));
    }

    /// <summary>
    /// Creates a new cover.
    /// </summary>
    /// <param name="cover">The cover to create.</param>
    /// <returns>The created cover.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        var created = await _coversService.CreateCoverAsync(cover);
        return Ok(created);
    }

    /// <summary>
    /// Deletes a cover by its ID.
    /// </summary>
    /// <param name="id">The ID of the cover to delete.</param>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coversService.DeleteCoverAsync(id);
    }   
}
