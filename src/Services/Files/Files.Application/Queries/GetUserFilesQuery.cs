using MediatR;
using Shared.Kernel;

namespace Files.Application.Queries;

public record GetUserFilesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserFileDto>>>;

public record UserFileDto(
    Guid FileId,
    string OriginalName,
    string ContentType,
    long SizeBytes,
    string StoragePath,
    DateTime UploadedAt);

public class GetUserFilesQueryHandler(IUserFileRepository repository)
    : IRequestHandler<GetUserFilesQuery, Result<IReadOnlyList<UserFileDto>>>
{
    public async Task<Result<IReadOnlyList<UserFileDto>>> Handle(GetUserFilesQuery query, CancellationToken ct)
    {
        var files = await repository.GetByUserIdAsync(query.UserId, ct);
        var dtos = files.Select(f =>
            new UserFileDto(f.Id, f.OriginalName, f.ContentType, f.SizeBytes, f.StoragePath, f.UploadedAt))
            .ToList();

        return Result<IReadOnlyList<UserFileDto>>.Success(dtos);
    }
}
