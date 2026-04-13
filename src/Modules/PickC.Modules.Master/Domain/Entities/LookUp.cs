namespace PickC.Modules.Master.Domain.Entities;

public class LookUp
{
    public short LookupID { get; set; }
    public string LookupCode { get; set; } = string.Empty;
    public string LookupDescription { get; set; } = string.Empty;
    public string LookupCategory { get; set; } = string.Empty;
    public string? Image { get; set; }
}
