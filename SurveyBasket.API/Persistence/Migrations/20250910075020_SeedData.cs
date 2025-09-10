using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "B42E6B6B-223D-44FB-9AF2-504DB57A1C72", "14714DAF-3173-4A1A-9E61-2855F7507F88", true, false, "Member", "MEMBER" },
                    { "CAC716C1-8303-40EE-9E89-B66639503CE5", "226B9800-6050-48D5-B5EF-4318DB30449A", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5", 0, "4E136CDF-FBC8-44AC-9764-2CFCB543E798", "admin@survey.com", true, "SurveyBasket", "LastSurveyBasket", false, null, "ADMIN@SURVEY.COM", "ADMIN@SURVEY.COM", "AQAAAAIAAYagAAAAEMWSIFAQf9pi6w6qdEjFLCxPi77vWrg2QmaaAa1kwKuLDTty/W3d61xqwZJwO51sVA==", null, false, "37CB4836-B3FE-4D99-AF30-45FBEB2E8546", false, "admin@survey.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 2, "permissions", "polls:add", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 3, "permissions", "polls:update", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 4, "permissions", "polls:delete", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 5, "permissions", "questions:read", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 6, "permissions", "questions:add", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 7, "permissions", "questions:update", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 8, "permissions", "users:read", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 9, "permissions", "users:add", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 10, "permissions", "users:update", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 11, "permissions", "users:read", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 12, "permissions", "users:add", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 13, "permissions", "users:update", "CAC716C1-8303-40EE-9E89-B66639503CE5" },
                    { 14, "permissions", "results:read", "CAC716C1-8303-40EE-9E89-B66639503CE5" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "CAC716C1-8303-40EE-9E89-B66639503CE5", "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B42E6B6B-223D-44FB-9AF2-504DB57A1C72");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "CAC716C1-8303-40EE-9E89-B66639503CE5", "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "CAC716C1-8303-40EE-9E89-B66639503CE5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5");
        }
    }
}
