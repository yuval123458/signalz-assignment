import { getUsers } from "../lib/api.js";

export function initUserList({ onAddNew, onSelect }) {

  // one delegated handler for all rows
  $("#users-body").on("click", "tr", function () {
    onSelect($(this).data("id"));
  });
}

export async function loadUsers() {
  const users = await getUsers();
  const $body = $("#users-body").empty();

  users.forEach(function (u) {
    $("<tr>")
      .attr("data-id", u.id)           
      .append(
        $("<td>").text(u.username),
        $("<td>").text(u.email),
        $("<td>").text(u.birthDate)
      )
      .appendTo($body);
  });
}
