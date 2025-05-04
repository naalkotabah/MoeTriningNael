using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moe.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailandPhonChang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeEmailRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeEmailRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChangePhoneRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewPhone = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Otp = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePhoneRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeEmailRequest");

            migrationBuilder.DropTable(
                name: "ChangePhoneRequest");
        }
    }
}
