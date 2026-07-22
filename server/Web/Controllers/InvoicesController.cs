using Application.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace Web;

[ApiController]
[Route("api/invoices")]
public class InvoicesController(InvoiceService invoices) : ControllerBase
{
    [HttpPost]
    public async Task<InvoiceResult> Process([FromForm] string email, IFormFile pdf)
    {
        using var ms = new MemoryStream();
        await pdf.CopyToAsync(ms);
        return await invoices.ProcessAsync(ms.ToArray(), pdf.FileName, email);
    }
}
