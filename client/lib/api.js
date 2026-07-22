export async function createUser(formData) {
  const res = await fetch("/api/users", { method: "POST", body: formData });
  if (!res.ok) throw new Error("Failed to save user");
  return res.json();
}

export async function getUsers() {
  const res = await fetch("/api/users");
  return res.json();
}

export async function getUser(id) {
  const res = await fetch(`/api/users/${id}`);
  return res.json();
}

export async function extractInvoice(formData) {
  const res = await fetch("/api/invoices", { method: "POST", body: formData });
  if (!res.ok) throw new Error("Failed to process invoice");
  return res.json();
}

