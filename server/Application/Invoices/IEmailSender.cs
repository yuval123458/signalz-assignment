using Domain;

namespace Application.Invoices;

public interface IEmailSender
{
    Task SendAsync(byte[] pdf, string fileName, Invoice invoice,
                   List<string> warnings, string recipientEmail);
}
