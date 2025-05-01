using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moe.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColmBannedForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "SystemSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "PendingUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "LocalizedContents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Countries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Cities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "PendingUsers");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "LocalizedContents");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Cities");
        }
    }
}
