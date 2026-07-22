using Application.Users;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<int> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user.Id;
    }

    public Task<List<User>> GetAllAsync() =>
        _db.Users.ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);
}
