using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimValue",
                value: "roles:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimValue",
                value: "roles:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimValue",
                value: "roles:update");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIeFBD4wTkQ7OrmJiAr6NThbTn7I5GxSIcqYYi9TW8n3NNe527mww28g95Dc3acXgA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimValue",
                value: "users:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimValue",
                value: "users:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimValue",
                value: "users:update");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMWSIFAQf9pi6w6qdEjFLCxPi77vWrg2QmaaAa1kwKuLDTty/W3d61xqwZJwO51sVA==");
        }
    }
}
