import { initUserForm } from "./user-form.js";
import { initUserList, loadUsers } from "./user-list.js";
import { getUser } from "../lib/api.js";

function show(screen) {
  $("#form-screen, #list-screen, #detail-screen").hide();
  $(screen).show();
}

function showForm() { show("#form-screen"); }
function showList() { show("#list-screen"); loadUsers(); }

async function onSelect(id) {
  const u = await getUser(id);
  $("#d-username").text(u.username);
  $("#d-email").text(u.email);
  $("#d-birthDate").text(u.birthDate);
  $("#d-image").attr("src", u.imagePath);
  show("#detail-screen");
}

initUserForm({ onSaved: showList });
initUserList({ onSelect });
$("#back-to-list").on("click", showList);

showForm();
