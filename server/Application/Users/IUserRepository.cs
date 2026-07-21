using Domain;

namespace Application.Users;

public interface IUserRepository
{
    Task<int> AddAsync(User user);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
}