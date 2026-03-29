using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class EnrichLocationDataFromAllCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("06b3a525-5e3d-42c6-aec7-f473b3981750"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("34d010cc-a631-42a2-8813-fb753b65b362"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("cd0abc1b-5664-4612-9faa-49e3da855174"));

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Provinces",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Provinces",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Population",
                table: "Provinces",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Provinces",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeoNameId",
                table: "Neighborhoods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Neighborhoods",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Neighborhoods",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Population",
                table: "Districts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Districts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Neighborhoods_GeoNameId",
                table: "Neighborhoods",
                column: "GeoNameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Neighborhoods_GeoNameId",
                table: "Neighborhoods");

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

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Population",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "GeoNameId",
                table: "Neighborhoods");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Neighborhoods");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Neighborhoods");

            migrationBuilder.DropColumn(
                name: "Population",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Districts");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 18, 46, 23, 540, DateTimeKind.Utc).AddTicks(4210), new DateTime(2026, 3, 28, 18, 46, 23, 540, DateTimeKind.Utc).AddTicks(4467) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 18, 46, 23, 540, DateTimeKind.Utc).AddTicks(4875), new DateTime(2026, 3, 28, 18, 46, 23, 540, DateTimeKind.Utc).AddTicks(4875) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("06b3a525-5e3d-42c6-aec7-f473b3981750"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2559), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2564) },
                    { new Guid("34d010cc-a631-42a2-8813-fb753b65b362"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2574), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2574) },
                    { new Guid("cd0abc1b-5664-4612-9faa-49e3da855174"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2570), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 28, 18, 46, 23, 541, DateTimeKind.Utc).AddTicks(2570) }
                });
        }
    }
}
