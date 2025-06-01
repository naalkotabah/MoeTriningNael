using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moe.Core.Migrations
{
    /// <inheritdoc />
    public partial class Addwarhouseuponitemcreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Items_ItemId",
                table: "WarehouseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_FromWarehouseId",
                table: "WarehouseItemTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_ToWarehouseId",
                table: "WarehouseItemTransactions");

            migrationBuilder.RenameColumn(
                name: "ToWarehouseId",
                table: "WarehouseItemTransactions",
                newName: "ToId");

            migrationBuilder.RenameColumn(
                name: "Qtu",
                table: "WarehouseItemTransactions",
                newName: "Qty");

            migrationBuilder.RenameColumn(
                name: "FromWarehouseId",
                table: "WarehouseItemTransactions",
                newName: "FromId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseItemTransactions_ToWarehouseId",
                table: "WarehouseItemTransactions",
                newName: "IX_WarehouseItemTransactions_ToId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseItemTransactions_FromWarehouseId",
                table: "WarehouseItemTransactions",
                newName: "IX_WarehouseItemTransactions_FromId");

            migrationBuilder.RenameColumn(
                name: "Qtu",
                table: "WarehouseItems",
                newName: "Qty");

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Items_ItemId",
                table: "WarehouseItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_FromId",
                table: "WarehouseItemTransactions",
                column: "FromId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_ToId",
                table: "WarehouseItemTransactions",
                column: "ToId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Items_ItemId",
                table: "WarehouseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_FromId",
                table: "WarehouseItemTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_ToId",
                table: "WarehouseItemTransactions");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "ToId",
                table: "WarehouseItemTransactions",
                newName: "ToWarehouseId");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "WarehouseItemTransactions",
                newName: "Qtu");

            migrationBuilder.RenameColumn(
                name: "FromId",
                table: "WarehouseItemTransactions",
                newName: "FromWarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseItemTransactions_ToId",
                table: "WarehouseItemTransactions",
                newName: "IX_WarehouseItemTransactions_ToWarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseItemTransactions_FromId",
                table: "WarehouseItemTransactions",
                newName: "IX_WarehouseItemTransactions_FromWarehouseId");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "WarehouseItems",
                newName: "Qtu");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Items_ItemId",
                table: "WarehouseItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Warehouses_WarehouseId",
                table: "WarehouseItems",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_FromWarehouseId",
                table: "WarehouseItemTransactions",
                column: "FromWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItemTransactions_Warehouses_ToWarehouseId",
                table: "WarehouseItemTransactions",
                column: "ToWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
