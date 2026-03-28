using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddDistrictLatLngAndLocationDistrictId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Districts",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Districts",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Countries",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Countries",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

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

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DistrictId",
                table: "Locations",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code3",
                table: "Countries",
                column: "Code3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Continent",
                table: "Countries",
                column: "Continent");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_IsActive",
                table: "Countries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Districts_DistrictId",
                table: "Locations",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Districts_DistrictId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_DistrictId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Code",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Code3",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Continent",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_IsActive",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");

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

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Districts");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Countries",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Countries",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

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
        }
    }
}
