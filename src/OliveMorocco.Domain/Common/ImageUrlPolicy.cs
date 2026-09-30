namespace OliveMorocco.Domain.Common;

/// <summary>
/// What counts as a usable <c>ImageUrl</c>. Lives in Domain because two layers need the
/// same answer: DataAccess sizes the column to <see cref="MaxLength"/>, and Business
/// validates what users type into the product form.
/// <para>
/// The scheme check is not decoration. The value is rendered into an <c>img src</c> on
/// the public shop, so without pinning the scheme a stored <c>javascript:</c> URL would be
/// a script-injection vector. <c>data:</c> is refused for the same reason plus the fact
/// that it would let someone park an unbounded string in a varchar.
/// </para>
/// </summary>
public static class ImageUrlPolicy
{
    /// <summary>Column width, and the matching validation limit.</summary>
    public const int MaxLength = 500;

    public const string MaxLengthMessage =
        "L'URL de l'image ne doit pas dépasser 500 caractères.";

    public const string SchemeMessage =
        "L'URL de l'image doit commencer par http:// ou https://, ou être un chemin commençant par /.";

    /// <summary>
    /// True for an http(s) URL or a root-relative path. A relative path is allowed
    /// because a product photo may sit under wwwroot rather than on a CDN, and
    /// <c>Url.Content</c> resolves either form at render time.
    /// </summary>
    public static bool IsWebUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();

        if (trimmed[0] == '/')
            return true;

        return Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
