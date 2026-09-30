namespace OliveMorocco.Web.Photos;

/// <summary>
/// Stores product photos as files on disk and hands back the URL to record in
/// <c>Produits.ImageUrl</c>. Files rather than a bytea column: the shop reads these on
/// every page view, and serving a file is a straight read off the filesystem instead of
/// a heap allocation plus a copy out of the database for each request.
/// </summary>
public interface IPhotoStore
{
    /// <summary>Absolute path of the directory backing <c>/photos</c>.</summary>
    string Root { get; }

    /// <summary>URL prefix this store owns. Anything outside it is not ours to delete.</summary>
    string UrlPrefix { get; }

    /// <summary>Largest accepted upload, in bytes.</summary>
    long MaxBytes { get; }

    /// <summary>
    /// Writes <paramref name="file"/> under <see cref="Root"/> and returns the URL to
    /// store. Never throws for a bad upload — that is a <see cref="PhotoSaveResult.Fail"/>.
    /// </summary>
    Task<PhotoSaveResult> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a previously stored file. A no-op for a URL this store did not write, so
    /// a product pointing at a CDN image keeps working and is never deleted by mistake.
    /// </summary>
    void Delete(string? url);
}
