import { extractInvoice } from "../lib/api.js";

const $form  = $("#invoice-form");
const $error = $("#invoice-error");

function show(screen) {
  $("#upload-screen, #result-screen").prop("hidden", true);
  $(screen).prop("hidden", false);
}

$form.on("submit", async function (event) {
  event.preventDefault();
  $error.text("");

  const email = $form.find("[name=email]")[0];
  const pdf   = $form.find("[name=pdf]")[0].files[0];

  if (!email.checkValidity()) return $error.text("Please enter a valid email.");
  if (!pdf)                   return $error.text("Please choose a PDF.");

  const $button = $form.find("button[type=submit]");
  $button.prop("disabled", true).text("Processing…");
  try {
    const data = new FormData();
    data.append("email", email.value.trim());
    data.append("pdf", pdf);
    render(await extractInvoice(data));
    show("#result-screen");
  } catch (err) {
    $error.text(err.message || "Something went wrong.");
  } finally {
    $button.prop("disabled", false).text("Extract & email");
  }
});

$("#back").on("click", () => show("#upload-screen"));

function render(result) {
  const inv  = result.invoice;
  const $out = $("#invoice-result").empty();

  $out.append(
    $("<p>").text(`Supplier: ${inv.supplierName} (${inv.supplierTaxId})`),
    $("<p>").text(`Client: ${inv.clientName} (${inv.clientTaxId})`),
    $("<p>").text(`Total before VAT: ${inv.totalBeforeVat}`),
    $("<p>").text(`Total incl. VAT: ${inv.totalWithVat}`)
  );

  const $tbody = $("<tbody>");
  inv.products.forEach(p =>
    $("<tr>").append($("<td>").text(p.product), $("<td>").text(p.cost)).appendTo($tbody)
  );
  $out.append(
    $("<h3>").text("Products"),
    $("<table>").append(
      $("<thead>").append($("<tr>").append($("<th>").text("Product"), $("<th>").text("Cost"))),
      $tbody
    )
  );

  const dates = inv.dates.map(d => `${d.label}: ${d.value}`).join(", ");
  $out.append($("<p>").text("Dates: " + dates));

  if (result.warnings.length) {
    const $ul = $("<ul>");
    result.warnings.forEach(w => $("<li>").text(w).appendTo($ul));
    $out.append($("<h3>").text("Warnings"), $ul);
  }

  $out.append($("<p>").text("A copy was emailed to the address you entered."));
}
