using Files.Domain.Entities;

namespace Files.Application;

public interface IUserFileRepository
{
    Task AddAsync(UserFile file, CancellationToken ct);
    Task<UserFile?> GetByIdAsync(Guid fileId, CancellationToken ct);
    Task<IReadOnlyList<UserFile>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task RemoveAsync(UserFile file, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
