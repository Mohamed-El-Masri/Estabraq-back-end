using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstabraqTourismAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2252), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2252) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2255), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2256) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2258), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2258) });

            migrationBuilder.UpdateData(
                table: "ContactInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2309), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2310) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2365), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2366) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2369), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2370) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2372), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2377) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2379), new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(2379) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(1420), "$2a$11$DhPgcCiLUcn4ZN8/feOCZ.BZZSRQ477JyEWLuC/l93pLJUtTr1uqy", new DateTime(2025, 8, 20, 0, 4, 16, 623, DateTimeKind.Utc).AddTicks(1423) });
        }
    }
}
