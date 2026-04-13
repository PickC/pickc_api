using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PickC.Modules.Reports.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = BuildMessage(to, subject, htmlBody);
        await SendMessageAsync(message, ct);
    }

    public async Task SendWithAttachmentAsync(string to, string subject, string htmlBody,
        byte[] attachmentBytes, string attachmentFileName, string attachmentMimeType,
        CancellationToken ct = default)
    {
        var builder = new BodyBuilder { HtmlBody = htmlBody };
        builder.Attachments.Add(attachmentFileName, attachmentBytes, ContentType.Parse(attachmentMimeType));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = builder.ToMessageBody();

        await SendMessageAsync(message, ct);
    }

    private MimeMessage BuildMessage(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();
        return message;
    }

    private async Task SendMessageAsync(MimeMessage message, CancellationToken ct)
    {
        using var client = new SmtpClient();
        var socketOptions = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_settings.Host, _settings.Port, socketOptions, ct);
        if (!string.IsNullOrEmpty(_settings.UserName))
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
