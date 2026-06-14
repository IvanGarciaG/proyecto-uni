using Files.Application;
using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Infrastructure.Persistence;

public class UserFileRepository(FilesDbContext db) : IUserFileRepository
{
    public async Task AddAsync(UserFile file, CancellationToken ct) => await db.UserFiles.AddAsync(file, ct);

    public Task<UserFile?> GetByIdAsync(Guid fileId, CancellationToken ct) =>
        db.UserFiles.FirstOrDefaultAsync(f => f.Id == fileId, ct);

    public async Task<IReadOnlyList<UserFile>> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        await db.UserFiles.Where(f => f.UserId == userId).OrderByDescending(f => f.UploadedAt).ToListAsync(ct);

    public Task RemoveAsync(UserFile file, CancellationToken ct)
    {
        db.UserFiles.Remove(file);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
