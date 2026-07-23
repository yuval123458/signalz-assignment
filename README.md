# Signalz assignment

Two full-stack tasks:

- **Task 1** – a user form (name, email, birthdate, photo). on submit the image gets compressed in the browser, uploaded, and the user is saved to SQL Server. After that the form turns into the users list, and clicking a row shows that user with their photo.
- **Task 2** – upload a PDF invoice, an LLM pulls out the fields (supplier, client, tax IDs, line items, totals, dates), and it emails the file + the extracted data.

Backend is .NET 10 with a Clean Architecture layout, frontend is JS + jQuery, and the backend also serves the frontend.

## Set up

**SQL Server.** I'm on a Mac, so I used Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=password" -p 1433:1433 --name signalz-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

**Secrets.** I didn't want the connection string, the API key or the email password sitting in a public repo, so they go in .NET user secrets instead
and sent them to you in the submission email.

```bash
cd server/Web
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=users;User Id=sa;Password=password;TrustServerCertificate=True"
dotnet user-secrets set "Anthropic:ApiKey" "ApiKey"
dotnet user-secrets set "Email:From" "yourEmail@gmail.com"
dotnet user-secrets set "Email:AppPassword" "your gmail app password"
```

**Create the DB.** This runs the migrations — the users table and the `CreateUser` stored procedure:

```bash
dotnet ef database update --project ../Infrastructure
```

**Run:**

```bash
dotnet run
```

Task 1 is at http://localhost:5093, task 2 at http://localhost:5093/invoice.html (theres a link between them).

## Some of the choices I made

- **Clean Architecture** – Domain / Application / Infrastructure / Web. The Application layer only knows interfaces; the DB, file storage, the LLM and the email are all implementations in Infrastructure. Kept things separated and easy to swap.
- **EF Core for reads, a stored procedure for the user insert.** Wanted to show both LINQ for the queries, and a `CreateUser` procedure (added through a migration) for the write. Everything's parameterized either way, so no SQL injection.
- **Image compression happens in the browser** before upload — keeps the heavy work off the server.
- **Task 2 sends the PDF straight to the model.** It reads Hebrew invoices fine, and even scanned ones since it sees the actual page. I use Pdf Pig only as a second opinion.
- **Single origin** – the backend serves the static frontend, so no CORS and the images just load from `/uploads`.

## If I had more time

Add tests, the Application layer is easy to unit-test.
Better handling for scanned invoices and auth.
