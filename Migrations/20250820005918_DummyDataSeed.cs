using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstabraqTourismAPI.Migrations
{
    /// <inheritdoc />
    public partial class DummyDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1091), "fa-landmark", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1092) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1095), "fa-mountain", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1095) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1098), "fa-mosque", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1098) });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DescriptionAr", "Icon", "IsActive", "Name", "NameAr", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1101), "Relaxing beach and coastal experiences", "تجارب شاطئية وساحلية مريحة", "fa-umbrella-beach", true, "Beach Tours", "جولات الشواطئ", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1110) },
                    { 5, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1112), "Desert adventures and Bedouin experiences", "مغامرات صحراوية وتجارب بدوية", "fa-sun", true, "Desert Safari", "سفاري الصحراء", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1113) }
                });

            migrationBuilder.UpdateData(
                table: "ContactInfo",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1545), new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1546) });

            migrationBuilder.InsertData(
                table: "ContactMessages",
                columns: new[] { "Id", "AdminReply", "CreatedAt", "Email", "Message", "Name", "Phone", "RepliedAt", "RepliedByUserId", "Status", "Subject", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1478), "abdullah.alsaad@email.com", "أرغب في الحصول على مزيد من التفاصيل حول رحلة العلا الأثرية، وما إذا كانت مناسبة للأطفال.", "عبدالله السعد", "+966501111111", null, null, "New", "استفسار عن رحلة العلا", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1480) },
                    { 2, "بالطبع يمكننا تنظيم رحلة خاصة لمجموعتكم. سيتواصل معكم فريق المبيعات خلال 24 ساعة.", new DateTime(2025, 8, 18, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1484), "nora.ahmed@email.com", "هل يمكنكم تنظيم رحلة خاصة لمجموعة من 10 أشخاص إلى الطائف؟", "نورا أحمد", "+966502222222", null, 1, "Replied", "طلب تخصيص رحلة", new DateTime(2025, 8, 19, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1485) },
                    { 3, null, new DateTime(2025, 8, 19, 18, 59, 18, 50, DateTimeKind.Utc).AddTicks(1487), "reem.alzahrani@email.com", "أحتاج إلى تغيير تاريخ حجزي من 15 سبتمبر إلى 20 سبتمبر.", "ريم الزهراني", "+966504444444", null, null, "New", "تعديل الحجز", new DateTime(2025, 8, 19, 18, 59, 18, 50, DateTimeKind.Utc).AddTicks(1488) }
                });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1623), new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1624) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1626), new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1627) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1628), new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1629) });

            migrationBuilder.UpdateData(
                table: "SiteStats",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1631), new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1631) });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DescriptionAr", "DiscountPrice", "Duration", "IsActive", "IsFeatured", "Location", "LocationAr", "MainImage", "MaxParticipants", "Price", "Title", "TitleAr", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1220), "Explore the historical landmarks of Riyadh including Masmak Fortress and National Museum", "استكشف المعالم التاريخية في الرياض بما في ذلك قلعة المصمك والمتحف الوطني", 249.99m, 6, true, true, "Riyadh, Saudi Arabia", "الرياض، المملكة العربية السعودية", "riyadh-tour.jpg", 25, 299.99m, "Riyadh Historical Tour", "جولة الرياض التاريخية", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1221) },
                    { 4, 1, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1331), "Discover the ancient Nabatean city of Hegra and stunning rock formations", "اكتشف مدينة الحجر النبطية القديمة والتشكيلات الصخرية المذهلة", 699.99m, 8, true, true, "Al-Ula, Saudi Arabia", "العلا، المملكة العربية السعودية", "alula-hegra.jpg", 20, 799.99m, "Al-Ula Archaeological Tour", "جولة العلا الأثرية", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1331) },
                    { 5, 3, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1342), "Religious and historical tour around the holy city of Mecca", "جولة دينية وتاريخية حول المدينة المقدسة مكة", 179.99m, 3, true, true, "Mecca, Saudi Arabia", "مكة المكرمة، المملكة العربية السعودية", "mecca-ziyarat.jpg", 35, 199.99m, "Mecca Ziyarat Tour", "جولة زيارة مكة", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1342) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 59, 17, 577, DateTimeKind.Utc).AddTicks(9509), "$2a$11$JsJEv1/gXv0HBYBC26oUVeET0fGKwioDTqhGQh4ILu8MB5T1t6VMC", new DateTime(2025, 8, 20, 0, 59, 17, 577, DateTimeKind.Utc).AddTicks(9512) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "Phone", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2025, 8, 20, 0, 59, 17, 736, DateTimeKind.Utc).AddTicks(3408), "ahmed.mohamed@example.com", true, "أحمد محمد", "$2a$11$oaFIyKhlvqt/L68ZziG2NOn72qRjTLBNLrNSkWSs3jTBHsVUg4xha", "+966501234567", "Customer", new DateTime(2025, 8, 20, 0, 59, 17, 736, DateTimeKind.Utc).AddTicks(3413) },
                    { 3, null, new DateTime(2025, 8, 20, 0, 59, 17, 896, DateTimeKind.Utc).AddTicks(7368), "fatima.ahmed@example.com", true, "فاطمة أحمد", "$2a$11$9jKTxVmkgdaa6f9Lsp14f.WoINNoWeoyV5Fpknf6jTEdNgfPlpbFm", "+966501234568", "Customer", new DateTime(2025, 8, 20, 0, 59, 17, 896, DateTimeKind.Utc).AddTicks(7370) },
                    { 4, null, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(219), "mohammed.ali@example.com", true, "محمد علي", "$2a$11$tE4NMf3VtHJ2EIYzSB2egeYxMb636ZwhuyYg10qfmtA3Cyv2IST5m", "+966501234569", "Customer", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(223) }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AdminNotes", "BookingDate", "BookingReference", "CreatedAt", "CustomerEmail", "CustomerName", "CustomerPhone", "NumberOfPeople", "SpecialRequests", "Status", "TotalPrice", "TripId", "UpdatedAt", "UserId" },
                values: new object[] { 1, null, new DateTime(2025, 9, 19, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1415), "BK202501001", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1424), "ahmed.mohamed@example.com", "أحمد محمد", "+966501234567", 2, "سيارة إضافية للعائلة", "Confirmed", 499.98m, 1, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1426), 2 });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DescriptionAr", "DiscountPrice", "Duration", "IsActive", "IsFeatured", "Location", "LocationAr", "MainImage", "MaxParticipants", "Price", "Title", "TitleAr", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, 4, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1226), "Beautiful coastal walk along Jeddah Corniche with stunning Red Sea views", "نزهة ساحلية جميلة على طول كورنيش جدة مع إطلالات خلابة على البحر الأحمر", null, 4, true, true, "Jeddah, Saudi Arabia", "جدة، المملكة العربية السعودية", "jeddah-corniche.jpg", 30, 199.99m, "Jeddah Corniche Walk", "جولة كورنيش جدة", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1226) },
                    { 3, 5, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1233), "Adventure through the largest sand desert in the world with camel riding and camping", "مغامرة عبر أكبر صحراء رملية في العالم مع ركوب الجمال والتخييم", 499.99m, 24, true, true, "Empty Quarter, Saudi Arabia", "الربع الخالي، المملكة العربية السعودية", "empty-quarter.jpg", 15, 599.99m, "Empty Quarter Desert Safari", "سفاري الربع الخالي", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1234) }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AdminNotes", "BookingDate", "BookingReference", "CreatedAt", "CustomerEmail", "CustomerName", "CustomerPhone", "NumberOfPeople", "SpecialRequests", "Status", "TotalPrice", "TripId", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2025, 10, 4, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1429), "BK202501002", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1430), "fatima.ahmed@example.com", "فاطمة أحمد", "+966501234568", 1, null, "Pending", 499.99m, 3, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1430), 3 },
                    { 3, null, new DateTime(2025, 9, 4, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1433), "BK202501003", new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1434), "mohammed.ali@example.com", "محمد علي", "+966501234569", 3, "وجبة نباتية", "Confirmed", 599.97m, 2, new DateTime(2025, 8, 20, 0, 59, 18, 50, DateTimeKind.Utc).AddTicks(1434), 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ContactMessages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContactMessages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ContactMessages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5331), null, new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5332) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5335), null, new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5336) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Icon", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5339), null, new DateTime(2025, 8, 20, 0, 47, 17, 637, DateTimeKind.Utc).AddTicks(5339) });

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
    }
}
