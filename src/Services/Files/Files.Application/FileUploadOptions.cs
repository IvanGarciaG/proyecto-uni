namespace Files.Application;

public class FileUploadOptions
{
    public const string Section = "FileUpload";

    /// <summary>Tamaño máximo en bytes. Default: 10 MB.</summary>
    public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024;

    public string Provider { get; init; } = "Local";  // "Local" | "AzureBlob"
    public string LocalBasePath { get; init; } = "uploads";
}
