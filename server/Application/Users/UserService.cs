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

    public async Task<List<UserResponseDto>> GetAllAsync() =>
    (await _users.GetAllAsync()).Select(u => ToDto(u)).ToList();

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    private UserResponseDto ToDto(User u) =>
        new(u.Id, u.Username, u.Email, u.BirthDate, u.ImagePath);

}
