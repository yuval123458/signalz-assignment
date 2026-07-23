using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateUserProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        CREATE OR ALTER PROCEDURE CreateUser
            @Username  NVARCHAR(MAX),
            @Email     NVARCHAR(MAX),
            @BirthDate DATE,
            @ImagePath NVARCHAR(MAX)
        AS
        INSERT INTO Users (Username, Email, BirthDate, ImagePath)
        OUTPUT INSERTED.Id AS Value
        VALUES (@Username, @Email, @BirthDate, @ImagePath);
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP PROCEDURE IF EXISTS CreateUser""");
        }
    }
}
