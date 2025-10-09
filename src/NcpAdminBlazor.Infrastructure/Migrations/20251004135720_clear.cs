using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NcpAdminBlazor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class clear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_application_users_Email",
                table: "application_users");

            migrationBuilder.DropIndex(
                name: "IX_application_users_Phone",
                table: "application_users");

            migrationBuilder.DropIndex(
                name: "IX_application_users_Username",
                table: "application_users");

            migrationBuilder.CreateIndex(
                name: "IX_application_users_Username",
                table: "application_users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_application_users_Username",
                table: "application_users");

            migrationBuilder.CreateIndex(
                name: "IX_application_users_Email",
                table: "application_users",
                column: "Email",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_application_users_Phone",
                table: "application_users",
                column: "Phone",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_application_users_Username",
                table: "application_users",
                column: "Username",
                unique: true,
                filter: "IsDeleted = 0");
        }
    }
}
