using Domain;

namespace Application.Invoices;

public class InvoiceService
{
    private readonly IInvoiceExtractor _extractor;
    private readonly IPdfTextReader _pdfText;
    private readonly IEmailSender _email;

    public InvoiceService(IInvoiceExtractor extractor, IPdfTextReader pdfText, IEmailSender email)
    {
        _extractor = extractor;
        _pdfText = pdfText;
        _email = email;
    }

    public async Task<InvoiceResult> ProcessAsync(byte[] pdf, string fileName, string recipientEmail)
    {
        var rawText = _pdfText.Read(pdf);
        var invoice = await _extractor.ExtractAsync(pdf);
        var warnings = Validate(invoice, rawText);
        await _email.SendAsync(pdf, fileName, invoice, warnings, recipientEmail);
        return new InvoiceResult(invoice, warnings);
    }

    private static List<string> Validate(Invoice invoice, string rawText)
    {
        var warnings = new List<string>();

        // math
        var lineSum = invoice.Products.Sum(p => p.Cost);
        if (Math.Abs(lineSum - invoice.TotalBeforeVat) > 0.01m)
            warnings.Add($"Products sum to {lineSum:0.00} but total before VAT is {invoice.TotalBeforeVat:0.00}.");
        if (invoice.TotalWithVat < invoice.TotalBeforeVat)
            warnings.Add("Total incl. VAT is lower than total before VAT.");

        var digitsInText = Digits(rawText);
        if (digitsInText.Length < 5)
        {
            warnings.Add("Scanned or unreadable PDF text — grounding skipped.");
        }
        else
        {
            CheckDigits(warnings, digitsInText, "Supplier tax ID", invoice.SupplierTaxId);
            CheckDigits(warnings, digitsInText, "Client tax ID", invoice.ClientTaxId);
        }

        return warnings;
    }

    private static void CheckDigits(List<string> warnings, string digitsInText, string field, string value)
    {
        var digits = Digits(value);
        if (digits.Length > 0 && !digitsInText.Contains(digits))
            warnings.Add($"{field} ({value}) could not be verified against the document.");
    }

    private static string Digits(string s) => new string(s.Where(char.IsDigit).ToArray());
}

public record InvoiceResult(Invoice Invoice, List<string> Warnings);
