using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Files.Application;

namespace Files.Infrastructure.Storage;

/// <summary>Guarda archivos en Azure Blob Storage. Usar en producción.</summary>
public class AzureBlobStorageService(BlobServiceClient blobClient, AzureBlobOptions azureOptions)
    : IFileStorageService
{
    public async Task<string> SaveAsync(Stream content, string storedName, string contentType, CancellationToken ct)
    {
        var container = blobClient.GetBlobContainerClient(azureOptions.ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(storedName);
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string storagePath, CancellationToken ct)
    {
        var container = blobClient.GetBlobContainerClient(azureOptions.ContainerName);
        var blobName = new Uri(storagePath).Segments.Last();
        await container.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct);
    }

    public async Task<Stream> GetAsync(string storagePath, CancellationToken ct)
    {
        var container = blobClient.GetBlobContainerClient(azureOptions.ContainerName);
        var blobName = new Uri(storagePath).Segments.Last();
        var blob = container.GetBlobClient(blobName);
        var response = await blob.DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }
}

public class AzureBlobOptions
{
    public const string Section = "AzureBlob";
    public string ConnectionString { get; init; } = string.Empty;
    public string ContainerName { get; init; } = "user-files";
}
