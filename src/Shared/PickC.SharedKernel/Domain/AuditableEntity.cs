namespace PickC.SharedKernel.Domain;

public abstract class AuditableEntity : BaseEntity
{
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
