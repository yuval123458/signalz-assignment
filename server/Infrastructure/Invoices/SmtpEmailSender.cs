using Application.Invoices;
using Domain;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure.Invoices;

public class SmtpEmailSender : IEmailSender
{
    private readonly string _fromEmail;
    private readonly string _appPassword;

    public SmtpEmailSender(string fromEmail, string appPassword)
    {
        _fromEmail = fromEmail;
        _appPassword = appPassword;
    }

    public async Task SendAsync(byte[] pdf, string fileName, Invoice invoice,
                                List<string> warnings, string recipientEmail)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_fromEmail));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = $"Invoice from {invoice.SupplierName}";

        var body = new BodyBuilder { TextBody = BuildBody(invoice, warnings) };
        body.Attachments.Add(fileName, pdf, ContentType.Parse("application/pdf"));  // attach the original PDF
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_fromEmail, _appPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static string BuildBody(Invoice invoice, List<string> warnings)
    {
        var lines = string.Join("\n", invoice.Products.Select(p => $"  {p.Product}: {p.Cost:0.00}"));
        var dates = string.Join("\n", invoice.Dates.Select(d => $"  {d.Label}: {d.Value:yyyy-MM-dd}"));
        var warn = warnings.Count == 0 ? "  none" : string.Join("\n", warnings.Select(w => "  - " + w));

        return $"""
            Supplier: {invoice.SupplierName} (tax ID {invoice.SupplierTaxId})
            Client:   {invoice.ClientName} (tax ID {invoice.ClientTaxId})

            Products:
            {lines}

            Total before VAT: {invoice.TotalBeforeVat:0.00}
            Total incl. VAT:  {invoice.TotalWithVat:0.00}

            Dates:
            {dates}

            Validation warnings:
            {warn}
            """;
    }
}
