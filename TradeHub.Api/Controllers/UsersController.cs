using Microsoft.AspNetCore.Mvc;
using TradeHub.Api.Models;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    //Add Mapper/Dto to Alll Endpoints
    [HttpGet("{id}", Name = "GetUser")]
    public async Task<ActionResult<User>> GetAsync(long id)
    {
        var user = await _service.GetUserByIdAsync(id);
        return user is not null ? Ok(user) : NotFound();
    }
    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateAsync([FromBody] User user)
    {
        await _service.CreateUserAsync(user);
        return Created($"/users/{user.Id}", user);
    }

    [HttpPut("{id}", Name = "UpdateStudent")]
    // "/users/{id}"
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] User user)
    {
        bool result = await _service.UpdateUserAsync(id, user);

        if (!result) return BadRequest();

        return Ok(user);
    }


    [HttpDelete("{id}", Name = "DeleteStudent")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        bool result = await _service.DeleteUserAsync(id);

        if (!result) return NotFound();
        return NoContent();
    }
    

}