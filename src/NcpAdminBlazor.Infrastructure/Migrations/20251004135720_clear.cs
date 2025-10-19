using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NcpAdminBlazor.Infrastructure.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
    public partial class clear : Migration
#pragma warning restore CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
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
