import { getUsers } from "../lib/api.js";

export function UserList(handleUser) {

  $("#users-body").on("click", "tr", function () {
    handleUser($(this).data("id"));
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
