using System.ComponentModel.DataAnnotations;
using PeopleDirectory.Constants;
using PeopleDirectory.Exceptions;
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
        try
        {
            var newUser = await repository.CreateUser(user);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newUser.Id },
                newUser);
        }
        catch (Exception ex)
        {
            return ex switch
            {
                DuplicateEmailException => Conflict(new { errorCode = "form.errors.DUPLICATE_EMAIL", message = ex.Message }),
                ValidationException => BadRequest(new { errorCode = "form.errors.DATA_VALIDATION", message = "Data validation failed.", details = ex.Message }),
                UnauthorizedAccessException => Forbid(),
                // Catch-all for unexpected errors
                _ => StatusCode(500, new { errorCode = "form.errors.UNEXPECTED", message = "An unexpected error occurred." })
            };
        }
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
