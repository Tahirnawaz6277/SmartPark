using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPark.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "User");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "City", "Email", "ImageExtension", "IsDeleted", "Name", "Password", "PhoneNumber", "ProfileImagePath", "RoleId", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "AdminCity", "admin@gmail.com", null, false, "admin", "J8X5lGsz6RVs9GjretabgA==", "1234567890", null, new Guid("11111111-1111-1111-1111-111111111111"), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "User",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: true);
        }
    }
}
