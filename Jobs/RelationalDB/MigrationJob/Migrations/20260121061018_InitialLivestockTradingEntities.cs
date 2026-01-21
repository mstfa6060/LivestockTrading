using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class InitialLivestockTradingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("3c4d61be-465a-4df3-9b1f-33c153ca933f"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("c71c27f5-6784-45ee-834f-d2e7fa565f5e"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("fc5b3784-dbb2-4828-b455-fabc539efe71"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4136), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4139) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4150), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4151) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44af24e2-6895-4236-8b27-dce78f3efdf4"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4413), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4414) },
                    { new Guid("8a702f6a-2e39-4061-a94f-3ebbab6a9c20"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4397), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4398) },
                    { new Guid("f5fb3a7f-9b80-4e6b-b6cd-55ea3cc10bdc"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4407), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 6, 10, 17, 170, DateTimeKind.Utc).AddTicks(4408) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("44af24e2-6895-4236-8b27-dce78f3efdf4"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("8a702f6a-2e39-4061-a94f-3ebbab6a9c20"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("f5fb3a7f-9b80-4e6b-b6cd-55ea3cc10bdc"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2116), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2122) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2132), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2133) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3c4d61be-465a-4df3-9b1f-33c153ca933f"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2376), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2377) },
                    { new Guid("c71c27f5-6784-45ee-834f-d2e7fa565f5e"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2369), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2370) },
                    { new Guid("fc5b3784-dbb2-4828-b455-fabc539efe71"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2418), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 20, 20, 34, 49, 543, DateTimeKind.Utc).AddTicks(2419) }
                });
        }
    }
}
