using System;

namespace Application.Common;

public static class StorageFileName
{
    public static string Create(Guid id, string propertyName, string extension)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension cannot be null or empty.", nameof(extension));

        return $"{id}-{propertyName.ToLower()}.{extension}";
    }
}
