import { UserForm } from "./user-form.js";
import { UserList, loadUsers } from "./user-list.js";
import { getUser } from "../lib/api.js";

function show(screen) {
  $("#form-screen, #list-screen, #detail-screen").prop("hidden", true);
  $(screen).prop("hidden", false);
}

function showForm() { show("#form-screen"); }
function showList() { show("#list-screen"); loadUsers(); }

async function handleUser(id) {
  const u = await getUser(id);
  $("#d-username").text(u.username);
  $("#d-email").text(u.email);
  $("#d-birthDate").text(u.birthDate);
  $("#d-image").attr("src", u.imagePath);
  show("#detail-screen");
}

UserForm(showList);
UserList(handleUser);
$("#back-to-list").on("click", showList);

showForm();
