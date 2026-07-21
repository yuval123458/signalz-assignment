import { compressImage } from "../lib/image.js";
import { createUser } from "../lib/api.js";

export function initUserForm({ onSaved }) {
  
  const $form  = $("#user-form");
  const $error = $("#form-error");

  $form.on("submit", async function (event) {
    event.preventDefault();
    $error.text("");

    const username  = $form.find("[name=username]").val().trim();
    const email     = $form.find("[name=email]").val().trim();
    const birthDate = $form.find("[name=birthDate]").val();
    const file      = $form.find("[name=image]")[0].files[0]; 

    if (!username)                                        return fail("Username is required.");
    if (!$form.find("[name=email]")[0].checkValidity())  return fail("Please enter a valid email address.");
    if (!birthDate)                                      return fail("Birth date is required.");
    if (!file)                                           return fail("Please choose an image.");

    const compressed = await compressImage(file);

    const data = new FormData();
    data.append("username", username);
    data.append("email", email);
    data.append("birthDate", birthDate);
    data.append("image", compressed, "image.jpg");

    await createUser(data);  
    onSaved();      

  });

  function fail(message) { $error.text(message); }
}
