using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BTG.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _col;

    public UserRepository(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var db = client.GetDatabase(options.Value.Database);
        _col = db.GetCollection<User>("users");
        _col.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Username),
            new CreateIndexOptions { Unique = true }));
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct) =>
        _col.Find(u => u.Username == username).FirstOrDefaultAsync(ct)!;

    public Task<User?> GetByIdAsync(string id, CancellationToken ct) =>
        _col.Find(u => u.Id == id).FirstOrDefaultAsync(ct)!;

    public Task AddAsync(User user, CancellationToken ct) => _col.InsertOneAsync(user, cancellationToken: ct);

    public Task UpdateAsync(User user, CancellationToken ct) =>
        _col.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
}
