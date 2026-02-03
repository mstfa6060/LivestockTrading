using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddProductMediaFieldsAndSlugConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("88e237a5-c613-4e01-a842-8b419473aeaa"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("e93b738c-86da-4263-957b-375458bca29c"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("ff0b9318-98b0-4da7-bf5f-ddfded99eb8c"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4197), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4201) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4208), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4208) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("63fbd10f-e4b9-41aa-b896-e66253002643"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4393), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4394) },
                    { new Guid("8c84b9ff-2b12-43ed-9f55-dd6a545e0b1f"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4414), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4414) },
                    { new Guid("9b187dae-42c2-4fcd-92b5-ee557bf6f664"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4397), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 2, 3, 20, 59, 47, 163, DateTimeKind.Utc).AddTicks(4398) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("63fbd10f-e4b9-41aa-b896-e66253002643"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("8c84b9ff-2b12-43ed-9f55-dd6a545e0b1f"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("9b187dae-42c2-4fcd-92b5-ee557bf6f664"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(687), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(692) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(700), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(700) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("88e237a5-c613-4e01-a842-8b419473aeaa"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(866), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(867) },
                    { new Guid("e93b738c-86da-4263-957b-375458bca29c"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(873), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(874) },
                    { new Guid("ff0b9318-98b0-4da7-bf5f-ddfded99eb8c"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(870), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 2, 3, 20, 52, 35, 867, DateTimeKind.Utc).AddTicks(871) }
                });
        }
    }
}
