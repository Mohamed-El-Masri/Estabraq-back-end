using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstabraqTourismAPI.Migrations
{
    /// <inheritdoc />
    public partial class DummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5331), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5332) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5335), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5336) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5339), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5339) });

            migrationBuilder.UpdateData(
                table: "ContactInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5405), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5406) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5468), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5469) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5473), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5473) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5475), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5491) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5493), new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5494) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(4651), "$2a$11$hgm7MIdCswuQkshyV.xYCO0dbxIN2ZAXghgzgTLwRHqAKBSJ4Khta", new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(4655) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(308), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(309) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(313), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(313) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(317), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(318) });

            migrationBuilder.UpdateData(
                table: "ContactInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(402), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(404) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(469), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(471) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(477), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(477) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(480), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(485) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(487), new DateTime(2025, 8, 20, 0, 44, 35, 361, DateTimeKind.Utc).AddTicks(488) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 44, 35, 360, DateTimeKind.Utc).AddTicks(9314), "$2a$11$DpusbOQ71w91OwvcWaJmE.R3xZG.ZSRAHj3EDyUoolFKfLI0n4EIC", new DateTime(2025, 8, 20, 0, 44, 35, 360, DateTimeKind.Utc).AddTicks(9318) });
        }
    }
}
