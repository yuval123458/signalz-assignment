using Application.Invoices;
using UglyToad.PdfPig;

namespace Infrastructure.Invoices;

public class PdfPigTextReader : IPdfTextReader
{
    public string Read(byte[] pdf)
    {
        using var document = PdfDocument.Open(pdf);
        return string.Join("\n", document.GetPages().Select(page => page.Text));
    }
}
