using Files.Application;

namespace Files.Infrastructure.Storage;

/// <summary>Guarda archivos en el sistema de archivos local. Útil para desarrollo.</summary>
public class LocalFileStorageService(FileUploadOptions options) : IFileStorageService
{
    public async Task<string> SaveAsync(Stream content, string storedName, string contentType, CancellationToken ct)
    {
        var dir = Path.GetFullPath(options.LocalBasePath);
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, storedName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, ct);

        return $"/files/{storedName}";  // URL relativa servida por el endpoint de descarga
    }

    public Task DeleteAsync(string storagePath, CancellationToken ct)
    {
        var storedName = Path.GetFileName(storagePath);
        var fullPath = Path.Combine(Path.GetFullPath(options.LocalBasePath), storedName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<Stream> GetAsync(string storagePath, CancellationToken ct)
    {
        var storedName = Path.GetFileName(storagePath);
        var fullPath = Path.Combine(Path.GetFullPath(options.LocalBasePath), storedName);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Archivo no encontrado.", fullPath);

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }
}
