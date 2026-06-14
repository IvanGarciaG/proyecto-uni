using Files.Domain.Enums;
using Shared.Kernel;

namespace Files.Domain.Entities;

public class UserFile : Entity
{
    public Guid UserId { get; private set; }
    public string OriginalName { get; private set; } = string.Empty;
    public string StoredName { get; private set; } = string.Empty;  // UUID + extension
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public FileType FileType { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;  // ruta o URL
    public DateTime UploadedAt { get; private set; }

    private UserFile() { }

    public static UserFile Create(
        Guid userId,
        string originalName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        var ext = Path.GetExtension(originalName).ToLowerInvariant();
        var storedName = $"{Guid.NewGuid()}{ext}";

        return new UserFile
        {
            UserId = userId,
            OriginalName = originalName,
            StoredName = storedName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            FileType = ResolveType(contentType),
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow
        };
    }

    private static FileType ResolveType(string contentType) =>
        contentType switch
        {
            "application/pdf" => FileType.Document,
            _ => FileType.Image
        };
}
