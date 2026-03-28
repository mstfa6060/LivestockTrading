using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryIdToProvinceAndTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Provinces_Code",
                table: "Provinces");

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

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Provinces",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Provinces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GeoNameId",
                table: "Provinces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NameTranslations",
                table: "Provinces",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeoNameId",
                table: "Districts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NameTranslations",
                table: "Districts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 52, 119, DateTimeKind.Utc).AddTicks(9438), new DateTime(2026, 3, 28, 15, 12, 52, 119, DateTimeKind.Utc).AddTicks(9639) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 52, 119, DateTimeKind.Utc).AddTicks(9939), new DateTime(2026, 3, 28, 15, 12, 52, 119, DateTimeKind.Utc).AddTicks(9940) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("39008b97-8a35-44e3-b1f9-65188430c2db"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5180), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5180) },
                    { new Guid("891c45d2-b776-4399-b0e6-83d8e688ec42"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5160), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5161) },
                    { new Guid("d3f7fa0d-2078-40c8-8b30-c5abdd185951"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5183), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 28, 15, 12, 52, 120, DateTimeKind.Utc).AddTicks(5184) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_CountryId",
                table: "Provinces",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_CountryId_Code",
                table: "Provinces",
                columns: new[] { "CountryId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_GeoNameId",
                table: "Provinces",
                column: "GeoNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_GeoNameId",
                table: "Districts",
                column: "GeoNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Provinces_Countries_CountryId",
                table: "Provinces",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provinces_Countries_CountryId",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Provinces_CountryId",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Provinces_CountryId_Code",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Provinces_GeoNameId",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Districts_GeoNameId",
                table: "Districts");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("39008b97-8a35-44e3-b1f9-65188430c2db"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("891c45d2-b776-4399-b0e6-83d8e688ec42"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("d3f7fa0d-2078-40c8-8b30-c5abdd185951"));

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "GeoNameId",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "NameTranslations",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "GeoNameId",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "NameTranslations",
                table: "Districts");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Provinces",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

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

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Code",
                table: "Provinces",
                column: "Code",
                unique: true);
        }
    }
}
