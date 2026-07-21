using Domain;

namespace Application.Users;

public class UserService
{
    private readonly IUserRepository _users;
    private readonly IImageStorage _images;

    public UserService(IUserRepository users, IImageStorage images)
    {
        _users = users;
        _images = images;
    }

    public async Task<int> CreateAsync(string username, string email, DateOnly birthDate, Stream image, string fileName)
    {
        var path = await _images.SaveAsync(image, fileName);
        var user = new User
        {
            Username = username,
            Email = email,
            BirthDate = birthDate,
            ImagePath = path
        };
        return await _users.AddAsync(user);
    }

    public Task<List<User>> GetAllAsync() => _users.GetAllAsync();
    public Task<User?> GetByIdAsync(int id) => _users.GetByIdAsync(id);
}
