namespace PickC.Modules.Master.Application.DTOs;

public class CustomerDto
{
    public string MobileNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmailID { get; set; } = string.Empty;
    public string DeviceID { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

public class CustomerSaveDto
{
    public string MobileNo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmailID { get; set; } = string.Empty;
    public string DeviceID { get; set; } = string.Empty;
}

public class CustomerUpdateDeviceDto
{
    public string MobileNo { get; set; } = string.Empty;
    public string DeviceID { get; set; } = string.Empty;
}

public class CustomerUpdatePasswordDto
{
    public string MobileNo { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
