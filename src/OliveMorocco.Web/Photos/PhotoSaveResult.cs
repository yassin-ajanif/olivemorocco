namespace OliveMorocco.Web.Photos;

/// <summary>
/// Outcome of an upload. The only two failure modes are a rejected file or a rejected
/// size, and both are worth explaining to the user, so the message is carried rather
/// than thrown.
/// </summary>
public sealed record PhotoSaveResult(bool Succeeded, string? Url, string? Erreur)
{
    public static PhotoSaveResult Ok(string url) => new(true, url, null);

    public static PhotoSaveResult Fail(string erreur) => new(false, null, erreur);
}
