using PeopleDirectory.Constants;
using PeopleDirectory.Models;

namespace PeopleDirectory.Controllers;

using Microsoft.AspNetCore.Mvc;
using Repositories;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(UserRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDto user)
    {
        var created = await repository.AddAsync(user);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await repository.GetById(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] int limit = SearchConstants.DefaultLimit)
    {
        limit = Math.Clamp(limit, SearchConstants.MinLimit, SearchConstants.MaxLimit);

        var results = await repository.SearchByName(name, limit);
        return Ok(results);
    }
}
