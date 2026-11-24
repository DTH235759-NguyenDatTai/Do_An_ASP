namespace DoAnASP.Helpers;

public static class ImageUrlHelper
{
    public static string GetUrl(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return "https://via.placeholder.com/300x200?text=No+Image";
        }

        imagePath = imagePath.Trim().Replace('\\', '/');

        if (Uri.TryCreate(imagePath, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return imagePath;
        }

        if (imagePath.StartsWith("~/", StringComparison.Ordinal))
        {
            return imagePath[1..];
        }

        if (imagePath.StartsWith("/", StringComparison.Ordinal))
        {
            return imagePath;
        }

        if (imagePath.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
        {
            return $"/{imagePath}";
        }

        return $"/images/{imagePath}";
    }
}
