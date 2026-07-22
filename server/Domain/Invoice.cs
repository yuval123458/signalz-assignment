namespace Domain;

public record Invoice(
    string SupplierName,
    string ClientName,
    string SupplierTaxId,
    string ClientTaxId,
    List<InvoiceLine> Products,
    decimal TotalBeforeVat,
    decimal TotalWithVat,
    List<InvoiceDate> Dates);

public record InvoiceLine(string Product, decimal Cost);

public record InvoiceDate(string Label, DateOnly Value);