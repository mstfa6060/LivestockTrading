using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryCurrencyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("0c51a24b-4024-40f2-912d-2a08e3839f86"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("0f80c0ab-26d2-4ad1-a68c-75fac70c04f9"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f7b4fefb-8e2f-408b-b1c7-65cc9561e8fa"));

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrencyCode",
                table: "Countries",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrencySymbol",
                table: "Countries",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5099), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5105) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5119), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5120) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("31ee0ad6-51ec-4df0-9ee4-e2d0ab7b0d53"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5504), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5505) },
                    { new Guid("5e354a9e-5f1e-4cb0-99fa-02a6b9e1c720"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5469), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5470) },
                    { new Guid("b5ae9829-1c99-42ce-9228-2a1b25aa0feb"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5460), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 23, 20, 9, 58, 122, DateTimeKind.Utc).AddTicks(5462) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("31ee0ad6-51ec-4df0-9ee4-e2d0ab7b0d53"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("5e354a9e-5f1e-4cb0-99fa-02a6b9e1c720"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("b5ae9829-1c99-42ce-9228-2a1b25aa0feb"));

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencySymbol",
                table: "Countries");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(4971), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(4977) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(4990), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(4991) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0c51a24b-4024-40f2-912d-2a08e3839f86"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5317), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5318) },
                    { new Guid("0f80c0ab-26d2-4ad1-a68c-75fac70c04f9"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5339), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5340) },
                    { new Guid("f7b4fefb-8e2f-408b-b1c7-65cc9561e8fa"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5348), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 21, 16, 58, 17, 6, DateTimeKind.Utc).AddTicks(5349) }
                });
        }
    }
}
