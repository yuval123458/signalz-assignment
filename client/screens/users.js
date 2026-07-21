import { initUserForm }  from "./user-form.js";
import { initUserList } from "./user-list.js";

function showForm() { $("#form-screen").show(); $("#list-screen").hide(); }
function showList() { $("#form-screen").hide(); $("#list-screen").show(); }

initUserForm({ onSaved: showList });
initUserList({ onAddNew: showForm });

showForm();
