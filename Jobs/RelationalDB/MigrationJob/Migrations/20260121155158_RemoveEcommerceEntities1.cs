using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEcommerceEntities1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2ffb43cd-c09b-413c-9ae3-ca0a6f9b25de"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("69f8a154-db0a-4780-a1d3-b93a129e1bb8"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("d25a4b5d-f209-41cd-8bb7-477b4645bbba"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5237), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5247) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5265), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5266) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2ffb43cd-c09b-413c-9ae3-ca0a6f9b25de"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5799), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5801) },
                    { new Guid("69f8a154-db0a-4780-a1d3-b93a129e1bb8"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5746), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5747) },
                    { new Guid("d25a4b5d-f209-41cd-8bb7-477b4645bbba"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5757), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 1, 21, 15, 41, 16, 499, DateTimeKind.Utc).AddTicks(5758) }
                });
        }
    }
}
