namespace PickC.Modules.Identity.Application.DTOs;

public class CustomerLoginRequest
{
    public string MobileNo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class DriverLoginRequest
{
    public string DriverId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class DutyStatusRequest
{
    public bool IsOnDuty { get; set; }
}

public class LogoutRequest
{
    public string? DriverId { get; set; }
    public string? UserType { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
