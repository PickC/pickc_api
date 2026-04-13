namespace PickC.Modules.Master.Domain.Entities;

public class BankDetails
{
    public string OperatorBankID { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string? Branch { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public string? IFSC { get; set; }
}
