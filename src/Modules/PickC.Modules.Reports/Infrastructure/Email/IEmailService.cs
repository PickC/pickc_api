namespace PickC.Modules.Reports.Infrastructure.Email;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    Task SendWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentBytes, string attachmentFileName, string attachmentMimeType, CancellationToken ct = default);
}
