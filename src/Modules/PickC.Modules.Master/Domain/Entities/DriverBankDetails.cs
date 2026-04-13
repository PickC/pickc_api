namespace PickC.Modules.Master.Domain.Entities;

public class DriverBankDetails
{
    public string DriverID { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string IFSC { get; set; } = string.Empty;
    public string? AccountName { get; set; }
}
