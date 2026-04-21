using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddTransporterStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("b9b10da9-1efd-4e38-81bd-8ccb1f9cff58"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("e23110bd-d428-49ab-b2bb-dda4e2f6c768"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("eee86aad-bcb3-447b-b090-e706a34693a3"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(1304), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(1521) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(1831), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(1832) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0b853de8-1de5-4f2e-b69e-09630f236ea9"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7363), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7364) },
                    { new Guid("4601b7b6-c171-4726-9997-f7e8220bac93"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7336), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7338) },
                    { new Guid("ab7fa489-3b7a-4455-b20c-b5448c0da554"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7360), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 19, 1, 43, 28, 47, DateTimeKind.Utc).AddTicks(7360) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("0b853de8-1de5-4f2e-b69e-09630f236ea9"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("4601b7b6-c171-4726-9997-f7e8220bac93"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("ab7fa489-3b7a-4455-b20c-b5448c0da554"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 19, 0, 47, 46, 167, DateTimeKind.Utc).AddTicks(4641), new DateTime(2026, 3, 19, 0, 47, 46, 167, DateTimeKind.Utc).AddTicks(5145) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 19, 0, 47, 46, 167, DateTimeKind.Utc).AddTicks(5930), new DateTime(2026, 3, 19, 0, 47, 46, 167, DateTimeKind.Utc).AddTicks(5930) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b9b10da9-1efd-4e38-81bd-8ccb1f9cff58"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2031), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2037) },
                    { new Guid("e23110bd-d428-49ab-b2bb-dda4e2f6c768"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2082), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2082) },
                    { new Guid("eee86aad-bcb3-447b-b090-e706a34693a3"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2075), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 19, 0, 47, 46, 169, DateTimeKind.Utc).AddTicks(2075) }
                });
        }
    }
}
