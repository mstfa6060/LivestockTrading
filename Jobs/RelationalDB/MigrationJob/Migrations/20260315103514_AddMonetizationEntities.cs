using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Jobs.RelationalDB.MigrationJob.Migrations
{
    /// <inheritdoc />
    public partial class AddMonetizationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveSubscriptionId",
                table: "Sellers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoostScore",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeaturedUntil",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BoostPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTranslations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTranslations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AppleProductId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GoogleProductId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BoostType = table.Column<int>(type: "int", nullable: false),
                    BoostScore = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoostPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameTranslations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTranslations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    PriceMonthly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceYearly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AppleProductIdMonthly = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AppleProductIdYearly = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GoogleProductIdMonthly = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GoogleProductIdYearly = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaxActiveListings = table.Column<int>(type: "int", nullable: false),
                    MaxPhotosPerListing = table.Column<int>(type: "int", nullable: false),
                    MonthlyBoostCredits = table.Column<int>(type: "int", nullable: false),
                    HasDetailedAnalytics = table.Column<bool>(type: "bit", nullable: false),
                    HasPrioritySupport = table.Column<bool>(type: "bit", nullable: false),
                    HasFeaturedBadge = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    OriginalTransactionId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    LastValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerSubscriptions_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SellerSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IAPTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductBoostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    StoreTransactionId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Receipt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IAPTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IAPTransactions_SellerSubscriptions_SellerSubscriptionId",
                        column: x => x.SellerSubscriptionId,
                        principalTable: "SellerSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductBoosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoostPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IAPTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BoostScore = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBoosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBoosts_BoostPackages_BoostPackageId",
                        column: x => x.BoostPackageId,
                        principalTable: "BoostPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBoosts_IAPTransactions_IAPTransactionId",
                        column: x => x.IAPTransactionId,
                        principalTable: "IAPTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBoosts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBoosts_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8724), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8727) });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8736), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8736) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Permission", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6a7a5850-49a5-4303-918d-fb393c9a2558"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8960), null, false, "ManageRoles", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8960) },
                    { new Guid("7330612a-1a06-441d-8198-680fa73c619a"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8963), null, false, "ViewListings", new Guid("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8964) },
                    { new Guid("d4b21b41-7b69-4706-9be4-9882390cc41f"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8956), null, false, "ManageUsers", new Guid("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), new DateTime(2026, 3, 15, 10, 35, 13, 546, DateTimeKind.Utc).AddTicks(8957) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_ActiveSubscriptionId",
                table: "Sellers",
                column: "ActiveSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_IAPTransactions_SellerSubscriptionId",
                table: "IAPTransactions",
                column: "SellerSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_IAPTransactions_StoreTransactionId",
                table: "IAPTransactions",
                column: "StoreTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_IAPTransactions_UserId",
                table: "IAPTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoosts_BoostPackageId",
                table: "ProductBoosts",
                column: "BoostPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoosts_ExpiresAt",
                table: "ProductBoosts",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoosts_IAPTransactionId",
                table: "ProductBoosts",
                column: "IAPTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoosts_ProductId",
                table: "ProductBoosts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoosts_SellerId",
                table: "ProductBoosts",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerSubscriptions_SellerId",
                table: "SellerSubscriptions",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerSubscriptions_Status",
                table: "SellerSubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SellerSubscriptions_SubscriptionPlanId",
                table: "SellerSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_SellerSubscriptions_ActiveSubscriptionId",
                table: "Sellers",
                column: "ActiveSubscriptionId",
                principalTable: "SellerSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_SellerSubscriptions_ActiveSubscriptionId",
                table: "Sellers");

            migrationBuilder.DropTable(
                name: "ProductBoosts");

            migrationBuilder.DropTable(
                name: "BoostPackages");

            migrationBuilder.DropTable(
                name: "IAPTransactions");

            migrationBuilder.DropTable(
                name: "SellerSubscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_ActiveSubscriptionId",
                table: "Sellers");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("6a7a5850-49a5-4303-918d-fb393c9a2558"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("7330612a-1a06-441d-8198-680fa73c619a"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("d4b21b41-7b69-4706-9be4-9882390cc41f"));

            migrationBuilder.DropColumn(
                name: "ActiveSubscriptionId",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "BoostScore",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FeaturedUntil",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Products");

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
    }
}
