namespace PeopleDirectory.Controllers;

using Microsoft.AspNetCore.Mvc;
using Repositories;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(UserRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var people = await repository.GetAll();
        return Ok(people);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await repository.GetById(id);
        if (person is null)
            return NotFound();

        return Ok(person);
    }
}
