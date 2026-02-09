using PeopleDirectory.Constants;

namespace PeopleDirectory.Controllers;

using Microsoft.AspNetCore.Mvc;
using Repositories;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(UserRepository repository) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] int limit = SearchConstants.DefaultLimit)
    {
        limit = Math.Clamp(limit, SearchConstants.MinLimit, SearchConstants.MaxLimit);

        var results = await repository.SearchByName(name, limit);
        return Ok(results);
    }
}
