using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnInApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAEFbP+1Rvd9It/gAQeypeoGt0RX4KQt0upsENGa+Akin6t0uq7ZgMfDQsLEc+lDjgmw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C3C3118D-E13A-4DE5-AB70-1B2788AC18C5",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIeFBD4wTkQ7OrmJiAr6NThbTn7I5GxSIcqYYi9TW8n3NNe527mww28g95Dc3acXgA==");
        }
    }
}
