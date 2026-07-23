using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace Web;

[ApiController]
[Route("api/users")]
public class UsersController(UserService users) : ControllerBase
{
    [HttpPost]
    public async Task<int> Create(
        [FromForm] string username, [FromForm] string email,
        [FromForm] DateOnly birthDate, IFormFile image)
    {
        await using var stream = image.OpenReadStream();
        return await users.CreateAsync(username, email, birthDate, stream, image.FileName);
    }

    [HttpGet]
    public Task<List<UserResponseDto>> GetAll() => users.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        var user = await users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }



}
