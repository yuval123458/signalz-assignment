using Application.Users;
using Domain;
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
    public Task<List<User>> GetAll() => users.GetAllAsync();

    [HttpGet("{id:int}")]
    public Task<User?> GetById(int id) => users.GetByIdAsync(id);
}
