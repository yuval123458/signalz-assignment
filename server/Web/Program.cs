using Application.Users;
using Infrastructure;
using Infrastructure.Users;
using Application.Invoices;
using Infrastructure.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// DI registration - task 1
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IImageStorage>(new DiskImageStorage(
    Path.Combine(builder.Environment.ContentRootPath, "uploads")));
builder.Services.AddScoped<UserService>();

// DI registration - task 2

builder.Services.AddScoped<IPdfTextReader, PdfPigTextReader>();
builder.Services.AddScoped<IInvoiceExtractor>(_ =>
    new ClaudeInvoiceExtractor(builder.Configuration["Anthropic:ApiKey"]!));
builder.Services.AddScoped<IEmailSender>(_ =>
    new SmtpEmailSender(builder.Configuration["Email:From"]!,
                        builder.Configuration["Email:AppPassword"]!));
builder.Services.AddScoped<InvoiceService>();


var app = builder.Build();

app.UseExceptionHandler();

app.MapControllers();

// static frontend
var clientPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "client"));
var clientFiles = new PhysicalFileProvider(clientPath);
app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = clientFiles });
app.UseStaticFiles(new StaticFileOptions { FileProvider = clientFiles });

// serve saved images at /uploads
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.Run();
