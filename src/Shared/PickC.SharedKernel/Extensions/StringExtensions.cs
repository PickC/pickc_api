namespace PickC.SharedKernel.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    public static string ToSafeString(this string? value)
        => value ?? string.Empty;

    public static string BuildFullAddress(params string?[] parts)
        => string.Join(", ", parts.Where(s => !string.IsNullOrWhiteSpace(s)));
}
