using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("59934992-8de8-45ca-b436-51f3f23e8af5"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("eab930cc-2cab-4018-99ea-87517c66ad26"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("fce72fec-9532-4edd-bf96-1cf0810f6569"));

            migrationBuilder.AddColumn<int>(
                name: "LastViewingCountryId",
                table: "AppUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrencyCode",
                table: "AppUsers",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_LastViewingCountryId",
                table: "AppUsers",
                column: "LastViewingCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Countries_LastViewingCountryId",
                table: "AppUsers",
                column: "LastViewingCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Countries_LastViewingCountryId",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_LastViewingCountryId",
                table: "AppUsers");

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

            migrationBuilder.DropColumn(
                name: "LastViewingCountryId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PreferredCurrencyCode",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6036), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6045) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6059), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6060) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("59934992-8de8-45ca-b436-51f3f23e8af5"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6451), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6452) },
                    { new Guid("eab930cc-2cab-4018-99ea-87517c66ad26"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6458), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6459) },
                    { new Guid("fce72fec-9532-4edd-bf96-1cf0810f6569"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6443), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 15, 51, 56, 92, DateTimeKind.Utc).AddTicks(6445) }
                });
        }
    }
}
