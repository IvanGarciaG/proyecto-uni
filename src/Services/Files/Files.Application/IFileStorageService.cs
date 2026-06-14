namespace Files.Application;

public interface IFileStorageService
{
    /// <summary>Persiste el stream y devuelve la ruta/URL pública del archivo.</summary>
    Task<string> SaveAsync(Stream content, string storedName, string contentType, CancellationToken ct);

    Task DeleteAsync(string storagePath, CancellationToken ct);

    /// <summary>Devuelve un stream de lectura para el archivo.</summary>
    Task<Stream> GetAsync(string storagePath, CancellationToken ct);
}
