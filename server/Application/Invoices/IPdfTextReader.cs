namespace Application.Invoices;

public interface IPdfTextReader
{
    string Read(byte[] pdf);
}
