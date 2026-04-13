namespace PickC.Modules.Master.Application.DTOs;

public class AddressDto
{
    public int AddressId { get; set; }
    public string AddressLinkID { get; set; } = string.Empty;
    public short SeqNo { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string? CityName { get; set; }
    public string? StateName { get; set; }
    public string? ZipCode { get; set; }
    public string? MobileNo { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }
    public string? FullAddress { get; set; }
}

public class AddressSaveDto
{
    public int AddressId { get; set; }
    public string AddressLinkID { get; set; } = string.Empty;
    public short SeqNo { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? CityName { get; set; }
    public string? StateName { get; set; }
    public string? CountryCode { get; set; }
    public string? ZipCode { get; set; }
    public string? TelNo { get; set; }
    public string? MobileNo { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }
}
