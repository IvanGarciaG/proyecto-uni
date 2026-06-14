using Files.Application.Commands;
using Files.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Files.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController(IMediator mediator) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Sube un archivo (PNG, JPG o PDF). Máximo 10 MB.</summary>
    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No se recibió ningún archivo." });

        await using var stream = file.OpenReadStream();

        var cmd = new UploadFileCommand(
            CurrentUserId,
            file.FileName,
            file.ContentType,
            file.Length,
            stream);

        var result = await mediator.Send(cmd, ct);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(
            nameof(Download),
            new { fileId = result.Value!.FileId },
            result.Value);
    }

    /// <summary>Lista todos los archivos del usuario autenticado.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyFiles(CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserFilesQuery(CurrentUserId), ct);
        return Ok(result.Value);
    }

    /// <summary>Descarga un archivo por ID.</summary>
    [HttpGet("{fileId:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid fileId, CancellationToken ct)
    {
        var filesResult = await mediator.Send(new GetUserFilesQuery(CurrentUserId), ct);
        var meta = filesResult.Value?.FirstOrDefault(f => f.FileId == fileId);

        if (meta is null)
            return NotFound(new { error = "Archivo no encontrado." });

        // Para LocalStorage: servir directamente el stream
        var storageService = HttpContext.RequestServices
            .GetRequiredService<Files.Application.IFileStorageService>();

        var stream = await storageService.GetAsync(meta.StoragePath, ct);
        return File(stream, meta.ContentType, meta.OriginalName);
    }

    /// <summary>Elimina un archivo del usuario autenticado.</summary>
    [HttpDelete("{fileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid fileId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFileCommand(fileId, CurrentUserId), ct);

        if (!result.IsSuccess)
            return result.Error!.Contains("permiso")
                ? Forbid()
                : NotFound(new { error = result.Error });

        return NoContent();
    }
}
