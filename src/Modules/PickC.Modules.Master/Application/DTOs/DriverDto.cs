namespace PickC.Modules.Master.Application.DTOs;

public class DriverDto
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string LicenseNo { get; set; } = string.Empty;
    public bool Status { get; set; }
    public string? OperatorID { get; set; }
    public bool IsVerified { get; set; }
}

public class DriverDetailDto : DriverDto
{
    public string FatherName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public short Gender { get; set; }
    public short MaritialStatus { get; set; }
    public string? PANNo { get; set; }
    public string? AadharCardNo { get; set; }
    public string? Nationality { get; set; }
    public string? VehicleRCNo { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
    public List<DriverAttachmentDto> Attachments { get; set; } = new();
    public DriverBankDetailsDto? BankDetails { get; set; }
}

public class DriverSaveDto
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public short Gender { get; set; }
    public short MaritialStatus { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string? PANNo { get; set; }
    public string? AadharCardNo { get; set; }
    public string LicenseNo { get; set; } = string.Empty;
    public string? Nationality { get; set; }
    public string? OperatorID { get; set; }
    public string? VehicleRCNo { get; set; }
}

public class DriverUpdatePasswordDto
{
    public string DriverID { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class DriverAttachmentDto
{
    public string AttachmentId { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string? LookupCode { get; set; }
    public string? ImagePath { get; set; }
}

public class DriverBankDetailsDto
{
    public string DriverID { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string IFSC { get; set; } = string.Empty;
    public string? AccountName { get; set; }
}

public class AvailableDriverDto
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public int VehicleGroupID { get; set; }
    public string VehicleGroupName { get; set; } = string.Empty;
    public int VehicleTypeID { get; set; }
    public string VehicleTypeName { get; set; } = string.Empty;
    public string VehicleNumber { get; set; } = string.Empty;
    public decimal CurrentLatitude { get; set; }
    public decimal CurrentLongitude { get; set; }
}
