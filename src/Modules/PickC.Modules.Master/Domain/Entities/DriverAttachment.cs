namespace PickC.Modules.Master.Domain.Entities;

public class DriverAttachment
{
    public string AttachmentId { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string? LookupCode { get; set; }
    public string? ImagePath { get; set; }
}
