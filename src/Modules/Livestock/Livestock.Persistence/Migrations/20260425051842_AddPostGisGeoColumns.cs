using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Livestock.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostGisGeoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostGIS extension must be created OUTSIDE a transaction on most
            // managed Postgres providers; the explicit Sql call with
            // suppressTransaction:true guarantees that. The AlterDatabase()
            // annotation that follows keeps EF's model snapshot in sync — both
            // emit CREATE EXTENSION IF NOT EXISTS so the second is a no-op.
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS postgis;", suppressTransaction: true);

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Geo",
                table: "TransportTrackings",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "DeliveryGeo",
                table: "TransportRequests",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "PickupGeo",
                table: "TransportRequests",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Geo",
                table: "Sellers",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Geo",
                table: "Locations",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transport_trackings_geo",
                table: "TransportTrackings",
                column: "Geo")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_transport_requests_delivery_geo",
                table: "TransportRequests",
                column: "DeliveryGeo")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_transport_requests_pickup_geo",
                table: "TransportRequests",
                column: "PickupGeo")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_sellers_geo",
                table: "Sellers",
                column: "Geo")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_locations_geo",
                table: "Locations",
                column: "Geo")
                .Annotation("Npgsql:IndexMethod", "GIST");

            // ── Backfill existing lat/lng pairs into geography(Point, 4326) ──
            // Skip rows with NULL coords, out-of-range values, or the (0,0)
            // sentinel which is almost always "unset" rather than the real Gulf
            // of Guinea. Once /Sellers/Nearby & friends are migrated to
            // ST_DWithin, the legacy lat/lng columns will be dropped two
            // releases later.
            migrationBuilder.Sql(@"
                UPDATE ""Locations""
                SET ""Geo"" = ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)::geography
                WHERE ""Latitude"" IS NOT NULL AND ""Longitude"" IS NOT NULL
                  AND ""Latitude"" BETWEEN -90 AND 90 AND ""Longitude"" BETWEEN -180 AND 180
                  AND NOT (""Latitude"" = 0 AND ""Longitude"" = 0);");

            migrationBuilder.Sql(@"
                UPDATE ""TransportTrackings""
                SET ""Geo"" = ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)::geography
                WHERE ""Latitude"" IS NOT NULL AND ""Longitude"" IS NOT NULL
                  AND ""Latitude"" BETWEEN -90 AND 90 AND ""Longitude"" BETWEEN -180 AND 180
                  AND NOT (""Latitude"" = 0 AND ""Longitude"" = 0);");

            migrationBuilder.Sql(@"
                UPDATE ""TransportRequests""
                SET ""PickupGeo"" = ST_SetSRID(ST_MakePoint(""PickupLongitude"", ""PickupLatitude""), 4326)::geography
                WHERE ""PickupLatitude"" IS NOT NULL AND ""PickupLongitude"" IS NOT NULL
                  AND ""PickupLatitude"" BETWEEN -90 AND 90 AND ""PickupLongitude"" BETWEEN -180 AND 180
                  AND NOT (""PickupLatitude"" = 0 AND ""PickupLongitude"" = 0);");

            migrationBuilder.Sql(@"
                UPDATE ""TransportRequests""
                SET ""DeliveryGeo"" = ST_SetSRID(ST_MakePoint(""DeliveryLongitude"", ""DeliveryLatitude""), 4326)::geography
                WHERE ""DeliveryLatitude"" IS NOT NULL AND ""DeliveryLongitude"" IS NOT NULL
                  AND ""DeliveryLatitude"" BETWEEN -90 AND 90 AND ""DeliveryLongitude"" BETWEEN -180 AND 180
                  AND NOT (""DeliveryLatitude"" = 0 AND ""DeliveryLongitude"" = 0);");

            // Sellers don't have their own lat/lng — backfill from the seller's
            // primary Location (joined via OwnerId/OwnerType). If a seller has
            // multiple Locations, prefer the most recently created one.
            migrationBuilder.Sql(@"
                UPDATE ""Sellers"" s
                SET ""Geo"" = sub.""Geo""
                FROM (
                    SELECT DISTINCT ON (l.""OwnerId"") l.""OwnerId"", l.""Geo""
                    FROM ""Locations"" l
                    WHERE l.""OwnerType"" = 'Seller'
                      AND l.""Geo"" IS NOT NULL
                      AND l.""IsDeleted"" = false
                    ORDER BY l.""OwnerId"", l.""CreatedAt"" DESC
                ) sub
                WHERE s.""Id"" = sub.""OwnerId"" AND s.""Geo"" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transport_trackings_geo",
                table: "TransportTrackings");

            migrationBuilder.DropIndex(
                name: "ix_transport_requests_delivery_geo",
                table: "TransportRequests");

            migrationBuilder.DropIndex(
                name: "ix_transport_requests_pickup_geo",
                table: "TransportRequests");

            migrationBuilder.DropIndex(
                name: "ix_sellers_geo",
                table: "Sellers");

            migrationBuilder.DropIndex(
                name: "ix_locations_geo",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Geo",
                table: "TransportTrackings");

            migrationBuilder.DropColumn(
                name: "DeliveryGeo",
                table: "TransportRequests");

            migrationBuilder.DropColumn(
                name: "PickupGeo",
                table: "TransportRequests");

            migrationBuilder.DropColumn(
                name: "Geo",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "Geo",
                table: "Locations");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
