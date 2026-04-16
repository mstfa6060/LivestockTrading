using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddAppVersionConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("4f7e24fa-2773-41df-93f6-e169f4a2aee8"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("99c06299-0b44-4dc0-b008-5937919862c6"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("9f6eb95f-d17a-4289-a8b3-021bea718130"));

            migrationBuilder.CreateTable(
                name: "AppVersionConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    MinSupportedVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LatestVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StoreUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdateMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersionConfigs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 14, 14, 15, 6, 240, DateTimeKind.Utc).AddTicks(5332), new DateTime(2026, 4, 14, 14, 15, 6, 240, DateTimeKind.Utc).AddTicks(5976) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 14, 14, 15, 6, 240, DateTimeKind.Utc).AddTicks(6627), new DateTime(2026, 4, 14, 14, 15, 6, 240, DateTimeKind.Utc).AddTicks(6628) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0f4da6f4-bee6-45b0-894d-e80bc09cf858"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5910), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5911) },
                    { new Guid("52d09aa6-6c73-4460-a78e-813637c04793"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5926), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5926) },
                    { new Guid("d6aa8ac3-5e34-4712-9961-9d01a0c427bc"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5930), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 4, 14, 14, 15, 6, 241, DateTimeKind.Utc).AddTicks(5930) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVersionConfigs_Platform",
                table: "AppVersionConfigs",
                column: "Platform");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersionConfigs");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("0f4da6f4-bee6-45b0-894d-e80bc09cf858"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("52d09aa6-6c73-4460-a78e-813637c04793"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("d6aa8ac3-5e34-4712-9961-9d01a0c427bc"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 0, 33, 3, 953, DateTimeKind.Utc).AddTicks(6213), new DateTime(2026, 3, 29, 0, 33, 3, 953, DateTimeKind.Utc).AddTicks(6443) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 29, 0, 33, 3, 953, DateTimeKind.Utc).AddTicks(6790), new DateTime(2026, 3, 29, 0, 33, 3, 953, DateTimeKind.Utc).AddTicks(6791) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4f7e24fa-2773-41df-93f6-e169f4a2aee8"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2753), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2753) },
                    { new Guid("99c06299-0b44-4dc0-b008-5937919862c6"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2749), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2750) },
                    { new Guid("9f6eb95f-d17a-4289-a8b3-021bea718130"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2736), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 29, 0, 33, 3, 954, DateTimeKind.Utc).AddTicks(2740) }
                });
        }
    }
}
