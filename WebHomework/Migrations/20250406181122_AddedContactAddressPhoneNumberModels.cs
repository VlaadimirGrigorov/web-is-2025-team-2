using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebHomework.Migrations
{
    /// <inheritdoc />
    public partial class AddedContactAddressPhoneNumberModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Contacts_ContactId",
                table: "Address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Address_ContactId",
                table: "addresses",
                newName: "IX_addresses_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_addresses",
                table: "addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_Contacts_ContactId",
                table: "addresses",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_Contacts_ContactId",
                table: "addresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_addresses",
                table: "addresses");

            migrationBuilder.RenameTable(
                name: "addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_addresses_ContactId",
                table: "Address",
                newName: "IX_Address_ContactId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Contacts_ContactId",
                table: "Address",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");
        }
    }
}
