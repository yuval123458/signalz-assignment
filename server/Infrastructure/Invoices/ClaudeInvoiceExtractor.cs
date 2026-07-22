using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using Application.Invoices;
using Domain;

namespace Infrastructure.Invoices;

public class ClaudeInvoiceExtractor : IInvoiceExtractor
{
    private readonly AnthropicClient _client;

    private const string SystemPrompt = """
        You are an expert at extracting data from tax invoices.
        Extract only what actually appears in the document — never invent or guess a value.
        If a field is genuinely absent, leave it empty rather than fabricating it.
        Invoices are often in Hebrew: ח.פ is a company/tax ID, ספק is the supplier,
        לקוח is the client, מע"מ is VAT (value-added tax).
        Return every date in ISO format (yyyy-MM-dd).
        """;


    public ClaudeInvoiceExtractor(string apiKey) => _client = new() { ApiKey = apiKey };

    public async Task<Invoice> ExtractAsync(byte[] pdf)
    {
        var base64 = Convert.ToBase64String(pdf);

        var response = await _client.Messages.Create(new MessageCreateParams
        {
            Model = Model.ClaudeOpus4_8,
            MaxTokens = 4096,
            System = SystemPrompt,
            OutputConfig = new OutputConfig { Format = new JsonOutputFormat { Schema = Schema } },
            Messages =
            [
                new MessageParam
                {
                    Role = Role.User,
                    Content = new List<ContentBlockParam>
                    {
                        new DocumentBlockParam { Source = new Base64PdfSource { Data = base64 } },
                        new TextBlockParam { Text = "Extract the invoice fields into the required JSON schema." }
                    }
                }
            ]
        });

        var json = response.Content.Select(b => b.Value).OfType<TextBlock>().First().Text;
        return JsonSerializer.Deserialize<Invoice>(json, JsonOpts)!;
    }

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private static readonly Dictionary<string, JsonElement> Schema =
        JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(SchemaJson)!;

    private const string SchemaJson = """
    {
      "type": "object",
      "properties": {
        "supplierName":   { "type": "string" },
        "clientName":     { "type": "string" },
        "supplierTaxId":  { "type": "string" },
        "clientTaxId":    { "type": "string" },
        "products": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": { "product": { "type": "string" }, "cost": { "type": "number" } },
            "required": ["product", "cost"],
            "additionalProperties": false
          }
        },
        "totalBeforeVat": { "type": "number" },
        "totalWithVat":   { "type": "number" },
        "dates": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": { "label": { "type": "string" }, "value": { "type": "string", "format": "date" } },
            "required": ["label", "value"],
            "additionalProperties": false
          }
        }
      },
      "required": ["supplierName","clientName","supplierTaxId","clientTaxId","products","totalBeforeVat","totalWithVat","dates"],
      "additionalProperties": false
    }
    """;
}
