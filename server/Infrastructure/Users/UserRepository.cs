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
        var ids = await _db.Database
            .SqlQuery<int>($"EXEC CreateUser {user.Username}, {user.Email}, {user.BirthDate}, {user.ImagePath}")
            .ToListAsync();
        return ids[0];
    }


    public Task<List<User>> GetAllAsync() =>
        _db.Users.ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);
}
