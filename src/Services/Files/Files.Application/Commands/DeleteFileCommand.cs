using MediatR;
using Shared.Kernel;

namespace Files.Application.Commands;

public record DeleteFileCommand(Guid FileId, Guid RequestingUserId) : IRequest<Result>;

public class DeleteFileCommandHandler(IUserFileRepository repository, IFileStorageService storage)
    : IRequestHandler<DeleteFileCommand, Result>
{
    public async Task<Result> Handle(DeleteFileCommand cmd, CancellationToken ct)
    {
        var file = await repository.GetByIdAsync(cmd.FileId, ct);
        if (file is null)
            return Result.Failure("Archivo no encontrado.");

        // Solo el dueño puede eliminar
        if (file.UserId != cmd.RequestingUserId)
            return Result.Failure("No tienes permiso para eliminar este archivo.");

        await storage.DeleteAsync(file.StoragePath, ct);
        await repository.RemoveAsync(file, ct);
        await repository.SaveChangesAsync(ct);

        return Result.Success();
    }
}
