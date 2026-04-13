namespace PickC.Modules.Master.Domain.Entities;

public class Address
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
    public string? FaxNo { get; set; }
    public string? MobileNo { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }
    public string? WebSite { get; set; }
    public bool? IsBilling { get; set; }
    public bool? IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // Computed
    public string FullAddress =>
        string.Join(", ", new[] { Address1, Address2, CityName, StateName, ZipCode }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}
