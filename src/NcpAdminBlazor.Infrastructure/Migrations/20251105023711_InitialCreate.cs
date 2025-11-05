using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NcpAdminBlazor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "deliver_record",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliver_record", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PageKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Path = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDisable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PermissionCode = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Paid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Count = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色标识", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "角色名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "角色描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDisabled = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否禁用"),
                    AssignedMenuIds = table.Column<string>(type: "text", nullable: false, comment: "分配的菜单ID(逗号分隔)")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedPermissionCodes = table.Column<string>(type: "text", nullable: false, comment: "分配的权限代码(逗号分隔)")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否已删除"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "删除时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户标识", collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "用户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "手机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, comment: "密码哈希")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordSalt = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, comment: "密码盐值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RealName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "真实姓名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "邮箱地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedRoleIds = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, comment: "刷新令牌")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshExpiry = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "刷新令牌到期时间"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否删除"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "删除时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deliver_record");

            migrationBuilder.DropTable(
                name: "menus");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
