using Microsoft.Extensions.FileProviders;

namespace OliveMorocco.Web.Photos;

/// <summary>
/// Writes product photos to <c>App_Data/photos</c> by default — deliberately outside
/// wwwroot, so an upload is not swept away by a rebuild, a publish, or a git clean, and
/// so user content never sits in the tree the app ships as static assets. The directory
/// is mounted back into the pipeline at <see cref="UrlPrefix"/> by a second
/// <c>UseStaticFiles</c> call in Program.cs.
/// </summary>
public sealed class PhotoStore : IPhotoStore
{
    /// <summary>5 MB. Generous for a product photo, small enough to bound disk use.</summary>
    public const long DefaultMaxBytes = 5 * 1024 * 1024;

    private const string DefaultRoot = "App_Data/photos";
    private const int SignatureLength = 12;

    private readonly ILogger<PhotoStore> _logger;

    public PhotoStore(IConfiguration configuration, IHostEnvironment environment, ILogger<PhotoStore> logger)
    {
        _logger = logger;

        var configured = configuration["Photos:Root"];
        var path = string.IsNullOrWhiteSpace(configured) ? DefaultRoot : configured.Trim();

        Root = Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, path));

        var maxBytes = configuration.GetValue<long?>("Photos:MaxBytes");
        MaxBytes = maxBytes is > 0 ? maxBytes.Value : DefaultMaxBytes;

        // The physical file provider throws if the directory is missing, and the app
        // should start on a clean clone with no uploads yet.
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public string UrlPrefix { get; } = "/photos";

    public long MaxBytes { get; }

    public async Task<PhotoSaveResult> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            return PhotoSaveResult.Fail("Aucun fichier reçu.");

        if (file.Length > MaxBytes)
        {
            return PhotoSaveResult.Fail(
                $"L'image dépasse la taille maximale de {MaxBytes / 1024 / 1024} Mo.");
        }

        // The file is identified by its magic bytes, not by its name. Trusting
        // ContentType or the client filename would let a .html or .svg renamed to .jpg
        // through, and this directory is served from our own origin — that is a stored
        // XSS hole, not a cosmetic bug. SVG is refused for the same reason it is not in
        // the table below: it is XML and can carry script.
        var signature = new byte[SignatureLength];
        int read;

        await using (var source = file.OpenReadStream())
        {
            read = await source.ReadAsync(signature, cancellationToken);

            var extension = DetectExtension(signature.AsSpan(0, read));
            if (extension is null)
            {
                return PhotoSaveResult.Fail(
                    "Format non reconnu. Formats acceptés : JPEG, PNG, WebP ou GIF.");
            }

            var now = DateTime.Now;
            var year = now.ToString("yyyy");
            var month = now.ToString("MM");

            // Sharded by month so a directory never accumulates more than a few thousand
            // entries, which keeps enumeration cheap.
            var directory = Path.Combine(Root, year, month);
            Directory.CreateDirectory(directory);

            // GUID, never the uploaded name: that removes traversal, collisions and
            // double extensions in one step.
            var name = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(directory, name);

            await using var target = File.Create(fullPath);
            await target.WriteAsync(signature.AsMemory(0, read), cancellationToken);
            await source.CopyToAsync(target, cancellationToken);

            var url = $"{UrlPrefix}/{year}/{month}/{name}";
            _logger.LogInformation("Stored product photo {Url} ({Bytes} bytes)", url, file.Length);

            return PhotoSaveResult.Ok(url);
        }
    }

    public void Delete(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        if (!url.StartsWith(UrlPrefix + "/", StringComparison.OrdinalIgnoreCase))
        {
            // A CDN link or a hand-typed path. Not ours, and deleting it is not ours either.
            return;
        }

        var relative = url[(UrlPrefix.Length + 1)..];

        // The URL came from our own generator, but the column is editable, so re-check
        // that the resolved path really is under the root before touching the disk.
        var rootWithSeparator = Root.EndsWith(Path.DirectorySeparatorChar)
            ? Root
            : Root + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(Path.Combine(Root, relative));

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Refused to delete {Url}: resolves outside the photo root", url);
            return;
        }

        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (IOException exception)
        {
            // A file we cannot remove is an orphan, not a failure — the product row is
            // already gone, so this must not surface as an error to the user.
            _logger.LogWarning(exception, "Could not delete photo {Path}", fullPath);
        }
    }

    private static string? DetectExtension(ReadOnlySpan<byte> head)
    {
        if (head.Length >= 3 && head[0] == 0xFF && head[1] == 0xD8 && head[2] == 0xFF)
            return ".jpg";

        if (head.Length >= 8
            && head[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
        {
            return ".png";
        }

        // RIFF....WEBP — the four size bytes in the middle are ignored.
        if (head.Length >= 12
            && head[..4].SequenceEqual("RIFF"u8)
            && head[8..12].SequenceEqual("WEBP"u8))
        {
            return ".webp";
        }

        if (head.Length >= 4 && head[..4].SequenceEqual("GIF8"u8))
            return ".gif";

        return null;
    }
}
