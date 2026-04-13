namespace PickC.SharedKernel.Helpers;

/// <summary>
/// Returns current Indian Standard Time (UTC+5:30) for storing business timestamps in the database.
/// JWT token expiry and security-related timestamps must still use DateTime.UtcNow.
/// </summary>
public static class IstClock
{
    private static readonly TimeZoneInfo Ist =
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Ist);
}
