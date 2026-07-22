using Domain;

namespace Application.Invoices;

public interface IInvoiceExtractor
{
    Task<Invoice> ExtractAsync(byte[] pdf);   // Claude reads the PDF → structured Invoice
}
