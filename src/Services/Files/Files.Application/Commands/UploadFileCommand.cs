using Files.Domain.Entities;
using MediatR;
using Shared.Kernel;

namespace Files.Application.Commands;

public record UploadFileCommand(
    Guid UserId,
    string OriginalName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IRequest<Result<UploadFileResult>>;

public record UploadFileResult(Guid FileId, string StoredName, string StoragePath, long SizeBytes);

public class UploadFileCommandHandler(
    IUserFileRepository repository,
    IFileStorageService storage,
    FileUploadOptions options)
    : IRequestHandler<UploadFileCommand, Result<UploadFileResult>>
{
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "application/pdf"
    };

    public async Task<Result<UploadFileResult>> Handle(UploadFileCommand cmd, CancellationToken ct)
    {
        // Validar tipo MIME
        if (!AllowedTypes.Contains(cmd.ContentType))
            return Result<UploadFileResult>.Failure(
                "Tipo de archivo no permitido. Solo se aceptan PNG, JPG y PDF.");

        // Validar tamaño (default 10 MB)
        if (cmd.SizeBytes > options.MaxFileSizeBytes)
            return Result<UploadFileResult>.Failure(
                $"El archivo supera el tamaño máximo permitido de {options.MaxFileSizeBytes / 1_048_576} MB.");

        // Validar magic bytes (no confiar solo en Content-Type del cliente)
        var validationResult = await ValidateMagicBytesAsync(cmd.Content, cmd.ContentType);
        if (!validationResult)
            return Result<UploadFileResult>.Failure("El contenido del archivo no coincide con el tipo declarado.");

        // Generar nombre seguro y persistir
        var ext = ResolveExtension(cmd.ContentType);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var storagePath = await storage.SaveAsync(cmd.Content, storedName, cmd.ContentType, ct);

        var file = UserFile.Create(cmd.UserId, cmd.OriginalName, cmd.ContentType, cmd.SizeBytes, storagePath);

        await repository.AddAsync(file, ct);
        await repository.SaveChangesAsync(ct);

        return Result<UploadFileResult>.Success(
            new UploadFileResult(file.Id, storedName, storagePath, file.SizeBytes));
    }

    private static async Task<bool> ValidateMagicBytesAsync(Stream stream, string contentType)
    {
        var header = new byte[8];
        var read = await stream.ReadAsync(header.AsMemory(0, 8));
        stream.Position = 0;

        if (read < 4) return false;

        return contentType switch
        {
            "image/png"       => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
            "image/jpeg"      => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            "application/pdf" => header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46,
            _                 => false
        };
    }

    private static string ResolveExtension(string contentType) =>
        contentType switch
        {
            "image/png"       => ".png",
            "image/jpeg"      => ".jpg",
            "application/pdf" => ".pdf",
            _                 => string.Empty
        };
}
